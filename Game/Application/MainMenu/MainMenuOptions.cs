/* ----- ----- ----- ----- */
// MainMenuOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2026/10/05
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;
using Chinese_Chess_v3.Game.Application.Texts;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>The main menu, in button order (the UI builds one button per entry).</summary>
    public static class MainMenuOptions
    {
        /// <summary>The menu's entries; choosing one calls <paramref name="onSelect"/> with its option.</summary>
        /// <param name="onSelect">Called with the chosen option (<see cref="MainMenuPresenter.Select"/>).</param>
        /// <returns>The entries, in button order.</returns>
        public static List<MenuEntry<MainMenuOption>> Create(Action<MainMenuOption> onSelect)
        {
            return new List<MenuEntry<MainMenuOption>>
            {
                new MenuEntry<MainMenuOption>(MainMenuOption.NewGame,          "開新一局",   () => onSelect(MainMenuOption.NewGame)),
                new MenuEntry<MainMenuOption>(MainMenuOption.LoadGame,         "讀取存檔",   () => onSelect(MainMenuOption.LoadGame)),
                new MenuEntry<MainMenuOption>(MainMenuOption.EndgameChallenge, "殘局闖關",   () => onSelect(MainMenuOption.EndgameChallenge)),
                new MenuEntry<MainMenuOption>(MainMenuOption.OpeningPractice,  "開局練習",   () => onSelect(MainMenuOption.OpeningPractice)),
                new MenuEntry<MainMenuOption>(MainMenuOption.Multiplayer,      "多人連線",   () => onSelect(MainMenuOption.Multiplayer)),
                new MenuEntry<MainMenuOption>(MainMenuOption.Help,             "教學／幫助", () => onSelect(MainMenuOption.Help)),
                // The rules of local games only (a network game does not use them), right above 遊戲設定.
                new MenuEntry<MainMenuOption>(MainMenuOption.RuleSettings,     MenuTexts.LocalRuleSettings, () => onSelect(MainMenuOption.RuleSettings)),
                new MenuEntry<MainMenuOption>(MainMenuOption.Settings,         MenuTexts.GameSettings,      () => onSelect(MainMenuOption.Settings)),
                new MenuEntry<MainMenuOption>(MainMenuOption.Exit,             "離開遊戲",   () => onSelect(MainMenuOption.Exit)),
            };
        }
    }
}
