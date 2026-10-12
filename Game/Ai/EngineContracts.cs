/* ----- ----- ----- ----- */
// EngineContracts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pgn;

namespace Chinese_Chess_v3.Game.Ai;

/// <summary>A position analyzer independent of the client and its display.</summary>
public interface IChessEngine : IDisposable
{
    string Name { get; }
    Task<EngineResult> AnalyzeAsync(EnginePosition position, SearchLimits limits, CancellationToken ct);
}

/// <summary>Starting FEN and chronological moves to replay to reach the analyzed position.</summary>
public sealed record EnginePosition(string Fen, Rules Rules, IReadOnlyList<IccsMove>? EarlierMoves = null);

/// <summary>Search bounds. Clocks are milliseconds for the current mover and its opponent.</summary>
public sealed record SearchLimits(TimeSpan MoveTime, int? Depth = null, long? Nodes = null,
    long? OwnRemainingMs = null, long? OpponentRemainingMs = null, long IncrementMs = 0)
{
    internal void Validate()
    {
        if (MoveTime < TimeSpan.Zero || Depth is <= 0 or > 64 || Nodes is <= 0 ||
            OwnRemainingMs is < 0 || OpponentRemainingMs is < 0 || IncrementMs < 0)
            throw new ArgumentOutOfRangeException(nameof(SearchLimits));
    }

    internal TimeSpan Budget => MoveTime > TimeSpan.Zero ? MoveTime :
        OwnRemainingMs.HasValue ? TimeSpan.FromMilliseconds(Math.Max(1,
            Math.Min(OwnRemainingMs.Value, OwnRemainingMs.Value / 30 + IncrementMs))) : TimeSpan.FromSeconds(1);
}

/// <summary>Scores, mate distance and winning probability are relative to the side to move.</summary>
public sealed record EngineResult(IccsMove? BestMove, int ScoreCp, int? MateIn,
    double WinProbability, IReadOnlyList<IccsMove> Pv, int Depth, long Nodes)
{
    internal static double Probability(int score) => 1 / (1 + Math.Exp(-score / 400.0));
}

/// <summary>Increasing search effort after the random and one-ply material levels.</summary>
public enum EngineLevel { Random, Greedy, SearchDepth2, SearchDepth4, Search500Ms, Search2000Ms }
