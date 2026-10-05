/* ----- ----- ----- ----- */
// UIGameMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    public static class UIGameMenuOptions
    {
        public static List<MenuEntry<UIGameMenuType>> Create(Action<UIGameMenuType> onSelect)
        {
            return new List<MenuEntry<UIGameMenuType>>
            {
                new MenuEntry<UIGameMenuType>(UIGameMenuType.Restart, "重新開始",      () => onSelect(UIGameMenuType.Restart)),
                new MenuEntry<UIGameMenuType>(UIGameMenuType.Undo, "撤銷上步",         () => onSelect(UIGameMenuType.Undo)),
                new MenuEntry<UIGameMenuType>(UIGameMenuType.SaveGame, "儲存遊戲",     () => onSelect(UIGameMenuType.SaveGame)),
                new MenuEntry<UIGameMenuType>(UIGameMenuType.LoadLayout, "載入佈局",   () => onSelect(UIGameMenuType.LoadLayout)),
                new MenuEntry<UIGameMenuType>(UIGameMenuType.Surrender, "放棄對局",    () => onSelect(UIGameMenuType.Surrender)),
                new MenuEntry<UIGameMenuType>(UIGameMenuType.ReturnToMain, "回到主畫面", () => onSelect(UIGameMenuType.ReturnToMain)),
            };
        }
    }
}
