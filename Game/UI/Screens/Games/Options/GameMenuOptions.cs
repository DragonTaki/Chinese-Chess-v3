/* ----- ----- ----- ----- */
// GameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Options
{
    public static class GameMenuOptions
    {
        public static List<ButtonEntry<GameMenuType>> Create(Action<GameMenuType> onSelect)
        {
            return new List<ButtonEntry<GameMenuType>>
            {
                new ButtonEntry<GameMenuType>("重新開始",   GameMenuType.Restart,      () => onSelect(GameMenuType.Restart)),
                new ButtonEntry<GameMenuType>("撤銷上步",   GameMenuType.Undo,         () => onSelect(GameMenuType.Undo)),
                new ButtonEntry<GameMenuType>("儲存遊戲",   GameMenuType.SaveGame,     () => onSelect(GameMenuType.SaveGame)),
                new ButtonEntry<GameMenuType>("載入佈局",   GameMenuType.LoadLayout,   () => onSelect(GameMenuType.LoadLayout)),
                new ButtonEntry<GameMenuType>("放棄對局",   GameMenuType.Surrender,    () => onSelect(GameMenuType.Surrender)),
                new ButtonEntry<GameMenuType>("回到主畫面", GameMenuType.ReturnToMain, () => onSelect(GameMenuType.ReturnToMain)),
            };
        }
    }
}
