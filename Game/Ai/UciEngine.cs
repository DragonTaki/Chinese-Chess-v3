/* ----- ----- ----- ----- */
// UciEngine.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Diagnostics;
using System.Globalization;
using Chinese_Chess_v3.Game.Core.Pgn;

namespace Chinese_Chess_v3.Game.Ai;

/// <summary>Asynchronous UCI transport for user-supplied xiangqi engines.</summary>
public sealed class UciEngine : IChessEngine
{
    private readonly string _path;
    private readonly string[] _arguments;
    private readonly TimeSpan _timeout;
    private readonly SemaphoreSlim _gate = new(1);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly object _processLock = new();
    private Process? _process;
    private bool _disposed;
    private bool _supportsWdl;
    public string Name { get; private set; }

    public UciEngine(string path, IReadOnlyList<string>? args = null, TimeSpan? timeout = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = Path.GetFullPath(path);
        _arguments = args?.ToArray() ?? Array.Empty<string>();
        _timeout = timeout ?? TimeSpan.FromSeconds(5);
        if (_timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        Name = Path.GetFileNameWithoutExtension(path);
    }

    public async Task<EngineResult> AnalyzeAsync(EnginePosition position, SearchLimits limits, CancellationToken ct)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(limits);
        limits.Validate();
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, _lifetime.Token);
        await _gate.WaitAsync(linked.Token).ConfigureAwait(false);
        try
        {
            var loaded = await Task.Run(() => PositionLoader.Load(position, linked.Token), linked.Token).ConfigureAwait(false);
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(linked.Token);
            deadline.CancelAfter(limits.Budget + _timeout + _timeout);
            using var kill = deadline.Token.Register(KillProcess);
            try
            {
                var start = new ProcessStartInfo(_path)
                {
                    UseShellExecute = false, RedirectStandardInput = true,
                    RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true,
                };
                foreach (string arg in _arguments) start.ArgumentList.Add(arg);
                using var process = new Process { StartInfo = start };
                lock (_processLock)
                {
                    deadline.Token.ThrowIfCancellationRequested();
                    process.Start();
                    _process = process;
                }
                Task stderr = DrainErrors(process, deadline.Token);
                try
                {
                    await Send(process, "uci", deadline.Token).ConfigureAwait(false);
                    await Until(process, "uciok", deadline.Token).ConfigureAwait(false);
                    if (_supportsWdl) await Send(process, "setoption name UCI_ShowWDL value true", deadline.Token).ConfigureAwait(false);
                    await Send(process, "isready", deadline.Token).ConfigureAwait(false);
                    await Until(process, "readyok", deadline.Token).ConfigureAwait(false);
                    await Send(process, "ucinewgame", deadline.Token).ConfigureAwait(false);
                    await Send(process, "isready", deadline.Token).ConfigureAwait(false);
                    await Until(process, "readyok", deadline.Token).ConfigureAwait(false);
                    string fen = position.Fen.Trim();
                    if (fen.Contains('\n') || fen.Contains('\r')) throw new ArgumentException("FEN must occupy one line.", nameof(position));
                    string moves = position.EarlierMoves is { Count: > 0 } ? " moves " + string.Join(" ", position.EarlierMoves) : "";
                    await Send(process, "position fen " + fen + moves, deadline.Token).ConfigureAwait(false);
                    await Send(process, Go(limits, loaded.Position.Side), deadline.Token).ConfigureAwait(false);
                    var info = new Info();
                    while (true)
                    {
                        string line = await Read(process, deadline.Token).ConfigureAwait(false);
                        if (line.StartsWith("info ", StringComparison.Ordinal)) info.Parse(line);
                        if (!line.StartsWith("bestmove ", StringComparison.Ordinal)) continue;
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        IccsMove? best = parts.Length > 1 && IccsMove.TryParse(parts[1], out var move) ? move : null;
                        if (best.HasValue && !PositionLoader.IsLegal(loaded.Board, loaded.Side, best.Value))
                            throw new InvalidDataException("External engine returned an illegal move under the supplied rules.");
                        if (!best.HasValue && PositionLoader.RootMoves(loaded.Board, loaded.Side).Count > 0)
                            throw new InvalidDataException("External engine returned no move in a playable position.");
                        return new(best, info.Score, info.Mate, info.Probability ??
                            (info.Mate.HasValue ? (info.Mate > 0 ? 1 : 0) : EngineResult.Probability(info.Score)),
                            info.Pv.ToArray(), info.Depth, info.Nodes);
                    }
                }
                finally
                {
                    KillProcess();
                    await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
                    await stderr.ConfigureAwait(false);
                    lock (_processLock) _process = null;
                }
            }
            catch (Exception ex) when (deadline.IsCancellationRequested)
            {
                linked.Token.ThrowIfCancellationRequested();
                throw new TimeoutException("The UCI engine did not respond before its deadline.", ex);
            }
        }
        finally { _gate.Release(); }
    }

    private static string Go(SearchLimits limits, int side)
    {
        string command = "go";
        if (limits.OwnRemainingMs.HasValue && limits.OpponentRemainingMs.HasValue)
        {
            long white = side == 1 ? limits.OwnRemainingMs.Value : limits.OpponentRemainingMs.Value;
            long black = side == -1 ? limits.OwnRemainingMs.Value : limits.OpponentRemainingMs.Value;
            command += $" wtime {white} btime {black} winc {limits.IncrementMs} binc {limits.IncrementMs}";
        }
        if (limits.MoveTime > TimeSpan.Zero) command += " movetime " + Math.Max(1, (long)limits.MoveTime.TotalMilliseconds);
        if (limits.Depth.HasValue) command += " depth " + limits.Depth.Value;
        if (limits.Nodes.HasValue) command += " nodes " + limits.Nodes.Value;
        if (command == "go") command += " movetime " + (long)limits.Budget.TotalMilliseconds;
        return command;
    }

    private static async Task Send(Process process, string command, CancellationToken ct)
    {
        await process.StandardInput.WriteLineAsync(command.AsMemory(), ct).ConfigureAwait(false);
        await process.StandardInput.FlushAsync(ct).ConfigureAwait(false);
    }

    private static async Task<string> Read(Process process, CancellationToken ct) =>
        await process.StandardOutput.ReadLineAsync(ct).ConfigureAwait(false) ?? throw new IOException("UCI engine closed its output.");

    private async Task Until(Process process, string marker, CancellationToken ct)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(_timeout);
        try
        {
            while (true)
            {
                string line = await Read(process, deadline.Token).ConfigureAwait(false);
                if (line == marker) return;
                if (line.StartsWith("option name UCI_ShowWDL type check", StringComparison.Ordinal)) _supportsWdl = true;
                if (line.StartsWith("id name ", StringComparison.Ordinal)) Name = line[8..];
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new TimeoutException($"UCI engine did not send {marker}."); }
    }

    private static async Task DrainErrors(Process process, CancellationToken ct)
    {
        try { while (await process.StandardError.ReadLineAsync(ct).ConfigureAwait(false) != null) { } }
        catch (OperationCanceledException) { }
        catch (IOException) { }
    }

    private void KillProcess()
    {
        lock (_processLock)
        {
            try { if (_process is { HasExited: false }) _process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) { }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetime.Cancel();
        KillProcess();
    }

    private sealed class Info
    {
        internal int Score, Depth;
        internal int? Mate;
        internal long Nodes;
        internal double? Probability;
        internal List<IccsMove> Pv = new();

        internal void Parse(string line)
        {
            string[] fields = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int multi = Array.IndexOf(fields, "multipv");
            if (multi >= 0 && multi + 1 < fields.Length && fields[multi + 1] != "1") return;
            double? wdl = null;
            for (int i = 1; i < fields.Length; i++)
            {
                if (fields[i] == "string") break;
                if (fields[i] == "depth" && i + 1 < fields.Length && int.TryParse(fields[i + 1], out int depth)) Depth = depth;
                else if (fields[i] == "nodes" && i + 1 < fields.Length && long.TryParse(fields[i + 1], out long nodes)) Nodes = nodes;
                else if (fields[i] == "score" && i + 2 < fields.Length && int.TryParse(fields[i + 2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int score))
                {
                    if (fields[i + 1] == "cp") { Score = score; Mate = null; Probability = null; }
                    else if (fields[i + 1] == "mate") { Mate = score; Score = Math.Sign(score) * 30000; Probability = null; }
                }
                else if (fields[i] == "wdl" && i + 3 < fields.Length &&
                    int.TryParse(fields[i + 1], out int win) && int.TryParse(fields[i + 2], out int draw) && int.TryParse(fields[i + 3], out int loss) &&
                    win >= 0 && draw >= 0 && loss >= 0 && (long)win + draw + loss > 0)
                    wdl = win / (double)((long)win + draw + loss);
                else if (fields[i] == "pv")
                {
                    Pv = new();
                    for (int j = i + 1; j < fields.Length && IccsMove.TryParse(fields[j], out var move); j++) Pv.Add(move);
                    break;
                }
            }
            if (wdl.HasValue) Probability = wdl;
        }
    }
}
