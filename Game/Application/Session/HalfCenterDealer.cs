/* ----- ----- ----- ----- */
// HalfCenterDealer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

using Engine.Logging;
using Engine.Randomization;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>
    /// Deals the HalfCenter (台灣暗棋半盤, 8×4) start layouts of local games. Dealing is making a
    /// game, not a rule, so it lives here and not in the rules layer (author 2026-10-06): an
    /// online game's layout is dealt by the server instead.
    /// </summary>
    public static class HalfCenterDealer
    {
        /// <summary>
        /// Size of the <see cref="RandomTable"/> a shuffle draws from: at least the 31 draws one
        /// Fisher–Yates pass over the 32 squares takes, so no value repeats.
        /// </summary>
        private const int ShuffleTableSize = 64;

        /// <summary>
        /// A dealer for <c>GameManager.StartHalfCenter</c>: each call deals a new layout (face down
        /// when asked). The first deal uses <paramref name="firstSeed"/> when given (tests, replaying
        /// a layout from the log); every other deal - and the first one without a seed - is seeded
        /// from the clock, so every new game and every restart is a different layout.
        /// <see cref="GlobalRandom"/> is not used: its fixed seed would deal the same layout on every launch.
        /// </summary>
        public static Func<bool, List<PieceInfo>> Create(int? firstSeed = null)
        {
            int? pending = firstSeed;
            return faceDown =>
            {
                int seed = pending ?? Environment.TickCount;
                pending = null;
                AppLogger.Log($"(DarkChess) Dealing a HalfCenter layout, face down: {faceDown}, seed: {seed}", LogLevel.DEBUG);
                return Deal(new RandomTable(ShuffleTableSize, seed), faceDown);
            };
        }

        /// <summary>
        /// A fresh HalfCenter start position: both colours' <see cref="PieceConstants.HalfCenterPieceSet"/>
        /// (32 pieces) shuffled over all 32 squares (Fisher–Yates with <paramref name="random"/>).
        /// </summary>
        /// <param name="random">The random source.</param>
        /// <param name="faceDown">
        /// true (暗棋, <c>Rules.IsHiddenChess</c>): every piece face down and owned by nobody
        /// (<c>PlayerSide.None</c>) — the first flip decides the factions. false (明棋半盤): every
        /// piece face up and still owned by nobody — the first move decides the factions (the
        /// mover gets the moved piece's colour, see <c>GameManager.ColorOf</c>).
        /// </param>
        /// <returns>32 pieces, one per square.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is null.</exception>
        public static List<PieceInfo> Deal(IRandomProvider random, bool faceDown = true)
        {
            ArgumentNullException.ThrowIfNull(random);

            var squares = new List<(int x, int y)>();
            for (int y = 0; y < BoardConstants.HalfCenter.Rows; y++)
            {
                for (int x = 0; x < BoardConstants.HalfCenter.Columns; x++)
                    squares.Add((x, y));
            }

            // Fisher–Yates: every square order equally likely (as far as the source is).
            for (int i = squares.Count - 1; i > 0; i--)
            {
                int j = random.NextInt(i + 1);
                (squares[i], squares[j]) = (squares[j], squares[i]);
            }

            var pieces = new List<PieceInfo>(squares.Count);
            int next = 0;
            foreach (var color in new[] { PieceColor.Red, PieceColor.Black })
            {
                foreach (var (type, count) in PieceConstants.HalfCenterPieceSet)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var (x, y) = squares[next++];
                        pieces.Add(new PieceInfo(type, x, y, color, PlayerSide.None, isFaceUp: !faceDown));
                    }
                }
            }
            return pieces;
        }
    }
}
