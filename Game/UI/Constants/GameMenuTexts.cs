/* ----- ----- ----- ----- */
// GameMenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// Texts of the game screen's menu (<c>UIGameMenu</c>, docs/PLAN.md in-game menu): its
    /// confirm dialog messages, the game-log lines it writes itself (the Core writes the
    /// lines of the actions themselves, e.g. each move taken back, the saved file name, the
    /// game result) and the saved-game list's button texts.
    /// </summary>
    public static class GameMenuTexts
    {
        // ----- Confirm dialogs -----

        /// <summary>載入: the current game has unsaved changes.</summary>
        public const string DiscardUnsavedGame = "目前棋局尚未儲存，是否捨棄？";

        /// <summary>回到主畫面 while the game is still in progress (yes = resign, then go back).</summary>
        public const string ResignAndReturnToMain = "是否放棄這局並回到主畫面？";

        // ----- Game log lines -----

        /// <summary>撤銷 with nothing to undo (a round needs both sides' last move above the undo floor).</summary>
        public const string UndoUnavailable = "(Undo) 無法悔棋：悔棋一次退回一個回合（雙方各一步），目前還沒有可以退回的回合";

        /// <summary>儲存 off the Full board (only Full-board games can be saved).</summary>
        public const string SaveUnavailableBoardType = "(Save) 無法存檔：只有大盤對局可以存檔";

        /// <summary>儲存 without a start position (e.g. a cleared board).</summary>
        public const string SaveUnavailableNoStartPosition = "(Save) 無法存檔：這個盤面沒有開局局面（例如清空的棋盤）";

        /// <summary>儲存 failed to write the file.</summary>
        public static string SaveFailed(string reason) => $"(Save) 存檔失敗：{reason}";

        /// <summary>放棄 when the game is already over.</summary>
        public const string ResignGameOver = "(Resign) 對局已經結束";

        /// <summary>放棄: <paramref name="side"/> (the side to move) resigns.</summary>
        public static string Resigned(PlayerSide side) => $"(Resign) {SideName(side)}認輸";

        /// <summary>A side's name in the log lines: Player1 紅方, Player2 黑方 (like the move lines).</summary>
        public static string SideName(PlayerSide side) => side == PlayerSide.Player2 ? "黑方" : "紅方";

        // ----- Saved-game list -----

        /// <summary>Shown when there is no saved game; {0} = the saves folder.</summary>
        public const string NoSavedGamesFormat = "找不到存檔。\n用左邊的「儲存遊戲」存檔後會出現在這裡，存檔位置：\n{0}";

        /// <summary>Date line of a saved game's button (from its file name).</summary>
        public const string SavedGameDateFormat = "yyyy/MM/dd";

        /// <summary>Time line of a saved game's button (from its file name).</summary>
        public const string SavedGameTimeFormat = "HH:mm:ss";

        /// <summary>Ends a saved game's name cut to one line.</summary>
        public const string Ellipsis = "…";
    }
}
