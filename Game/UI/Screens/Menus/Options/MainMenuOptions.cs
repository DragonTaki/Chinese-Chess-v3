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

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Options
{
    public static class MainMenuOptions
    {
        public static List<ButtonEntry<MainMenuType>> Create(Action<MainMenuType> switchSubmenu)
        {
            return new List<ButtonEntry<MainMenuType>>
            {
                new ButtonEntry<MainMenuType>("開新一局",   MainMenuType.NewGame,          () => switchSubmenu(MainMenuType.NewGame)),
                new ButtonEntry<MainMenuType>("讀取存檔",   MainMenuType.LoadGame,         () => switchSubmenu(MainMenuType.LoadGame)),
                new ButtonEntry<MainMenuType>("殘局闖關",   MainMenuType.EndgameChallenge, () => switchSubmenu(MainMenuType.EndgameChallenge)),
                new ButtonEntry<MainMenuType>("規則設定",   MainMenuType.RuleSettings,     () => switchSubmenu(MainMenuType.RuleSettings)),
                new ButtonEntry<MainMenuType>("教學／幫助", MainMenuType.Help,             () => switchSubmenu(MainMenuType.Help)),
                new ButtonEntry<MainMenuType>("遊戲設定",   MainMenuType.Settings,         () => switchSubmenu(MainMenuType.Settings)),
                new ButtonEntry<MainMenuType>("離開遊戲",   MainMenuType.Exit,             () => switchSubmenu(MainMenuType.Exit)),
            };
        }
    }
}
