/* ----- ----- ----- ----- */
// GamePayloads.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chinese_Chess_v3.Network.Protocol
{
    // The game packets' payloads, as the server (game.go, rules/protocol.go) sends and takes them.
    // Squares are [x, y] in FEN coordinates: x 0-8 from the left, y 0-9 from the top, red at the
    // bottom (y 9). Players are numbered by turn order (1 moves first); arrays by player are
    // 0-based (index 0 is Player1).

    /// <summary>One piece of a position.</summary>
    public sealed record PieceDto
    {
        /// <summary>The piece type (General, Advisor, Elephant, Chariot, Horse, Cannon, Soldier).</summary>
        public string Type { get; init; }

        /// <summary>Red or Black.</summary>
        public string Color { get; init; }

        /// <summary>The owning player (1, 2...; 0 while undecided).</summary>
        public int Side { get; init; }

        /// <summary>The column (0-8).</summary>
        public int X { get; init; }

        /// <summary>The row (0-9, red at 9).</summary>
        public int Y { get; init; }

        /// <summary>Whether the piece is face up.</summary>
        public bool FaceUp { get; init; }
    }

    /// <summary>A position: whose turn it is and every piece.</summary>
    public sealed record PositionDto
    {
        /// <summary>The player to move (1, 2...).</summary>
        public int ToMove { get; init; }

        /// <summary>The pieces on the board.</summary>
        public List<PieceDto> Pieces { get; init; }
    }

    /// <summary>A validated move's record.</summary>
    public sealed record MoveRecordDto
    {
        /// <summary>The game kind.</summary>
        public string Kind { get; init; }

        /// <summary>The move in Chinese notation.</summary>
        public string Notation { get; init; }

        /// <summary>The captured piece, or null.</summary>
        public PieceDto Captured { get; init; }

        /// <summary>The piece a dark-chess move revealed, or null.</summary>
        public PieceDto Revealed { get; init; }
    }

    /// <summary>One move of a game's record as a resumed StartGame may carry it.</summary>
    public sealed record MoveEntryDto
    {
        /// <summary>The square moved from ([x, y]).</summary>
        public int[] From { get; init; }

        /// <summary>The square moved to ([x, y]).</summary>
        public int[] To { get; init; }

        /// <summary>The move in Chinese notation.</summary>
        public string Notation { get; init; }

        /// <summary>The time the mover spent, ms.</summary>
        public long Ms { get; init; }
    }

    /// <summary>One side's clock, ms. <see cref="UsedMs"/> may be negative (time added by increments beyond what was used).</summary>
    public sealed record ClockSideDto
    {
        /// <summary>The total time used.</summary>
        public long UsedMs { get; init; }

        /// <summary>The current move's time.</summary>
        public long StepMs { get; init; }
    }

    /// <summary>Every side's clock at the time of a packet (StartGame, GameUpdate, TimerSync).</summary>
    public sealed record ClocksDto
    {
        /// <summary>The clocks in turn order.</summary>
        public List<ClockSideDto> Sides { get; init; }

        /// <summary>The side to move: an index into <see cref="Sides"/> (0 is Player1).</summary>
        public int ToMove { get; init; }

        /// <summary>Which sides are away (disconnected, seat kept); same order as <see cref="Sides"/>.</summary>
        public List<bool> Away { get; init; }
    }

    /// <summary>What each player receives when a game starts (or, resumed, when it logs in again).</summary>
    public sealed record StartGameDto
    {
        /// <summary>The game's id.</summary>
        public string GameId { get; init; }

        /// <summary>The game kind.</summary>
        public string Kind { get; init; }

        /// <summary>The rule switches; null for the defaults.</summary>
        public Dictionary<string, bool> Rules { get; init; }

        /// <summary>The clocks' settings.</summary>
        public TimerSettingsDto Timer { get; init; }

        /// <summary>The players in turn order.</summary>
        public List<SeatDto> Players { get; init; }

        /// <summary>The receiver's player number (1 moves first).</summary>
        public int YourSide { get; init; }

        /// <summary>The position (the start, or the current one when resumed).</summary>
        public PositionDto Position { get; init; }

        /// <summary>The clocks now.</summary>
        public ClocksDto Clocks { get; init; }

        /// <summary>Whether this is a game already running, resumed after a new login (optional field).</summary>
        public bool Resumed { get; init; }

        /// <summary>The number of moves played so far, when resumed (optional field).</summary>
        public int? Ply { get; init; }

        /// <summary>The moves played so far, when resumed (optional field).</summary>
        public List<MoveEntryDto> Moves { get; init; }
    }

    /// <summary>GameAction's data: what a player wants to do.</summary>
    public sealed record GameActionDto
    {
        /// <summary>Action type: a move (<see cref="From"/>, <see cref="To"/>).</summary>
        public const string TypeMove = "move";

        /// <summary>Action type: resign.</summary>
        public const string TypeResign = "resign";

        /// <summary>The client's number for the action, echoed in the answer.</summary>
        public int Seq { get; init; }

        /// <summary><see cref="TypeMove"/> or <see cref="TypeResign"/>.</summary>
        public string Type { get; init; }

        /// <summary>The square moved from ([x, y]); null for a resign.</summary>
        public int[] From { get; init; }

        /// <summary>The square moved to ([x, y]); null for a resign.</summary>
        public int[] To { get; init; }
    }

    /// <summary>
    /// A validated move as every player sees it, or (<see cref="Rejected"/> set) one of the
    /// receiver's own actions the server refused (only its sender gets that).
    /// </summary>
    public sealed record GameUpdateDto
    {
        /// <summary>The action's seq (the mover's number for it).</summary>
        public int Seq { get; init; }

        /// <summary>Why the action was refused (e.g. NotYourTurn, IllegalMove, NoGame); null when it was applied.</summary>
        public string Rejected { get; init; }

        /// <summary>Whether this is a refusal.</summary>
        [JsonIgnore]
        public bool IsRejected => !string.IsNullOrEmpty(Rejected);

        /// <summary>The number of moves played, this one included.</summary>
        public int Ply { get; init; }

        /// <summary>The player who moved.</summary>
        public int Mover { get; init; }

        /// <summary>The move's record.</summary>
        public MoveRecordDto Record { get; init; }

        /// <summary>The position after the move.</summary>
        public PositionDto Position { get; init; }

        /// <summary>Whether the move gives check.</summary>
        public bool Check { get; init; }

        /// <summary>The clocks after the move.</summary>
        public ClocksDto Clocks { get; init; }
    }

    /// <summary>How a game ended.</summary>
    public sealed record EndGameDto
    {
        /// <summary>The winner's player number; 0 for a draw.</summary>
        public int Winner { get; init; }

        /// <summary>Why (e.g. Resign, TimeUp, Disconnect, Checkmate).</summary>
        public string Reason { get; init; }
    }
}
