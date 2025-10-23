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

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Options
{
    public static class NewGameMenuOptions
    {
        public static List<ButtonEntry<NewGameMenuType>> Create(Action<NewGameMenuType> startNewGame)
        {
            return new List<ButtonEntry<NewGameMenuType>>
            {
                new ButtonEntry<NewGameMenuType>("傳統大盤", NewGameMenuType.Traditional,       () => startNewGame(NewGameMenuType.Traditional)),
                new ButtonEntry<NewGameMenuType>("揭棋大盤", NewGameMenuType.FlipChess,         () => startNewGame(NewGameMenuType.FlipChess)),
                new ButtonEntry<NewGameMenuType>("暗棋半盤", NewGameMenuType.DarkHalf,          () => startNewGame(NewGameMenuType.DarkHalf)),
                new ButtonEntry<NewGameMenuType>("明棋半盤", NewGameMenuType.OpenHalf,          () => startNewGame(NewGameMenuType.OpenHalf)),
                new ButtonEntry<NewGameMenuType>("三國半盤", NewGameMenuType.ThreeKingdomsHalf, () => startNewGame(NewGameMenuType.ThreeKingdomsHalf)),
            };
        }
    }
}
