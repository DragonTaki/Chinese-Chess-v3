/* ----- ----- ----- ----- */
// NewGameOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/12
// Version: v1.4
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>The shared game-kind submenu: one entry per <see cref="GameKind"/>, grouped by board in <see cref="BoardGroups"/>.</summary>
    public static class NewGameOptions
    {
        /// <summary>The board groups; choosing a button calls <paramref name="startNewGame"/> with its game kind.</summary>
        /// <param name="startNewGame">Called with the chosen game kind.</param>
        /// <returns>The groups, in display order.</returns>
        public static IReadOnlyList<BoardGroup> Create(Action<GameKind> startNewGame) => BoardGroups.Create(startNewGame);

        /// <summary>The button text of <paramref name="kind"/> (its entry's label).</summary>
        /// <param name="kind">The game kind.</param>
        /// <returns>Its label, or the enum name for a kind without an entry.</returns>
        public static string LabelOf(GameKind kind) =>
            Create(_ => { }).SelectMany(g => g.Entries).FirstOrDefault(e => e.Id == kind)?.Label ?? kind.ToString();
    }
}
