/* ----- ----- ----- ----- */
// AiOpeningBook.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>
    /// An opponent's opening repertoire, selected by its own opening name and player side.
    /// Matches the complete played history; a deviation or an exhausted line returns no move,
    /// leaving the caller to search with its chess engine. Never changes the game.
    /// </summary>
    public sealed class AiOpeningBook
    {
        private readonly OpeningLine[] _lines;

        /// <summary>Snapshots standard-start opening lines in the supplied preference order.</summary>
        /// <param name="lines">Loaded opening lines; special start positions are excluded.</param>
        public AiOpeningBook(IEnumerable<OpeningLine> lines)
        {
            ArgumentNullException.ThrowIfNull(lines);
            _lines = lines.Where(line => line != null
                    && line.PlayerSide is PlayerSide.Player1 or PlayerSide.Player2
                    && !string.IsNullOrWhiteSpace(line.Title)
                    && line.Moves != null && line.Moves.Count > 0 && IsStandardStart(line.Fen))
                .Select(line => line with { Moves = line.Moves.ToArray() }).ToArray();
        }

        /// <summary>The named opening styles available for the opponent's chosen side.</summary>
        /// <param name="computerSide">The computer's player number, not its colour.</param>
        /// <returns>Distinct name names, sorted ordinally.</returns>
        public IReadOnlyList<string> StylesFor(PlayerSide computerSide)
        {
            ValidateSide(computerSide);
            return _lines.Where(line => line.PlayerSide == computerSide).Select(line => line.Title)
                .Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray();
        }

        /// <summary>Finds a legal next repertoire move without preplaying either player's moves.</summary>
        /// <param name="game">The current local Traditional game.</param>
        /// <param name="computerSide">The side whose repertoire is selected.</param>
        /// <param name="style">The name of the opponent's opening; empty selects no repertoire.</param>
        /// <param name="move">The next move when found; otherwise its default value.</param>
        /// <returns>False off the repertoire or outside an active turn of the selected side.</returns>
        public bool TryGetMove(GameManager game, PlayerSide computerSide, string style, out IccsMove move)
        {
            ArgumentNullException.ThrowIfNull(game);
            ValidateSide(computerSide);
            move = default;
            if (string.IsNullOrWhiteSpace(style) || game.IsRemote || game.IsReplaying || game.IsPaused
                || game.IsGameOver || game.CurrentTurn != computerSide || game.Board.Type != BoardType.Full
                || game.Board.IsJieqi || !IsStandardStart(game.InitialFen))
                return false;

            int ply = game.Moves.Count;
            foreach (var line in _lines)
            {
                if (line.PlayerSide != computerSide || !string.Equals(line.Title, style, StringComparison.Ordinal)
                    || line.Moves.Count <= ply)
                    continue;
                bool matches = true;
                for (int i = 0; i < ply; i++)
                {
                    var played = game.Moves[i];
                    if (line.Moves[i] != new IccsMove(played.FromX, played.FromY, played.ToX, played.ToY))
                    {
                        matches = false;
                        break;
                    }
                }
                if (!matches)
                    continue;
                var next = line.Moves[ply];
                if (!ActionResolver.IsLegalMove(game.Board, computerSide, next.FromX, next.FromY, next.ToX, next.ToY))
                    continue;
                move = next;
                return true;
            }
            return false;
        }

        private static void ValidateSide(PlayerSide side)
        {
            if (side is not (PlayerSide.Player1 or PlayerSide.Player2))
                throw new ArgumentOutOfRangeException(nameof(side), side, "An opening opponent must be Player1 or Player2");
        }

        private static bool IsStandardStart(string fen)
        {
            try
            {
                var parsed = XiangqiFen.Parse(fen);
                return string.Equals(XiangqiFen.Format(parsed.Pieces, parsed.FirstColor),
                    XiangqiFen.StartPosition, StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
