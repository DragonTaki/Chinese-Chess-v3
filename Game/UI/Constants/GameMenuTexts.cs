/* ----- ----- ----- ----- */
// GameMenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/02
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
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

        /// <summary>重新開始 while the game is in progress (a move was made and the game is not over).</summary>
        public const string DiscardAndRestart = "是否放棄目前進度並重新開始？";

        // ----- Game-over dialog -----

        /// <summary>
        /// The game-over dialog's message: the winner (by the colour it plays) and why, two lines.
        /// </summary>
        /// <param name="winner">The winning side; <c>None</c> is a draw (no rule ends a game in a draw yet).</param>
        /// <param name="winnerColor">The colour <paramref name="winner"/> plays (<c>GameManager.ColorOf</c>).</param>
        /// <param name="reason">How the game ended.</param>
        /// <param name="boardType">The board played on (a stalemate is worded differently on the dark-chess board).</param>
        public static string GameOverMessage(PlayerSide winner, PieceColor winnerColor, GameOverReason reason, BoardType boardType) =>
            winner == PlayerSide.None
                ? $"和棋\n{GameOverReasonText(reason, boardType)}"
                : $"{SideName(winner, winnerColor)}獲勝\n{GameOverReasonText(reason, boardType)}";

        /// <summary>The reason line of the game-over dialog.</summary>
        public static string GameOverReasonText(GameOverReason reason, BoardType boardType) => reason switch
        {
            GameOverReason.Checkmate => "將死",
            GameOverReason.Stalemate => boardType == BoardType.HalfCenter ? "對方無法行動" : "困斃（對方無子可走）",
            GameOverReason.TimeUp => "對方超時",
            GameOverReason.Resign => "對方認輸",
            GameOverReason.NoPiecesLeft => "對方棋子被吃光",
            _ => reason.ToString(),
        };

        // ----- Info board -----

        /// <summary>Appended to the name of the side to move while it is in check (將軍), on the info board.</summary>
        public const string InCheckSuffix = "（將軍）";

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

        /// <summary>放棄: <paramref name="side"/> (the side to move), playing <paramref name="color"/>, resigns.</summary>
        public static string Resigned(PlayerSide side, PieceColor color) => $"(Resign) {SideName(side, color)}認輸";

        /// <summary>
        /// A side's name in the log lines, by the colour it plays (<c>GameManager.ColorOf</c>):
        /// 紅方 / 黑方 (on the Full board Player1 is red, Player2 black, like the move lines);
        /// 先手方 / 後手方 while a dark-chess game has not decided the colours yet.
        /// </summary>
        public static string SideName(PlayerSide side, PieceColor color) => color switch
        {
            PieceColor.Red => "紅方",
            PieceColor.Black => "黑方",
            _ => side == PlayerSide.Player2 ? "後手方" : "先手方",
        };

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

        // ----- New-game menu -----

        /// <summary>A new-game mode whose game cannot be started yet (揭棋大盤, 三國半盤); <paramref name="mode"/> = its button text.</summary>
        public static string NewGameModeUnavailable(string mode) => $"「{mode}」尚未完成，目前無法開始。";

        // ----- Settings menu (遊戲設定 / 規則設定) -----

        /// <summary>Leaving the settings menu (back, or opening another entry) with unsaved changes.</summary>
        public const string DiscardUnsavedSettings = "設定尚未儲存，是否捨棄變更？";

        /// <summary>The save failed (details are in the log).</summary>
        public const string SettingsSaveFailed = "設定檔寫入失敗，請查看紀錄。";

        public const string SettingsSaveAndBack = "儲存並返回";
        public const string SettingsBack = "返回";

        /// <summary>A setting's button text: name, then its value.</summary>
        public static string SettingText(string name, string value) => $"{name}：{value}";

        public const string On = "開";
        public const string Off = "關";

        public const string SectionRules = "── 規則（下一局開始生效）──";
        public const string SectionDarkChess = "── 暗棋規則（下一局開始生效）──";
        public const string SectionTimer = "── 計時（下一局開始生效）──";
        public const string SectionHints = "── 提示（立即生效）──";
        public const string SectionOther = "── 其他 ──";

        public const string GeneralCanSeeGeneral = "王見王";
        public const string GeneralCanLeavePalace = "將帥出宮";
        public const string AdvisorCanLeavePalace = "士出宮";
        public const string ElephantEyeBlocks = "塞象眼";
        public const string HorseLegBlocks = "蹩馬腳";

        public const string HiddenChess = "暗棋（蓋子）";
        public const string CanCaptureHiddenPiece = "暗吃";
        public const string CaptureHiddenStrongerSuicide = "暗吃到更大的子時吃方被吃";
        public const string AllowChainCapture = "連吃";
        public const string ChariotRushHorseDiagonal = "車衝馬斜";
        public const string CannonMustJump = "包跳吃子";

        public const string StepTimer = "限制步時";
        public const string LoseOnTimeUp = "超時判負";
        public const string TimerMode = "計時方式";
        public const string TimerModeCountDown = "倒數";
        public const string TimerModeCountUp = "正數";

        /// <summary>The value of a time-limit setting while the clocks count up (正數 only measures time).</summary>
        public const string NotWithCountUp = "正數不限時";

        public const string LegalMoveHints = "可走位置提示";
        public const string HangingPieceHints = "無根子提示";

        public const string DebugLog = "顯示 DEBUG 紀錄";
    }
}
