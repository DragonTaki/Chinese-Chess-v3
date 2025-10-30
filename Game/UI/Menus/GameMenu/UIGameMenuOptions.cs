/* ----- ----- ----- ----- */
// UIGameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    public static class UIGameMenuOptions
    {
        public static List<ButtonEntry<UIGameMenuType>> Create(Action<UIGameMenuType> onSelect)
        {
            return new List<ButtonEntry<UIGameMenuType>>
            {
                new ButtonEntry<UIGameMenuType>("重新開始",   UIGameMenuType.Restart,      () => onSelect(UIGameMenuType.Restart)),
                new ButtonEntry<UIGameMenuType>("撤銷上步",   UIGameMenuType.Undo,         () => onSelect(UIGameMenuType.Undo)),
                new ButtonEntry<UIGameMenuType>("儲存遊戲",   UIGameMenuType.SaveGame,     () => onSelect(UIGameMenuType.SaveGame)),
                new ButtonEntry<UIGameMenuType>("載入佈局",   UIGameMenuType.LoadLayout,   () => onSelect(UIGameMenuType.LoadLayout)),
                new ButtonEntry<UIGameMenuType>("放棄對局",   UIGameMenuType.Surrender,    () => onSelect(UIGameMenuType.Surrender)),
                new ButtonEntry<UIGameMenuType>("回到主畫面", UIGameMenuType.ReturnToMain, () => onSelect(UIGameMenuType.ReturnToMain)),
            };
        }
    }
}
