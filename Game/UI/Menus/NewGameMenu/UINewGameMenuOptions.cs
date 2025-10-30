/* ----- ----- ----- ----- */
// UINewGameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    public static class UINewGameMenuOptions
    {
        public static List<ButtonEntry<UINewGameMenuType>> Create(Action<UINewGameMenuType> startNewGame)
        {
            return new List<ButtonEntry<UINewGameMenuType>>
            {
                new ButtonEntry<UINewGameMenuType>("傳統大盤", UINewGameMenuType.Traditional,       () => startNewGame(UINewGameMenuType.Traditional)),
                new ButtonEntry<UINewGameMenuType>("揭棋大盤", UINewGameMenuType.FlipChess,         () => startNewGame(UINewGameMenuType.FlipChess)),
                new ButtonEntry<UINewGameMenuType>("暗棋半盤", UINewGameMenuType.DarkHalf,          () => startNewGame(UINewGameMenuType.DarkHalf)),
                new ButtonEntry<UINewGameMenuType>("明棋半盤", UINewGameMenuType.OpenHalf,          () => startNewGame(UINewGameMenuType.OpenHalf)),
                new ButtonEntry<UINewGameMenuType>("三國半盤", UINewGameMenuType.ThreeKingdomsHalf, () => startNewGame(UINewGameMenuType.ThreeKingdomsHalf)),
            };
        }
    }
}
