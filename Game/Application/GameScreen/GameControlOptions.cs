/* ----- ----- ----- ----- */
// GameControlOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game controls, in button order (the sidebar builds one button per entry). Later
    /// controls (暫停, 求和, playback) are added here.
    /// </summary>
    public static class GameControlOptions
    {
        /// <summary>The controls; choosing one calls <paramref name="onSelect"/> with its option.</summary>
        /// <param name="onSelect">Called with the chosen option.</param>
        /// <returns>The entries, in button order.</returns>
        public static List<MenuEntry<GameControlOption>> Create(Action<GameControlOption> onSelect)
        {
            return new List<MenuEntry<GameControlOption>>
            {
                new MenuEntry<GameControlOption>(GameControlOption.Restart, "重新開始", () => onSelect(GameControlOption.Restart)),
                new MenuEntry<GameControlOption>(GameControlOption.Undo,    "撤銷上步", () => onSelect(GameControlOption.Undo)),
                new MenuEntry<GameControlOption>(GameControlOption.Resign,  "放棄對局", () => onSelect(GameControlOption.Resign)),
            };
        }
    }
}
