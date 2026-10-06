/* ----- ----- ----- ----- */
// GameTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/06
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Families.ThreeKingdoms;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts with game meaning used by the logic layer (the game screen's presenter, the saved-game catalog, the info board's view model,
    /// the game log's composer): their confirm dialog messages, the game-over message, every game-log line (the lines of the
    /// game's own entries, composed from <c>GameLogEvent</c>s, and the lines the logic layer writes itself), the info board's
    /// names and check mark, and the players' default names. The menus' texts are in <c>MenuTexts</c> and <c>SettingsTexts</c>.
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

        /// <summary>A save clicked in a save list's delete mode (yes = the file is deleted).</summary>
        public const string DeleteSaveConfirm = "確定要刪除這個存檔？";

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

        /// <summary>儲存 in a 揭棋 game (face-down pieces cannot be written to a saved game).</summary>
        public const string SaveUnavailableJieqi = "(Save) 無法存檔：揭棋對局目前不能存檔";

        /// <summary>儲存 without a start position (e.g. a cleared board).</summary>
        public const string SaveUnavailableNoStartPosition = "(Save) 無法存檔：這個盤面沒有開局局面（例如清空的棋盤）";

        /// <summary>儲存 failed to write the file.</summary>
        public static string SaveFailed(string reason) => $"(Save) 存檔失敗：{reason}";

        /// <summary>Deleting a save failed (the file could not be deleted).</summary>
        public static string DeleteSaveFailed(string reason) => $"(Delete) 刪除存檔失敗：{reason}";

        /// <summary>A failed delete shown in a dialog (the main menu has no game log; author decision 2026-10-05).</summary>
        public static string DeleteSaveFailedDialog(string reason) => $"刪除存檔失敗：{reason}";

        /// <summary>放棄 when the game is already over.</summary>
        public const string ResignGameOver = "(Resign) 對局已經結束";

        /// <summary>放棄: <paramref name="side"/> (the side to move), playing <paramref name="color"/>, resigns.</summary>
        public static string Resigned(PlayerSide side, PieceColor color) => $"(Resign) {SideName(side, color)}認輸";

        // ----- Game log: the game's entries (GameLogComposer) -----

        /// <summary>A shuffled HalfCenter game was dealt.</summary>
        public static string HalfCenterStarted(bool isHiddenChess) =>
            isHiddenChess ? "(DarkChess) 新局：暗棋半盤" : "(DarkChess) 新局：明棋半盤";

        /// <summary>A 揭棋 game was dealt.</summary>
        public const string JieqiStarted = "(Jieqi) 新局：揭棋大盤";

        /// <summary>A 三國 game was dealt.</summary>
        public static string ThreeKingdomsStarted(HalfCrossWinCondition winCondition) =>
            $"(ThreeKingdoms) 新局：三國半盤（{WinConditionName(winCondition)}）";

        /// <summary>三國: a player claimed a team.</summary>
        public static string TeamClaimed(PlayerSide side, int team, ThreeKingdomsTeamSplit split) =>
            $"(Faction) {DefaultPlayerName(side)} 執{TeamName(team, split)}";

        /// <summary>三國: a player with no action is skipped.</summary>
        public static string TurnSkipped(PlayerSide side) => $"(Turn) {DefaultPlayerName(side)} 無法行動，跳過";

        /// <summary>三國: a player resigned (棄權) or ran out of time; the others play on.</summary>
        public static string PlayerForfeited(PlayerSide side, bool timeUp) =>
            $"(Forfeit) {DefaultPlayerName(side)} {(timeUp ? "超時，視為棄權" : "棄權")}：棋子留在盤上，之後的回合跳過";

        /// <summary>
        /// 三國's line (e.g. <c>第1回合 玩家一：翻開(3,1) 紅俥</c>): by player, as the players own
        /// teams, not colours.
        /// </summary>
        public static string ThreeKingdomsLine(MoveRecord move)
        {
            string head = $"第{move.MoveNumber}回合 {DefaultPlayerName(move.Side)}：";
            if (move.Kind == MoveKind.Flip)
                return head + $"翻開({move.FromX},{move.FromY}) {PieceText(move.Revealed)}";
            return head + $"{PieceText(move.Piece)}({move.FromX},{move.FromY})→({move.ToX},{move.ToY})"
                + (move.Captured != null ? $"，吃{PieceText(move.Captured)}" : "");
        }

        /// <summary>
        /// A 三國 team by its pieces under <paramref name="split"/> (自訂分隊), by type with red before
        /// black, e.g. 帥將兵卒隊 / 仕相俥傌炮隊 / 士象車馬包隊 for the default split; 未定 for team 0.
        /// </summary>
        public static string TeamName(int team, ThreeKingdomsTeamSplit split) => team == 0
            ? "未定"
            : string.Concat(split.PiecesOf(team).Select(p => PieceConstants.GetPieceText(p.type, p.color))) + "隊";

        /// <summary>A team's number in the settings menu (自訂分隊).</summary>
        public static string TeamNumber(int team) => team switch
        {
            1 => "第一隊",
            2 => "第二隊",
            3 => "第三隊",
            _ => $"第{team}隊",
        };

        /// <summary>The way of winning's name (勝負方式).</summary>
        public static string WinConditionName(HalfCrossWinCondition condition) => condition switch
        {
            HalfCrossWinCondition.Points => "計分",
            HalfCrossWinCondition.Annihilation => "全滅",
            HalfCrossWinCondition.Recall => "收軍",
            HalfCrossWinCondition.ScoreBalance => "得失分",
            HalfCrossWinCondition.FirstTo200 => "先得 200 分",
            _ => condition.ToString(),
        };

        /// <summary>
        /// 三國's game-over dialog message: every player's place with its name, team and points,
        /// first place first, then why the game ended.
        /// </summary>
        /// <param name="ranking">The players, first place first (<c>GameOverInfo.Ranking</c>).</param>
        /// <param name="nameOf">A player's name (<c>GameManager.NameOf</c>); null for <see cref="DefaultPlayerName"/>.</param>
        /// <param name="teamOf">A player's team (<c>GameManager.TeamOf</c>).</param>
        /// <param name="rankingScoreOf">The number the players are ranked by (計分: points above the threshold).</param>
        public static string ThreeKingdomsResult(IReadOnlyList<PlayerSide> ranking, Func<PlayerSide, string> nameOf,
            Func<PlayerSide, int> teamOf, Func<PlayerSide, int> rankingScoreOf, ThreeKingdomsTeamSplit split, GameOverReason reason)
        {
            var lines = new List<string>();
            for (int i = 0; i < ranking.Count; i++)
            {
                var side = ranking[i];
                lines.Add($"第{i + 1}名 {nameOf(side) ?? DefaultPlayerName(side)}（{TeamName(teamOf(side), split)}）{rankingScoreOf(side)} 分");
            }
            lines.Add(ThreeKingdomsReasonText(reason));
            return string.Join("\n", lines);
        }

        /// <summary>Why a 三國 game ended.</summary>
        public static string ThreeKingdomsReasonText(GameOverReason reason) => reason switch
        {
            GameOverReason.NoPiecesLeft => "只剩一方還有棋子",
            GameOverReason.Resign => "其他玩家棄權",
            GameOverReason.Stalemate => "所有人都無法行動",
            GameOverReason.ScoreReached => "已有玩家先得 200 分",
            _ => reason.ToString(),
        };

        /// <summary>An endgame puzzle was set up.</summary>
        public static string EndgameStarted(string title, string goal) => $"(Endgame) {title} ({goal})";

        /// <summary>An opening was set up; <paramref name="ecco"/> may be null.</summary>
        public static string OpeningStarted(string title, string ecco) =>
            ecco != null ? $"(Opening) {title} ({ecco})" : $"(Opening) {title}";

        /// <summary>A saved game was loaded, or restarted.</summary>
        public static string SavedGameStarted(string title, bool isRestart) => $"{(isRestart ? "(Restart)" : "(Load)")} {title}";

        /// <summary>The game was saved to <paramref name="fileName"/>.</summary>
        public static string GameSaved(string fileName) => $"(Save) {fileName}";

        /// <summary>A board click (debug-style line): the side to move, the selected piece, the square and what is on it.</summary>
        public static string BoardClicked(PlayerSide turn, PieceType? held, int x, int y, PieceType? clicked, bool clickedFaceDown) =>
            $"Current turn: {turn}, holding: {(held == null ? "null" : held.Value.ToString())},\n" +
            $"clicked at ({x},{y}), on: {(clicked == null ? "null" : clickedFaceDown ? "face-down piece" : clicked.Value.ToString())}";

        /// <summary>A click changed the selection (debug-style line).</summary>
        public static string SelectionChanged(SelectionChange change, PieceType type, int x, int y) => change switch
        {
            SelectionChange.Selected => $"(Action) Selected {type} at ({x},{y})",
            SelectionChange.Unselected => $"(Action) Un-selected {type} at ({x},{y})",
            SelectionChange.Switched => $"(Action) Switched to {type} at ({x},{y})",
            SelectionChange.Invalid => $"(Action) Invalid move to ({x},{y})",
            _ => throw new ArgumentOutOfRangeException(nameof(change), change, "Unknown selection change"),
        };

        /// <summary>An ordinary move took a piece (debug-style line).</summary>
        public static string PieceTaken(PieceType type, int x, int y) => $"(Action) Captured {type} at ({x},{y})";

        /// <summary>An ordinary move put a piece on a square (debug-style line).</summary>
        public static string PieceMoved(PieceType type, int x, int y) => $"(Action) Moved {type} to ({x},{y})";

        /// <summary>
        /// A move's line: <c>第{MoveNumber}回合 紅：{Notation}</c> or <c>第{MoveNumber}回合 黑：{Notation}</c>
        /// (e.g. <c>第1回合 紅：炮二平五</c>). The side name is the moved piece's colour, like the notation's piece characters.
        /// </summary>
        public static string MoveLine(MoveRecord move) =>
            $"第{move.MoveNumber}回合 {(move.Color == PieceColor.Red ? "紅" : "黑")}：{move.Notation}";

        /// <summary>
        /// A dark-chess action's line, which has no notation (e.g. <c>第1回合 紅：翻開(3,2) 紅俥</c>);
        /// <paramref name="moverColor"/> is the mover's colour as decided when the entry was raised.
        /// </summary>
        public static string DarkChessLine(MoveRecord move, PieceColor moverColor)
        {
            string head = $"第{move.MoveNumber}回合 {ColorName(moverColor)}：";
            switch (move.Kind)
            {
                case MoveKind.Flip:
                    return head + $"翻開({move.FromX},{move.FromY}) {PieceText(move.Revealed)}";
                case MoveKind.HiddenCapture:
                    return head + $"{HiddenCaptureHead(move)}，吃掉";
                case MoveKind.HiddenOwnPiece:
                    return head + $"{HiddenCaptureHead(move)}（己方），退回原位";
                case MoveKind.HiddenStrongerReturn:
                    return head + $"{HiddenCaptureHead(move)}（吃不了），退回原位";
                case MoveKind.HiddenStrongerSuicide:
                    return head + $"{HiddenCaptureHead(move)}（吃不了），{PieceText(move.Piece)}陣亡";
                case MoveKind.Suicide:
                    return head + $"{PieceText(move.Piece)}({move.FromX},{move.FromY})撞({move.ToX},{move.ToY}){PieceText(move.Revealed)}，自殺陣亡";
                default:
                    return head + $"{PieceText(move.Piece)}({move.FromX},{move.FromY})→({move.ToX},{move.ToY})"
                        + (move.Captured != null ? $"，吃{PieceText(move.Captured)}" : "");
            }
        }

        /// <summary>The common start of a hidden-capture line, e.g. 紅俥(2,1)暗吃(3,1)，翻出黑卒.</summary>
        private static string HiddenCaptureHead(MoveRecord move) =>
            $"{PieceText(move.Piece)}({move.FromX},{move.FromY})暗吃({move.ToX},{move.ToY})，翻出{PieceText(move.Revealed)}";

        /// <summary>A piece's character with its colour name, e.g. 黑卒 (see <see cref="PieceConstants.GetPieceText"/>).</summary>
        private static string PieceText(PieceInfo info) =>
            info == null ? "?" : ColorName(info.Color) + PieceConstants.GetPieceText(info.Type, info.Color);

        /// <summary>紅 / 黑 for the log lines; 未定 for a colour not decided yet.</summary>
        private static string ColorName(PieceColor color) => color switch
        {
            PieceColor.Red => "紅",
            PieceColor.Black => "黑",
            _ => "未定",
        };

        /// <summary>The first action decided the factions.</summary>
        public static string FactionsDecided(PieceColor player1Color, PieceColor player2Color) =>
            $"(Faction) {PlayerSide.Player1} 執{ColorName(player1Color)}，{PlayerSide.Player2} 執{ColorName(player2Color)}";

        /// <summary><paramref name="side"/> is in check.</summary>
        public static string CheckGiven(PlayerSide side) => $"(Check) {side} is in check";

        /// <summary>One tactical event (Chinese name, event type, mover, move and involved pieces).</summary>
        public static string TacticDetected(TacticalEvent e)
        {
            var m = e.Move;
            string involved = e.Pieces.Count == 0
                ? "-"
                : string.Join(", ", e.Pieces.Select(p => $"{p.Side} {p.Type} ({p.X},{p.Y})"));
            return $"(Tactic) {e.ChineseName} [{e.Type}] {e.Mover} {m.Piece.Type} ({m.FromX},{m.FromY})->({m.ToX},{m.ToY}); pieces: {involved}";
        }

        /// <summary>A move taken back; <paramref name="moveLine"/> is its line (<see cref="MoveLine"/>, <see cref="DarkChessLine"/> or <see cref="MoveBackTo"/>).</summary>
        public static string MoveTakenBack(string moveLine) => $"(Undo) {moveLine}";

        /// <summary>A taken-back move with neither notation nor dark-chess rules: the piece and its from-square.</summary>
        public static string MoveBackTo(PieceType type, int x, int y) => $"{type} back to ({x},{y})";

        /// <summary><paramref name="side"/>'s clock ran out and the game goes on.</summary>
        public static string TimeRanOut(PlayerSide side) => $"(Timer) {side} ran out of time";

        /// <summary>The game ended.</summary>
        public static string GameEnded(PlayerSide winner, GameOverReason reason) => $"(Game over) {winner} wins ({reason})";

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

        // ----- Info board -----

        /// <summary>Appended to the name of the side to move while it is in check (將軍), on the info board.</summary>
        public const string InCheckSuffix = "（將軍）";

        /// <summary>
        /// The info board's name for a player with no name: the colour it plays — 紅方玩家 /
        /// 黑方玩家, or 先手玩家 / 後手玩家 while the colours are not decided yet (dark chess).
        /// </summary>
        public static string InfoBoardColorName(PlayerSide side, PieceColor color) => color switch
        {
            PieceColor.Red => "紅方玩家",
            PieceColor.Black => "黑方玩家",
            // Player1 always moves first.
            _ => side == PlayerSide.Player1 ? "先手玩家" : "後手玩家",
        };

        /// <summary>
        /// 三國's info-board line under a player's name: its team and points (計分: the points and
        /// the team's threshold), or 未定 before it has a team; 棄權 / 出局 when it no longer plays.
        /// </summary>
        public static string ThreeKingdomsStatus(int team, ThreeKingdomsTeamSplit split, int score, int? threshold, bool resigned, bool isOut)
        {
            string points = threshold != null ? $"{score}/{threshold}分" : $"{score}分";
            string state = resigned ? "（棄權）" : isOut ? "（出局）" : "";
            return team == 0 ? $"未定 {points}{state}" : $"{TeamName(team, split)} {points}{state}";
        }

        /// <summary>The name of an unnamed local player: 玩家一／玩家二／玩家三 (Player1..Player3, by turn order).</summary>
        public static string DefaultPlayerName(PlayerSide side) => side switch
        {
            PlayerSide.Player2 => "玩家二",
            PlayerSide.Player3 => "玩家三",
            _ => "玩家一",
        };
    }
}
