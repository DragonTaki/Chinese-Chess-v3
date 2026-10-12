/* ----- ----- ----- ----- */
// OnlinePosition.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Pure mappings for server-authoritative Traditional games.</summary>
    public static class OnlinePosition
    {
        /// <summary>Converts a server position to independent Core piece snapshots.</summary>
        /// <param name="position">The server position.</param>
        /// <returns>The pieces for StartRemote.</returns>
        public static List<PieceInfo> PiecesFrom(PositionDto position)
        {
            ArgumentNullException.ThrowIfNull(position);
            ArgumentNullException.ThrowIfNull(position.Pieces);
            return position.Pieces.Select(p => new PieceInfo(
                Parse<PieceType>(p.Type), p.X, p.Y, Parse<PieceColor>(p.Color),
                SideFrom(p.Side), p.FaceUp)).ToList();
        }

        /// <summary>Compares all visible piece state and the side to move.</summary>
        /// <param name="g">The current game.</param>
        /// <param name="p">The server position.</param>
        /// <returns>Whether the positions agree, independently of piece order.</returns>
        public static bool Matches(GameManager g, PositionDto p)
        {
            ArgumentNullException.ThrowIfNull(g);
            var expected = PiecesFrom(p);
            var current = BoardPieces(g);
            return g.CurrentTurn == SideFrom(p.ToMove) && current.Count == expected.Count &&
                expected.All(e => current.Any(c => c.X == e.X && c.Y == e.Y && SamePiece(c, e))) &&
                expected.Select(e => (e.X, e.Y)).Distinct().Count() == expected.Count;
        }

        /// <summary>Finds a Traditional move from exactly two changed squares; use only when From/To are absent.</summary>
        /// <param name="g">The game before the move.</param>
        /// <param name="p">The server position after the move.</param>
        /// <param name="from">The source square, or default on failure.</param>
        /// <param name="to">The destination square, or default on failure.</param>
        /// <returns>Whether the difference describes one move by the current player.</returns>
        public static bool FindMove(GameManager g, PositionDto p, out GridPoint from, out GridPoint to)
        {
            ArgumentNullException.ThrowIfNull(g);
            from = to = default;
            var nextPieces = PiecesFrom(p);
            if (g.Board.Type != BoardType.Full || g.Board.IsJieqi ||
                SideFrom(p.ToMove) != (g.CurrentTurn == PlayerSide.Player1 ? PlayerSide.Player2 : PlayerSide.Player1) ||
                nextPieces.Any(e => e.X < 0 || e.X >= g.Board.Columns || e.Y < 0 || e.Y >= g.Board.Rows) ||
                nextPieces.Select(e => (e.X, e.Y)).Distinct().Count() != nextPieces.Count)
                return false;

            var before = BoardPieces(g).ToDictionary(e => new GridPoint(e.X, e.Y));
            var after = nextPieces.ToDictionary(e => new GridPoint(e.X, e.Y));
            var changed = before.Keys.Union(after.Keys).Where(square =>
                !before.TryGetValue(square, out var oldPiece) ||
                !after.TryGetValue(square, out var newPiece) || !SamePiece(oldPiece, newPiece)).ToList();
            if (changed.Count != 2)
                return false;
            foreach (var source in changed)
            {
                var destination = changed.First(s => s != source);
                if (before.TryGetValue(source, out var mover) && mover.Side == g.CurrentTurn &&
                    !after.ContainsKey(source) && after.TryGetValue(destination, out var moved) && SamePiece(mover, moved))
                {
                    from = source;
                    to = destination;
                    return true;
                }
            }
            return false;
        }

        /// <summary>Maps elapsed clocks without adjusting for increments or disconnected seats.</summary>
        /// <param name="clocks">The server clock payload; Away does not pause a clock.</param>
        /// <param name="toMove">The active player, converted from a zero-based index.</param>
        /// <returns>Step and total elapsed times in player order.</returns>
        public static IReadOnlyList<ClockState> ClocksFrom(ClocksDto clocks, out PlayerSide toMove)
        {
            ArgumentNullException.ThrowIfNull(clocks);
            ArgumentNullException.ThrowIfNull(clocks.Sides);
            toMove = SideFrom(checked(clocks.ToMove + 1));
            return clocks.Sides.Select(s => new ClockState(TimeSpan.FromMilliseconds(s.StepMs),
                TimeSpan.FromMilliseconds(s.UsedMs))).ToArray();
        }

        /// <summary>Maps the winner and known end reasons, retaining unknown reasons as Other.</summary>
        /// <param name="end">The server result.</param>
        /// <returns>The winner (None for a draw) and end reason.</returns>
        public static (PlayerSide Winner, GameOverReason Reason) EndFrom(EndGameDto end)
        {
            ArgumentNullException.ThrowIfNull(end);
            return (SideFrom(end.Winner), end.Reason switch
            {
                "TimeUp" => GameOverReason.TimeUp,
                "Disconnect" => GameOverReason.Disconnect,
                "Resign" => GameOverReason.Resign,
                "Checkmate" => GameOverReason.Checkmate,
                "Stalemate" => GameOverReason.Stalemate,
                "NoPiecesLeft" => GameOverReason.NoPiecesLeft,
                _ => GameOverReason.Other,
            });
        }

        private static PlayerSide SideFrom(int side) => side switch
        {
            0 => PlayerSide.None,
            1 => PlayerSide.Player1,
            2 => PlayerSide.Player2,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown Traditional player number"),
        };

        private static T Parse<T>(string name) where T : struct, Enum =>
            Enum.TryParse(name, ignoreCase: false, out T value) && Enum.IsDefined(value)
                ? value : throw new ArgumentException($"Unknown {typeof(T).Name}: {name}", nameof(name));

        private static List<PieceInfo> BoardPieces(GameManager g) =>
            g.Board.GetAllPieces().Where(p => !p.CurrentInfo.IsDead).Select(p => p.CurrentInfo).ToList();

        private static bool SamePiece(PieceInfo a, PieceInfo b) =>
            a.Type == b.Type && a.Side == b.Side && a.Color == b.Color && a.IsFaceUp == b.IsFaceUp;
    }
}
