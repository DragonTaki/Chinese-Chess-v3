/* ----- ----- ----- ----- */
// ThreeKingdomsDealer.cs
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
    /// Deals the start layouts of local 三國 games (DARK-CHESS-RULES §1.2): all 32 pieces face down,
    /// owned by nobody, shuffled over the four 4×2 corner blocks of the HalfCross board (the middle
    /// column x = 4 and row y = 2 start empty). Dealing is making a game, not a rule (an online
    /// game's server deals).
    /// </summary>
    public static class ThreeKingdomsDealer
    {
        /// <summary>Random table size: at least the 31 draws of one Fisher–Yates pass over 32 squares.</summary>
        private const int ShuffleTableSize = 64;

        /// <summary>
        /// A dealer for <c>GameManager.StartThreeKingdoms</c>: each call deals a new layout. The first
        /// deal uses <paramref name="firstSeed"/> when given (tests, replaying a layout from the log);
        /// every other deal is seeded from the clock.
        /// </summary>
        public static Func<List<PieceInfo>> Create(int? firstSeed = null)
        {
            int? pending = firstSeed;
            return () =>
            {
                int seed = pending ?? Environment.TickCount;
                pending = null;
                AppLogger.Log($"(ThreeKingdoms) Dealing a 三國 layout, seed: {seed}", LogLevel.DEBUG);
                return Deal(new RandomTable(ShuffleTableSize, seed));
            };
        }

        /// <summary>The 32 squares of the four corner blocks, row by row.</summary>
        public static List<(int x, int y)> StartSquares()
        {
            var squares = new List<(int x, int y)>();
            for (int y = 0; y < BoardConstants.HalfCross.Rows; y++)
            {
                for (int x = 0; x < BoardConstants.HalfCross.Columns; x++)
                {
                    if (x != 4 && y != 2)
                        squares.Add((x, y));
                }
            }
            return squares;
        }

        /// <summary>A fresh 三國 start position dealt with <paramref name="random"/>: 32 face-down pieces, one per corner-block square.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is null.</exception>
        public static List<PieceInfo> Deal(IRandomProvider random)
        {
            ArgumentNullException.ThrowIfNull(random);

            var squares = StartSquares();
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
                        pieces.Add(new PieceInfo(type, x, y, color, PlayerSide.None, isFaceUp: false));
                    }
                }
            }
            return pieces;
        }
    }
}
