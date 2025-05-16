/* ----- ----- ----- ----- */
// MainMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

namespace Chinese_Chess_v3.UI.Menu
{
    public static class MainMenuOptions
    {
        public static List<MenuEntry<MainMenuType>> Create(Action<MainMenuType> switchSubmenu, Action exitApp)
        {
            return new List<MenuEntry<MainMenuType>>
            {
                new MenuEntry<MainMenuType>("開新一局",   MainMenuType.NewGame,      () => switchSubmenu(MainMenuType.NewGame)),
                new MenuEntry<MainMenuType>("讀取存檔",   MainMenuType.LoadGame,     () => switchSubmenu(MainMenuType.LoadGame)),
                new MenuEntry<MainMenuType>("規則設定",   MainMenuType.RuleSettings, () => switchSubmenu(MainMenuType.RuleSettings)),
                new MenuEntry<MainMenuType>("教學／幫助", MainMenuType.Help,         () => switchSubmenu(MainMenuType.Help)),
                new MenuEntry<MainMenuType>("遊戲設定",   MainMenuType.Settings,     () => switchSubmenu(MainMenuType.Settings)),
                new MenuEntry<MainMenuType>("離開遊戲",   MainMenuType.Exit,         exitApp)
            };
        }
    }
}
