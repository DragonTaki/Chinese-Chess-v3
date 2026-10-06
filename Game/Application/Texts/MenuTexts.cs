/* ----- ----- ----- ----- */
// MenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Globalization;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts of the main menu and its submenus (the main menu's presenter and option lists, the
    /// saved-game lists): entries named elsewhere too, the confirm dialog messages and the
    /// saved-game lists' texts, finished here so the display only shows them.
    /// </summary>
    public static class MenuTexts
    {
        // ----- Main menu -----

        /// <summary>Main menu entry and title of the rules screen (local games only; a network game does not use these rules).</summary>
        public static string LocalRuleSettings => Lang.Get("menu.local_rule_settings");

        /// <summary>Main menu entry and title of the general settings screen.</summary>
        public static string GameSettings => Lang.Get("menu.game_settings");

        /// <summary>離開遊戲: asks before the application closes (yes = exit).</summary>
        public static string ConfirmExit => Lang.Get("menu.confirm_exit");

        // ----- New-game menu -----

        /// <summary>A new-game mode whose game cannot be started yet; <paramref name="mode"/> = its button text.</summary>
        public static string NewGameModeUnavailable(string mode) => Lang.Get("menu.new_game_mode_unavailable", mode);

        /// <summary>三國半盤 whose 自訂分隊 leaves a team without pieces.</summary>
        public static string TeamSplitInvalid => Lang.Get("menu.team_split_invalid");

        // ----- Saved-game lists (載入 on the game screen, 讀取存檔 on the main menu) -----

        /// <summary>The game screen's list when there is no saved game; <paramref name="folder"/> = the saves folder.</summary>
        public static string NoSavedGames(string folder) =>
            Lang.Get("menu.no_saved_games", folder);

        /// <summary>The main menu's list: the disabled row shown when there is no save.</summary>
        public static string NoSavedGamesRow => Lang.Get("menu.no_saved_games_row");

        /// <summary>Ends a name cut to fit one line (the display decides how much fits).</summary>
        public static string Ellipsis => Lang.Get("menu.ellipsis");

        /// <summary>
        /// Both lists' delete-mode toggle: 刪除存檔 while off (clicking it turns delete mode on: a
        /// clicked save then asks to be deleted instead of being loaded), 結束刪除 while on.
        /// </summary>
        public static string DeleteModeToggle(bool on) => Lang.Get(on ? "menu.delete_mode.on" : "menu.delete_mode.off");

        /// <summary>The main menu's list: a save's one-line button, its (already fitted) name, date and time.</summary>
        public static string SavedGameRow(string name, DateTime time) =>
            Lang.Get("menu.saved_game_row", name, SavedGameDate(time), SavedGameTime(time));

        /// <summary>The game screen's list: a save's button, its (already fitted) name, date and time on three lines.</summary>
        public static string SavedGameButton(string name, DateTime time) => Lang.Get("menu.saved_game_button", name, SavedGameDate(time), SavedGameTime(time));

        /// <summary>A save's date (from its file name).</summary>
        private static string SavedGameDate(DateTime time) => time.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        /// <summary>A save's time (from its file name).</summary>
        private static string SavedGameTime(DateTime time) => time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
