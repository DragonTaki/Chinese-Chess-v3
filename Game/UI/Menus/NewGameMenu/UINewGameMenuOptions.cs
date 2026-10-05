/* ----- ----- ----- ----- */
// UINewGameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    public static class UINewGameMenuOptions
    {
        public static List<MenuEntry<UINewGameMenuType>> Create(Action<UINewGameMenuType> startNewGame)
        {
            return new List<MenuEntry<UINewGameMenuType>>
            {
                new MenuEntry<UINewGameMenuType>(UINewGameMenuType.Traditional, "傳統大盤",       () => startNewGame(UINewGameMenuType.Traditional)),
                new MenuEntry<UINewGameMenuType>(UINewGameMenuType.FlipChess, "揭棋大盤",         () => startNewGame(UINewGameMenuType.FlipChess)),
                new MenuEntry<UINewGameMenuType>(UINewGameMenuType.DarkHalf, "暗棋半盤",          () => startNewGame(UINewGameMenuType.DarkHalf)),
                new MenuEntry<UINewGameMenuType>(UINewGameMenuType.OpenHalf, "明棋半盤",          () => startNewGame(UINewGameMenuType.OpenHalf)),
                new MenuEntry<UINewGameMenuType>(UINewGameMenuType.ThreeKingdomsHalf, "三國半盤", () => startNewGame(UINewGameMenuType.ThreeKingdomsHalf)),
            };
        }
    }
}
