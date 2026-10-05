/* ----- ----- ----- ----- */
// GameMenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.4
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// Texts of the screens: the info board, the saved-game lists and the settings screens'
    /// footer button. The game screen's dialog messages, game-over message and game-log
    /// lines, and the players' default names, are in the logic layer's <c>GameTexts</c>; the
    /// settings screens' rows, tabs and messages in its <c>SettingsTexts</c>; the main menu's
    /// settings entries and the menus' dialog messages in its <c>MenuTexts</c>.
    /// </summary>
    public static class GameMenuTexts
    {
        // ----- Info board -----

        /// <summary>Appended to the name of the side to move while it is in check (將軍), on the info board.</summary>
        public const string InCheckSuffix = "（將軍）";

        // ----- Saved-game list -----

        /// <summary>Shown when there is no saved game; {0} = the saves folder.</summary>
        public const string NoSavedGamesFormat = "找不到存檔。\n用左邊的「儲存遊戲」存檔後會出現在這裡，存檔位置：\n{0}";

        /// <summary>Date line of a saved game's button (from its file name).</summary>
        public const string SavedGameDateFormat = "yyyy/MM/dd";

        /// <summary>Time line of a saved game's button (from its file name).</summary>
        public const string SavedGameTimeFormat = "HH:mm:ss";

        /// <summary>Ends a saved game's name cut to one line.</summary>
        public const string Ellipsis = "…";

        /// <summary>The main menu's saved-game list (讀取存檔): the disabled row shown when there is no save.</summary>
        public const string NoSavedGamesRow = "沒有存檔";

        /// <summary>
        /// The main menu's saved-game list: a save's one-line button; {0} = name, {1} = date
        /// (<see cref="SavedGameDateFormat"/>), {2} = time (<see cref="SavedGameTimeFormat"/>).
        /// </summary>
        public const string SavedGameRowFormat = "{0}　{1} {2}";

        /// <summary>
        /// Both saved-game lists: the delete-mode toggle while off (clicking it turns delete
        /// mode on: a clicked save then asks to be deleted instead of being loaded).
        /// </summary>
        public const string DeleteModeOff = "刪除存檔";

        /// <summary>Both saved-game lists: the delete-mode toggle while on (clicking it goes back to loading saves).</summary>
        public const string DeleteModeOn = "結束刪除";

        // ----- Settings screens (遊戲設定 / 單機規則設定) -----

        /// <summary>The footer button resetting the shown tab to the defaults.</summary>
        public const string SettingsResetTab = "恢復初始";
    }
}
