/* ----- ----- ----- ----- */
// UIMainMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public static class UIMainMenuOptions
    {
        public static List<ButtonEntry<UIMainMenuType>> Create(Action<UIMainMenuType> switchSubmenu)
        {
            return new List<ButtonEntry<UIMainMenuType>>
            {
                new ButtonEntry<UIMainMenuType>("開新一局",   UIMainMenuType.NewGame,          () => switchSubmenu(UIMainMenuType.NewGame)),
                new ButtonEntry<UIMainMenuType>("讀取存檔",   UIMainMenuType.LoadGame,         () => switchSubmenu(UIMainMenuType.LoadGame)),
                new ButtonEntry<UIMainMenuType>("殘局闖關",   UIMainMenuType.EndgameChallenge, () => switchSubmenu(UIMainMenuType.EndgameChallenge)),
                new ButtonEntry<UIMainMenuType>("規則設定",   UIMainMenuType.RuleSettings,     () => switchSubmenu(UIMainMenuType.RuleSettings)),
                new ButtonEntry<UIMainMenuType>("教學／幫助", UIMainMenuType.Help,             () => switchSubmenu(UIMainMenuType.Help)),
                new ButtonEntry<UIMainMenuType>("遊戲設定",   UIMainMenuType.Settings,         () => switchSubmenu(UIMainMenuType.Settings)),
                new ButtonEntry<UIMainMenuType>("離開遊戲",   UIMainMenuType.Exit,             () => switchSubmenu(UIMainMenuType.Exit)),
            };
        }
    }
}
