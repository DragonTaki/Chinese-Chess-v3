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
        public const string LocalRuleSettings = "單機規則設定";

        /// <summary>Main menu entry and title of the general settings screen.</summary>
        public const string GameSettings = "遊戲設定";

        /// <summary>離開遊戲: asks before the application closes (yes = exit).</summary>
        public const string ConfirmExit = "確認要離開遊戲嗎？";

        // ----- New-game menu -----

        /// <summary>A new-game mode whose game cannot be started yet; <paramref name="mode"/> = its button text.</summary>
        public static string NewGameModeUnavailable(string mode) => $"「{mode}」尚未完成，目前無法開始。";

        /// <summary>三國半盤 whose 自訂分隊 leaves a team without pieces.</summary>
        public const string TeamSplitInvalid = "三國半盤的自訂分隊有隊伍沒有任何棋子，目前無法開始；請在規則設定讓每隊至少一顆子。";

        // ----- Saved-game lists (載入 on the game screen, 讀取存檔 on the main menu) -----

        /// <summary>The game screen's list when there is no saved game; <paramref name="folder"/> = the saves folder.</summary>
        public static string NoSavedGames(string folder) =>
            string.Format("找不到存檔。\n用左邊的「儲存遊戲」存檔後會出現在這裡，存檔位置：\n{0}", folder);

        /// <summary>The main menu's list: the disabled row shown when there is no save.</summary>
        public const string NoSavedGamesRow = "沒有存檔";

        /// <summary>Ends a name cut to fit one line (the display decides how much fits).</summary>
        public const string Ellipsis = "…";

        /// <summary>
        /// Both lists' delete-mode toggle: 刪除存檔 while off (clicking it turns delete mode on: a
        /// clicked save then asks to be deleted instead of being loaded), 結束刪除 while on.
        /// </summary>
        public static string DeleteModeToggle(bool on) => on ? "結束刪除" : "刪除存檔";

        /// <summary>The main menu's list: a save's one-line button, its (already fitted) name, date and time.</summary>
        public static string SavedGameRow(string name, DateTime time) =>
            string.Format(CultureInfo.InvariantCulture, "{0}　{1} {2}", name, SavedGameDate(time), SavedGameTime(time));

        /// <summary>The game screen's list: a save's button, its (already fitted) name, date and time on three lines.</summary>
        public static string SavedGameButton(string name, DateTime time) => $"{name}\n{SavedGameDate(time)}\n{SavedGameTime(time)}";

        /// <summary>A save's date (from its file name).</summary>
        private static string SavedGameDate(DateTime time) => time.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        /// <summary>A save's time (from its file name).</summary>
        private static string SavedGameTime(DateTime time) => time.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
