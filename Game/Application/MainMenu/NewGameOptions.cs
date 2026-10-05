/* ----- ----- ----- ----- */
// NewGameOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;
using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>The new-game menu (開新一局): one entry per <see cref="GameKind"/>, in button order.</summary>
    public static class NewGameOptions
    {
        /// <summary>The menu's entries; choosing one calls <paramref name="startNewGame"/> with its game kind.</summary>
        /// <param name="startNewGame">Called with the chosen game kind.</param>
        /// <returns>The entries, in button order.</returns>
        public static List<MenuEntry<GameKind>> Create(Action<GameKind> startNewGame)
        {
            return new List<MenuEntry<GameKind>>
            {
                new MenuEntry<GameKind>(GameKind.Traditional,   "傳統大盤", () => startNewGame(GameKind.Traditional)),
                new MenuEntry<GameKind>(GameKind.Flip,          "揭棋大盤", () => startNewGame(GameKind.Flip)),
                new MenuEntry<GameKind>(GameKind.DarkHalf,      "暗棋半盤", () => startNewGame(GameKind.DarkHalf)),
                new MenuEntry<GameKind>(GameKind.OpenHalf,      "明棋半盤", () => startNewGame(GameKind.OpenHalf)),
                new MenuEntry<GameKind>(GameKind.ThreeKingdoms, "三國半盤", () => startNewGame(GameKind.ThreeKingdoms)),
            };
        }

        /// <summary>The button text of <paramref name="kind"/> (its entry's label).</summary>
        /// <param name="kind">The game kind.</param>
        /// <returns>Its label, or the enum name for a kind without an entry.</returns>
        public static string LabelOf(GameKind kind) =>
            Create(_ => { }).Find(e => e.Id == kind)?.Label ?? kind.ToString();
    }
}
