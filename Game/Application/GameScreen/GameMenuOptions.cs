/* ----- ----- ----- ----- */
// GameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game screen's menu (the left column: navigation and files), in button order (the UI
    /// builds one button per entry). The game controls are in the sidebar (<see cref="GameControlOptions"/>).
    /// </summary>
    public static class GameMenuOptions
    {
        /// <summary>The menu's entries; choosing one calls <paramref name="onSelect"/> with its option.</summary>
        /// <param name="onSelect">Called with the chosen option.</param>
        /// <returns>The entries, in button order.</returns>
        public static List<MenuEntry<GameMenuOption>> Create(Action<GameMenuOption> onSelect)
        {
            return new List<MenuEntry<GameMenuOption>>
            {
                new MenuEntry<GameMenuOption>(GameMenuOption.SaveGame,     "儲存遊戲",   () => onSelect(GameMenuOption.SaveGame)),
                new MenuEntry<GameMenuOption>(GameMenuOption.LoadLayout,   "載入佈局",   () => onSelect(GameMenuOption.LoadLayout)),
                new MenuEntry<GameMenuOption>(GameMenuOption.ReturnToMain, "回到主畫面", () => onSelect(GameMenuOption.ReturnToMain)),
            };
        }
    }
}
