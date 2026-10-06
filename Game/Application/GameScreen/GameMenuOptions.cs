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

using Engine.Localization;

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
                new MenuEntry<GameMenuOption>(GameMenuOption.SaveGame, Lang.Get("menu.game.save_game"), () => onSelect(GameMenuOption.SaveGame)),
                new MenuEntry<GameMenuOption>(GameMenuOption.LoadLayout, Lang.Get("menu.game.load_layout"), () => onSelect(GameMenuOption.LoadLayout)),
                new MenuEntry<GameMenuOption>(GameMenuOption.ReturnToMain, Lang.Get("menu.game.return_to_main"), () => onSelect(GameMenuOption.ReturnToMain)),
            };
        }
    }
}
