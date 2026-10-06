/* ----- ----- ----- ----- */
// JieqiDealer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core.Pieces;

using Engine.Logging;
using Engine.Randomization;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>
    /// Deals the start layouts of local 揭棋 games (DARK-CHESS-RULES §1.3): both Generals face up on
    /// their squares; each side's other 15 pieces face down, shuffled over that side's other 15
    /// starting squares. Dealing is making a game, not a rule (an online game's server deals).
    /// </summary>
    public static class JieqiDealer
    {
        /// <summary>Random table size: at least the 2 × 14 draws of two Fisher–Yates passes over 15 squares.</summary>
        private const int ShuffleTableSize = 64;

        /// <summary>
        /// A dealer for <c>GameManager.StartJieqi</c>: each call deals a new layout. The first deal
        /// uses <paramref name="firstSeed"/> when given (tests, replaying a layout from the log);
        /// every other deal is seeded from the clock.
        /// </summary>
        public static Func<List<PieceInfo>> Create(int? firstSeed = null)
        {
            int? pending = firstSeed;
            return () =>
            {
                int seed = pending ?? Environment.TickCount;
                pending = null;
                AppLogger.Log($"(Jieqi) Dealing a 揭棋 layout, seed: {seed}", LogLevel.DEBUG);
                return Deal(new RandomTable(ShuffleTableSize, seed));
            };
        }

        /// <summary>A fresh 揭棋 start position dealt with <paramref name="random"/>.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="random"/> is null.</exception>
        public static List<PieceInfo> Deal(IRandomProvider random)
        {
            ArgumentNullException.ThrowIfNull(random);

            var pieces = new List<PieceInfo>();
            foreach (var color in new[] { PieceColor.Red, PieceColor.Black })
            {
                var start = PieceConstants.InitialClassicPieces.Where(p => p.Color == color).ToList();
                var general = start.Single(p => p.Type == PieceType.General);
                pieces.Add(new PieceInfo(PieceType.General, general.X, general.Y, color, general.Side, isFaceUp: true));

                var others = start.Where(p => p.Type != PieceType.General).ToList();
                var types = others.Select(p => p.Type).ToList();
                // Fisher–Yates over the types; the squares stay in place.
                for (int i = types.Count - 1; i > 0; i--)
                {
                    int j = random.NextInt(i + 1);
                    (types[i], types[j]) = (types[j], types[i]);
                }
                for (int i = 0; i < others.Count; i++)
                    pieces.Add(new PieceInfo(types[i], others[i].X, others[i].Y, color, others[i].Side, isFaceUp: false));
            }
            return pieces;
        }
    }
}
