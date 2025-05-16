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

namespace Chinese_Chess_v3.UI.Menu
{
    public static class GameMenuOptions
    {
        public static List<MenuEntry<GameMenuType>> Create(Action<GameMenuType> onSelect)
        {
            return new List<MenuEntry<GameMenuType>>
            {
                new MenuEntry<GameMenuType>("重新開始",   GameMenuType.Restart,      () => onSelect(GameMenuType.Restart)),
                new MenuEntry<GameMenuType>("撤銷上步",   GameMenuType.Undo,         () => onSelect(GameMenuType.Undo)),
                new MenuEntry<GameMenuType>("儲存遊戲",   GameMenuType.SaveGame,     () => onSelect(GameMenuType.LoadLayout)),
                new MenuEntry<GameMenuType>("載入佈局",   GameMenuType.LoadLayout,   () => onSelect(GameMenuType.LoadLayout)),
                new MenuEntry<GameMenuType>("放棄對局",   GameMenuType.Surrender,    () => onSelect(GameMenuType.Surrender)),
                new MenuEntry<GameMenuType>("回到主選單", GameMenuType.ReturnToMain, () => onSelect(GameMenuType.ReturnToMain)),
            };
        }
    }
}
