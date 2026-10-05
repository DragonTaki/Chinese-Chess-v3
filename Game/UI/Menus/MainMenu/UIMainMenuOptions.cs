/* ----- ----- ----- ----- */
// UIMainMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;
using Chinese_Chess_v3.Game.UI.Constants;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public static class UIMainMenuOptions
    {
        public static List<MenuEntry<UIMainMenuType>> Create(Action<UIMainMenuType> switchSubmenu)
        {
            return new List<MenuEntry<UIMainMenuType>>
            {
                new MenuEntry<UIMainMenuType>(UIMainMenuType.NewGame, "開新一局",          () => switchSubmenu(UIMainMenuType.NewGame)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.LoadGame, "讀取存檔",         () => switchSubmenu(UIMainMenuType.LoadGame)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.EndgameChallenge, "殘局闖關", () => switchSubmenu(UIMainMenuType.EndgameChallenge)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.OpeningPractice, "開局練習",  () => switchSubmenu(UIMainMenuType.OpeningPractice)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.Multiplayer, "多人連線",      () => switchSubmenu(UIMainMenuType.Multiplayer)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.Help, "教學／幫助",             () => switchSubmenu(UIMainMenuType.Help)),
                // The rules of local games only (a network game does not use them), right above 遊戲設定.
                new MenuEntry<UIMainMenuType>(UIMainMenuType.RuleSettings, GameMenuTexts.LocalRuleSettings, () => switchSubmenu(UIMainMenuType.RuleSettings)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.Settings, GameMenuTexts.GameSettings,     () => switchSubmenu(UIMainMenuType.Settings)),
                new MenuEntry<UIMainMenuType>(UIMainMenuType.Exit, "離開遊戲",             () => switchSubmenu(UIMainMenuType.Exit)),
            };
        }
    }
}
