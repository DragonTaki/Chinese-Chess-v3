/* ----- ----- ----- ----- */
// BuiltInEngine.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.3
/* ----- ----- ----- ----- */

using System.Diagnostics;
using Chinese_Chess_v3.Game.Core.Pgn;

namespace Chinese_Chess_v3.Game.Ai;

/// <summary>Mailbox xiangqi search with Core legality at the root.</summary>
public sealed class BuiltInEngine : IChessEngine
{
    private readonly EngineLevel _level;
    private readonly Random _random;
    private readonly EngineLevelProfile _profile;
    private readonly SemaphoreSlim _gate = new(1);
    private readonly CancellationTokenSource _lifetime = new();
    private bool _disposed;
    public string Name => $"Built-in {_level}";

    private BuiltInEngine(EngineLevel level, Random random, EngineLevelProfile profile)
    { _level = level; _random = random; _profile = profile; }

    /// <summary>Creates an engine; an optional seeded random source makes move selection reproducible.</summary>
    public static BuiltInEngine Create(EngineLevel level, Random? random = null, EngineLevelProfile? profile = null)
    {
        if (!Enum.IsDefined(level)) throw new ArgumentOutOfRangeException(nameof(level));
        profile ??= EngineLevelProfile.For(level);
        if (profile.DepthCap is < 2 or > 64 || profile.TimeBudget <= TimeSpan.Zero || profile.WindowCp is < 0 or > 150
            || profile.WeightPower is < 1 or > 3 || !profile.ProtectImmediateMate)
            throw new ArgumentOutOfRangeException(nameof(profile));
        return new(level, random ?? new Random(), profile);
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
            return await Task.Run(() => Analyze(position, limits, linked.Token), linked.Token).ConfigureAwait(false);
        }
        finally { _gate.Release(); }
    }

    /// <summary>Standard legal moves, without applying custom rule switches.</summary>
    public static IReadOnlyList<IccsMove> GenerateLegalMoves(string fen)
    {
        var parsed = XiangqiFen.Parse(fen);
        return new Mailbox(parsed.Pieces, parsed.FirstColor == Core.Pieces.PieceColor.Red ? 1 : -1).LegalMoves();
    }

    /// <summary>Static material and piece-square score from the FEN's side to move.</summary>
    public static int Evaluate(string fen)
    {
        var parsed = XiangqiFen.Parse(fen);
        return new Mailbox(parsed.Pieces, parsed.FirstColor == Core.Pieces.PieceColor.Red ? 1 : -1).Evaluate();
    }

    private EngineResult Analyze(EnginePosition input, SearchLimits limits, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var (core, side, board, history) = PositionLoader.Load(input, ct);
        var roots = PositionLoader.RootMoves(core, side);
        if (roots.Count == 0) return new(null, -Search.Mate, 0, 0, Array.Empty<IccsMove>(), 0, 0);
        if (history.Count(h => h == board.Hash) >= 3)
            return new(roots[0], 0, null, 0.5, new[] { roots[0] }, 1, 0);
        // Complete the tactical floor before applying time or node limits.
        var winning = new List<IccsMove>();
        var safe = new List<IccsMove>();
        foreach (var move in roots)
        {
            ct.ThrowIfCancellationRequested();
            int captured = board.Make(move);
            try
            {
                var replies = board.LegalMoves();
                if (replies.Count == 0) { winning.Add(move); continue; }
                bool loses = false;
                foreach (var reply in replies)
                {
                    ct.ThrowIfCancellationRequested();
                    int taken = board.Make(reply);
                    try { if (board.LegalMoves().Count == 0) { loses = true; break; } }
                    finally { board.Undo(reply, taken); }
                }
                if (!loses) safe.Add(move);
            }
            finally { board.Undo(move, captured); }
        }
        if (winning.Count > 0)
            return new(winning[0], Search.Mate - 1, 1, 1, new[] { winning[0] }, 2, 0);
        if (safe.Count > 0) roots = safe;
        int maxDepth = Math.Max(2, limits.Depth ?? _profile.DepthCap);
        TimeSpan budget = limits.MoveTime == TimeSpan.Zero && limits.OwnRemainingMs == null
            ? _profile.TimeBudget : limits.Budget;
        var search = new Search(board, history, limits.Nodes, budget, ct, _profile.WindowCp, _profile.WeightPower, _random);
        var result = search.Run(roots, maxDepth);
        if (result.BestMove.HasValue && PositionLoader.IsLegal(core, side, result.BestMove.Value)) return result;
        var fallback = roots.First(m => PositionLoader.IsLegal(core, side, m));
        return result with { BestMove = fallback, Pv = new[] { fallback } };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetime.Cancel();
    }

    private sealed class Search
    {
        internal const int Mate = 30000;
        private readonly Mailbox _board;
        private readonly List<ulong> _history;
        private readonly Dictionary<ulong, (int Depth, int Score, IccsMove Move)> _table = new();
        private readonly IccsMove?[,] _killers = new IccsMove?[128, 2];
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private readonly TimeSpan _budget;
        private readonly long? _nodeLimit;
        private readonly CancellationToken _ct;
        private readonly int _tolerance;
        private readonly int _weightPower;
        private readonly Random _random;
        private long _nodes;
        private bool _enforceBudget;
        private sealed class BudgetExpired : Exception { }

        internal Search(Mailbox board, List<ulong> history, long? nodeLimit, TimeSpan budget, CancellationToken ct,
            int tolerance, int weightPower, Random random)
        { _board = board; _history = history; _nodeLimit = nodeLimit; _budget = budget; _ct = ct; _tolerance = tolerance; _weightPower = weightPower; _random = random; }

        private void Visit()
        {
            _ct.ThrowIfCancellationRequested();
            if (_enforceBudget && (_clock.Elapsed >= _budget || (_nodeLimit.HasValue && _nodes >= _nodeLimit)))
                throw new BudgetExpired();
            _nodes++;
        }

        internal EngineResult Run(List<IccsMove> roots, int maxDepth)
        {
            var best = roots[0];
            int score = _board.Evaluate(), completed = 0;
            IReadOnlyList<IccsMove> pv = new[] { best };
            var candidates = new List<(IccsMove Move, int Score, IReadOnlyList<IccsMove> Pv)>();
            for (int depth = 1; depth <= maxDepth; depth++)
            {
                _enforceBudget = depth > 2;
                int iterationScore = -Mate - 1;
                var iterationBest = best;
                var iterationPv = pv;
                var iterationCandidates = new List<(IccsMove Move, int Score, IReadOnlyList<IccsMove> Pv)>();
                try
                {
                    foreach (var move in roots.OrderByDescending(m => m == best ? int.MaxValue : _board.Ordering(m)))
                    {
                        Visit();
                        int captured = _board.Make(move);
                        _history.Add(_board.Hash);
                        int value;
                        List<IccsMove> child;
                        // Every root receives an exact score, independent of the selection window.
                        try { value = -Negamax(depth - 1, -Mate - 1, Mate + 1, 1, out child); }
                        finally { _history.RemoveAt(_history.Count - 1); _board.Undo(move, captured); }
                        iterationCandidates.Add((move, value, new[] { move }.Concat(child).ToArray()));
                        if (value > iterationScore)
                        {
                            iterationScore = value; iterationBest = move;
                            iterationPv = new[] { move }.Concat(child).ToArray();
                        }
                    }
                    score = iterationScore; best = iterationBest; pv = iterationPv; completed = depth;
                    candidates = iterationCandidates;
                }
                catch (BudgetExpired) { break; }
                if (Math.Abs(score) >= Mate - 128) break;
            }
            if (candidates.Count > 0)
            {
                int tolerance = Math.Abs(score) >= Mate - 128 ? 0 : _tolerance;
                var choices = candidates.Where(c => c.Score >= score - tolerance).ToArray();
                var chosen = choices[0];
                if (tolerance > 0 && choices.Length > 1)
                {
                    var weights = choices.Select(c => (int)Math.Pow(1 + 9.0 *
                        (tolerance - (score - c.Score)) / tolerance, _weightPower)).ToArray();
                    int ticket = _random.Next(weights.Sum());
                    for (int i = 0; i < choices.Length; i++)
                    {
                        ticket -= weights[i];
                        if (ticket < 0) { chosen = choices[i]; break; }
                    }
                }
                best = chosen.Move; score = chosen.Score; pv = chosen.Pv;
            }
            int? mate = Math.Abs(score) >= Mate - 128 ? Math.Sign(score) * ((Mate - Math.Abs(score) + 1) / 2) : null;
            return new(best, score, mate, mate.HasValue ? (score > 0 ? 1 : 0) : EngineResult.Probability(score), pv, completed, _nodes);
        }

        private int Negamax(int depth, int alpha, int beta, int ply, out List<IccsMove> pv)
        {
            Visit();
            pv = new();
            if (_history.Count(h => h == _board.Hash) >= 3) return 0;
            if (depth <= 0) return Quiescence(alpha, beta, ply, 0);
            var moves = _board.LegalMoves();
            if (moves.Count == 0) return -Mate + ply;
            _table.TryGetValue(_board.Hash, out var entry);
            moves.Sort((a, b) => Order(b, entry.Move, ply).CompareTo(Order(a, entry.Move, ply)));
            int best = -Mate - 1;
            IccsMove bestMove = moves[0];
            foreach (var move in moves)
            {
                int captured = _board.Make(move);
                _history.Add(_board.Hash);
                int score;
                List<IccsMove> child;
                try { score = -Negamax(depth - 1, -beta, -alpha, ply + 1, out child); }
                finally { _history.RemoveAt(_history.Count - 1); _board.Undo(move, captured); }
                if (score > best) { best = score; bestMove = move; pv = new() { move }; pv.AddRange(child); }
                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                {
                    if (captured == 0 && ply < 128) { _killers[ply, 1] = _killers[ply, 0]; _killers[ply, 0] = move; }
                    break;
                }
            }
            // Use transpositions for ordering only: repetition makes cached bounds history-dependent.
            if (_table.Count < 100000) _table[_board.Hash] = (depth, best, bestMove);
            return best;
        }

        private int Order(IccsMove move, IccsMove tableMove, int ply) => move == tableMove ? 1000000 :
            _board.CaptureValue(move) > 0 ? 100000 + _board.Ordering(move) :
            ply < 128 && _killers[ply, 0] == move ? 90000 : ply < 128 && _killers[ply, 1] == move ? 80000 : 0;

        private int Quiescence(int alpha, int beta, int ply, int qdepth)
        {
            Visit();
            if (_history.Count(h => h == _board.Hash) >= 3) return 0;
            bool check = _board.InCheck(_board.Side);
            var legal = _board.LegalMoves();
            if (legal.Count == 0) return -Mate + ply;
            int stand = _board.Evaluate();
            if (qdepth >= 8) return stand;
            if (!check)
            {
                if (stand >= beta) return stand;
                alpha = Math.Max(alpha, stand);
            }
            foreach (var move in legal.Where(m => check || _board.CaptureValue(m) > 0).OrderByDescending(_board.Ordering))
            {
                int captured = _board.Make(move);
                _history.Add(_board.Hash);
                int score;
                try { score = -Quiescence(-beta, -alpha, ply + 1, qdepth + 1); }
                finally { _history.RemoveAt(_history.Count - 1); _board.Undo(move, captured); }
                if (score >= beta) return score;
                alpha = Math.Max(alpha, score);
            }
            return alpha;
        }
    }
}
