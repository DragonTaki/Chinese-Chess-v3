/* ----- ----- ----- ----- */
// NewGameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

namespace Chinese_Chess_v3.UI.Menu
{
    public static class NewGameMenuOptions
    {
        public static List<MenuEntry<NewGameMenuType>> Create(Action<NewGameMenuType> startNewGame)
        {
            return new List<MenuEntry<NewGameMenuType>>
            {
                new MenuEntry<NewGameMenuType>("傳統大盤", NewGameMenuType.Traditional,       () => startNewGame(NewGameMenuType.Traditional)),
                new MenuEntry<NewGameMenuType>("揭棋大盤", NewGameMenuType.FlipChess,         () => startNewGame(NewGameMenuType.FlipChess)),
                new MenuEntry<NewGameMenuType>("暗棋半盤", NewGameMenuType.DarkHalf,          () => startNewGame(NewGameMenuType.DarkHalf)),
                new MenuEntry<NewGameMenuType>("明棋半盤", NewGameMenuType.OpenHalf,          () => startNewGame(NewGameMenuType.OpenHalf)),
                new MenuEntry<NewGameMenuType>("三國半盤", NewGameMenuType.ThreeKingdomsHalf, () => startNewGame(NewGameMenuType.ThreeKingdomsHalf))
            };
        }
    }
}
