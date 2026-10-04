/* ----- ----- ----- ----- */
// GameTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts with game meaning used by the logic layer (the game screen's presenter): its
    /// confirm dialog messages, the game-over message, the game-log lines it writes itself (the
    /// Core writes the lines of the actions themselves, e.g. each move taken back, the saved file
    /// name, the game result) and the players' default names. The screens' own texts stay in
    /// <c>GameMenuTexts</c> (UI).
    /// </summary>
    public static class GameTexts
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
        /// The game-over dialog's message: the winner's name with the colour it plays (e.g.
        /// 玩家一（紅方）獲勝, author decision 2026-10-02) and why, two lines.
        /// </summary>
        /// <param name="winner">The winning side; <c>None</c> is a draw (no rule ends a game in a draw yet).</param>
        /// <param name="winnerName">The winner's name (<c>GameManager.NameOf</c>); null for <see cref="DefaultPlayerName"/>.</param>
        /// <param name="winnerColor">The colour <paramref name="winner"/> plays (<c>GameManager.ColorOf</c>).</param>
        /// <param name="reason">How the game ended.</param>
        /// <param name="boardType">The board played on (a stalemate is worded differently on the dark-chess board).</param>
        public static string GameOverMessage(PlayerSide winner, string winnerName, PieceColor winnerColor, GameOverReason reason, BoardType boardType) =>
            winner == PlayerSide.None
                ? $"和棋\n{GameOverReasonText(reason, boardType)}"
                : $"{winnerName ?? DefaultPlayerName(winner)}（{SideName(winner, winnerColor)}）獲勝\n{GameOverReasonText(reason, boardType)}";

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

        // ----- Names -----

        /// <summary>
        /// A side's name in the log lines, by the colour it plays (<c>GameManager.ColorOf</c>):
        /// 紅方 / 黑方 (the colour is per game: Player1 plays the colour that moves first);
        /// 先手方 / 後手方 while a dark-chess game has not decided the colours yet.
        /// </summary>
        public static string SideName(PlayerSide side, PieceColor color) => color switch
        {
            PieceColor.Red => "紅方",
            PieceColor.Black => "黑方",
            _ => side == PlayerSide.Player2 ? "後手方" : "先手方",
        };

        /// <summary>The name of an unnamed local player: 玩家一／玩家二／玩家三 (Player1..Player3, by turn order).</summary>
        public static string DefaultPlayerName(PlayerSide side) => side switch
        {
            PlayerSide.Player2 => "玩家二",
            PlayerSide.Player3 => "玩家三",
            _ => "玩家一",
        };
    }
}
