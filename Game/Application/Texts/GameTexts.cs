/* ----- ----- ----- ----- */
// GameTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/06
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Families.ThreeKingdoms;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts with game meaning used by the logic layer (the game screen's presenter, the saved-game catalog, the info board's view model,
    /// the game log's composer): their confirm dialog messages, the game-over message, every game-log line (the lines of the
    /// game's own entries, composed from <c>GameLogEvent</c>s, and the lines the logic layer writes itself), the info board's
    /// names and check mark, and the players' default names. The menus' texts are in <c>MenuTexts</c> and <c>SettingsTexts</c>.
    /// Every text comes from the language files (<see cref="Lang"/>, <c>Assets/Lang/zh-TW.json</c>); this class only picks
    /// the key and fills in the parameters.
    /// </summary>
    public static class GameTexts
    {
        // ----- Confirm dialogs -----

        /// <summary>載入: the current game has unsaved changes.</summary>
        public static string DiscardUnsavedGame => Lang.Get("game.dialog.discard_unsaved");

        /// <summary>回到主畫面 while the game is still in progress (yes = resign, then go back).</summary>
        public static string ResignAndReturnToMain => Lang.Get("game.dialog.resign_and_return");

        /// <summary>重新開始 while the game is in progress (a move was made and the game is not over).</summary>
        public static string DiscardAndRestart => Lang.Get("game.dialog.discard_and_restart");

        /// <summary>A save clicked in a save list's delete mode (yes = the file is deleted).</summary>
        public static string DeleteSaveConfirm => Lang.Get("game.dialog.delete_save");

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
                ? Lang.Get("game.over.draw", GameOverReasonText(reason, boardType))
                : Lang.Get("game.over.win", winnerName ?? DefaultPlayerName(winner), SideName(winner, winnerColor), GameOverReasonText(reason, boardType));

        /// <summary>The reason line of the game-over dialog.</summary>
        public static string GameOverReasonText(GameOverReason reason, BoardType boardType) => reason switch
        {
            GameOverReason.Checkmate => Lang.Get("game.over.reason.checkmate"),
            GameOverReason.Stalemate => Lang.Get(boardType == BoardType.HalfCenter ? "game.over.reason.stalemate_dark_chess" : "game.over.reason.stalemate"),
            GameOverReason.TimeUp => Lang.Get("game.over.reason.time_up"),
            GameOverReason.Resign => Lang.Get("game.over.reason.resign"),
            GameOverReason.NoPiecesLeft => Lang.Get("game.over.reason.no_pieces_left"),
            _ => reason.ToString(),
        };

        // ----- Game log lines -----

        /// <summary>撤銷 with nothing to undo (a round needs both sides' last move above the undo floor).</summary>
        public static string UndoUnavailable => Lang.Get("log.undo.unavailable");

        /// <summary>儲存 off the Full board (only Full-board games can be saved).</summary>
        public static string SaveUnavailableBoardType => Lang.Get("log.save.unavailable.board_type");

        /// <summary>儲存 in a 揭棋 game (face-down pieces cannot be written to a saved game).</summary>
        public static string SaveUnavailableJieqi => Lang.Get("log.save.unavailable.jieqi");

        /// <summary>儲存 without a start position (e.g. a cleared board).</summary>
        public static string SaveUnavailableNoStartPosition => Lang.Get("log.save.unavailable.no_start_position");

        /// <summary>儲存 failed to write the file.</summary>
        public static string SaveFailed(string reason) => Lang.Get("log.save.failed", reason);

        /// <summary>Deleting a save failed (the file could not be deleted).</summary>
        public static string DeleteSaveFailed(string reason) => Lang.Get("log.delete.failed", reason);

        /// <summary>A failed delete shown in a dialog (the main menu has no game log; author decision 2026-10-05).</summary>
        public static string DeleteSaveFailedDialog(string reason) => Lang.Get("game.dialog.delete_save_failed", reason);

        /// <summary>放棄 when the game is already over.</summary>
        public static string ResignGameOver => Lang.Get("log.resign.game_over");

        /// <summary>放棄: <paramref name="side"/> (the side to move), playing <paramref name="color"/>, resigns.</summary>
        public static string Resigned(PlayerSide side, PieceColor color) => Lang.Get("log.resign.done", SideName(side, color));

        // ----- Game log: the game's entries (GameLogComposer) -----

        /// <summary>A shuffled HalfCenter game was dealt.</summary>
        public static string HalfCenterStarted(bool isHiddenChess) =>
            Lang.Get(isHiddenChess ? "log.start.dark_half" : "log.start.open_half");

        /// <summary>A 揭棋 game was dealt.</summary>
        public static string JieqiStarted => Lang.Get("log.start.jieqi");

        /// <summary>A 三國 game was dealt.</summary>
        public static string ThreeKingdomsStarted(HalfCrossWinCondition winCondition) =>
            Lang.Get("log.start.three_kingdoms", WinConditionName(winCondition));

        /// <summary>三國: a player claimed a team.</summary>
        public static string TeamClaimed(PlayerSide side, int team, ThreeKingdomsTeamSplit split) =>
            Lang.Get("log.three_kingdoms.team_claimed", DefaultPlayerName(side), TeamName(team, split));

        /// <summary>三國: a player with no action is skipped.</summary>
        public static string TurnSkipped(PlayerSide side) => Lang.Get("log.three_kingdoms.turn_skipped", DefaultPlayerName(side));

        /// <summary>三國: a player resigned (棄權) or ran out of time; the others play on.</summary>
        public static string PlayerForfeited(PlayerSide side, bool timeUp) =>
            Lang.Get(timeUp ? "log.three_kingdoms.timed_out" : "log.three_kingdoms.resigned", DefaultPlayerName(side));

        /// <summary>
        /// 三國's line (e.g. <c>第1回合 玩家一：翻開(3,1) 紅俥</c>): by player, as the players own
        /// teams, not colours.
        /// </summary>
        public static string ThreeKingdomsLine(MoveRecord move)
        {
            string head = Lang.Get("log.move.head", move.MoveNumber, DefaultPlayerName(move.Side));
            if (move.Kind == MoveKind.Flip)
                return head + FlipText(move);
            return head + MoveText(move);
        }

        /// <summary>
        /// A 三國 team by its pieces under <paramref name="split"/> (自訂分隊), by type with red before
        /// black, e.g. 帥將兵卒隊 / 仕相俥傌炮隊 / 士象車馬包隊 for the default split; 未定 for team 0.
        /// </summary>
        public static string TeamName(int team, ThreeKingdomsTeamSplit split) => team == 0
            ? Lang.Get("three_kingdoms.team.none")
            : Lang.Get("three_kingdoms.team.name", string.Concat(split.PiecesOf(team).Select(p => PieceConstants.GetPieceText(p.type, p.color))));

        /// <summary>A team's number in the settings menu (自訂分隊).</summary>
        public static string TeamNumber(int team) => team switch
        {
            1 or 2 or 3 => Lang.Get($"three_kingdoms.team.number.{team}"),
            _ => Lang.Get("three_kingdoms.team.number", team),
        };

        /// <summary>The way of winning's name (勝負方式).</summary>
        public static string WinConditionName(HalfCrossWinCondition condition) => condition switch
        {
            HalfCrossWinCondition.Points => Lang.Get("three_kingdoms.win.points"),
            HalfCrossWinCondition.Annihilation => Lang.Get("three_kingdoms.win.annihilation"),
            HalfCrossWinCondition.ScoreBalance => Lang.Get("three_kingdoms.win.score_balance"),
            HalfCrossWinCondition.FirstTo200 => Lang.Get("three_kingdoms.win.first_to_200"),
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
                lines.Add(Lang.Get("three_kingdoms.result.place", i + 1, nameOf(side) ?? DefaultPlayerName(side), TeamName(teamOf(side), split), rankingScoreOf(side)));
            }
            lines.Add(ThreeKingdomsReasonText(reason));
            return string.Join("\n", lines);
        }

        /// <summary>Why a 三國 game ended.</summary>
        public static string ThreeKingdomsReasonText(GameOverReason reason) => reason switch
        {
            GameOverReason.NoPiecesLeft => Lang.Get("three_kingdoms.reason.no_pieces_left"),
            GameOverReason.Resign => Lang.Get("three_kingdoms.reason.resign"),
            GameOverReason.TimeUp => Lang.Get("three_kingdoms.reason.time_up"),
            GameOverReason.Stalemate => Lang.Get("three_kingdoms.reason.stalemate"),
            GameOverReason.ScoreReached => Lang.Get("three_kingdoms.reason.score_reached"),
            _ => reason.ToString(),
        };

        /// <summary>An endgame puzzle was set up.</summary>
        public static string EndgameStarted(string title, string goal) => Lang.Get("log.start.endgame", title, goal);

        /// <summary>An opening was set up; <paramref name="ecco"/> may be null.</summary>
        public static string OpeningStarted(string title, string ecco) =>
            ecco != null ? Lang.Get("log.start.opening_with_ecco", title, ecco) : Lang.Get("log.start.opening", title);

        /// <summary>A saved game was loaded, or restarted.</summary>
        public static string SavedGameStarted(string title, bool isRestart) => Lang.Get(isRestart ? "log.start.saved_game_restart" : "log.start.saved_game", title);

        /// <summary>The game was saved to <paramref name="fileName"/>.</summary>
        public static string GameSaved(string fileName) => Lang.Get("log.save.done", fileName);

        /// <summary>A board click (debug-style line): the side to move, the selected piece, the square and what is on it.</summary>
        public static string BoardClicked(PlayerSide turn, PieceType? held, int x, int y, PieceType? clicked, bool clickedFaceDown) =>
            Lang.Get("log.debug.board_clicked", turn, held == null ? Lang.Get("log.debug.nothing") : held.Value.ToString(), x, y,
                clicked == null ? Lang.Get("log.debug.nothing") : clickedFaceDown ? Lang.Get("log.debug.face_down_piece") : clicked.Value.ToString());

        /// <summary>A click changed the selection (debug-style line).</summary>
        public static string SelectionChanged(SelectionChange change, PieceType type, int x, int y) => change switch
        {
            SelectionChange.Selected => Lang.Get("log.debug.selected", type, x, y),
            SelectionChange.Unselected => Lang.Get("log.debug.unselected", type, x, y),
            SelectionChange.Switched => Lang.Get("log.debug.switched", type, x, y),
            SelectionChange.Invalid => Lang.Get("log.debug.invalid_move", x, y),
            _ => throw new ArgumentOutOfRangeException(nameof(change), change, "Unknown selection change"),
        };

        /// <summary>An ordinary move took a piece (debug-style line).</summary>
        public static string PieceTaken(PieceType type, int x, int y) => Lang.Get("log.debug.captured", type, x, y);

        /// <summary>An ordinary move put a piece on a square (debug-style line).</summary>
        public static string PieceMoved(PieceType type, int x, int y) => Lang.Get("log.debug.moved", type, x, y);

        /// <summary>
        /// A move's line: <c>第{MoveNumber}回合 紅：{Notation}</c> or <c>第{MoveNumber}回合 黑：{Notation}</c>
        /// (e.g. <c>第1回合 紅：炮二平五</c>). The side name is the moved piece's colour, like the notation's piece characters.
        /// </summary>
        public static string MoveLine(MoveRecord move) =>
            Lang.Get("log.move.head", move.MoveNumber, ColorName(move.Color == PieceColor.Red ? PieceColor.Red : PieceColor.Black)) + move.Notation;

        /// <summary>
        /// A dark-chess action's line, which has no notation (e.g. <c>第1回合 紅：翻開(3,2) 紅俥</c>);
        /// <paramref name="moverColor"/> is the mover's colour as decided when the entry was raised.
        /// </summary>
        public static string DarkChessLine(MoveRecord move, PieceColor moverColor)
        {
            string head = Lang.Get("log.move.head", move.MoveNumber, ColorName(moverColor));
            switch (move.Kind)
            {
                case MoveKind.Flip:
                    return head + FlipText(move);
                case MoveKind.HiddenCapture:
                    return head + Lang.Get("log.move.hidden_capture", HiddenCaptureHead(move));
                case MoveKind.HiddenOwnPiece:
                    return head + Lang.Get("log.move.hidden_own_piece", HiddenCaptureHead(move));
                case MoveKind.HiddenStrongerReturn:
                    return head + Lang.Get("log.move.hidden_stronger_return", HiddenCaptureHead(move));
                case MoveKind.HiddenStrongerSuicide:
                    return head + Lang.Get("log.move.hidden_stronger_suicide", HiddenCaptureHead(move), PieceText(move.Piece));
                case MoveKind.Suicide:
                    return head + Lang.Get("log.move.suicide", PieceText(move.Piece), move.FromX, move.FromY, move.ToX, move.ToY, PieceText(move.Revealed));
                default:
                    return head + MoveText(move);
            }
        }

        /// <summary>A flip's part of a line, e.g. 翻開(3,2) 紅俥.</summary>
        private static string FlipText(MoveRecord move) => Lang.Get("log.move.flip", move.FromX, move.FromY, PieceText(move.Revealed));

        /// <summary>A move's part of a line, e.g. 紅俥(0,0)→(0,4)，吃黑車.</summary>
        private static string MoveText(MoveRecord move) =>
            Lang.Get("log.move.move", PieceText(move.Piece), move.FromX, move.FromY, move.ToX, move.ToY)
            + (move.Captured != null ? Lang.Get("log.move.capture", PieceText(move.Captured)) : "");

        /// <summary>The common start of a hidden-capture line, e.g. 紅俥(2,1)暗吃(3,1)，翻出黑卒.</summary>
        private static string HiddenCaptureHead(MoveRecord move) =>
            Lang.Get("log.move.hidden_capture_head", PieceText(move.Piece), move.FromX, move.FromY, move.ToX, move.ToY, PieceText(move.Revealed));

        /// <summary>A piece's character with its colour name, e.g. 黑卒 (see <see cref="PieceConstants.GetPieceText"/>).</summary>
        private static string PieceText(PieceInfo info) =>
            info == null ? Lang.Get("piece.unknown") : ColorName(info.Color) + PieceConstants.GetPieceText(info.Type, info.Color);

        /// <summary>紅 / 黑 for the log lines; 未定 for a colour not decided yet.</summary>
        private static string ColorName(PieceColor color) => color switch
        {
            PieceColor.Red => Lang.Get("color.red"),
            PieceColor.Black => Lang.Get("color.black"),
            _ => Lang.Get("color.undecided"),
        };

        /// <summary>The first action decided the factions.</summary>
        public static string FactionsDecided(PieceColor player1Color, PieceColor player2Color) =>
            Lang.Get("log.factions_decided", PlayerSide.Player1, ColorName(player1Color), PlayerSide.Player2, ColorName(player2Color));

        /// <summary><paramref name="side"/> is in check.</summary>
        public static string CheckGiven(PlayerSide side) => Lang.Get("log.debug.check", side);

        /// <summary>One tactical event (Chinese name, event type, mover, move and involved pieces).</summary>
        public static string TacticDetected(TacticalEvent e)
        {
            var m = e.Move;
            string involved = e.Pieces.Count == 0
                ? Lang.Get("log.debug.tactic_no_pieces")
                : string.Join(", ", e.Pieces.Select(p => Lang.Get("log.debug.tactic_piece", p.Side, p.Type, p.X, p.Y)));
            return Lang.Get("log.debug.tactic", e.ChineseName, e.Type, e.Mover, m.Piece.Type, m.FromX, m.FromY, m.ToX, m.ToY, involved);
        }

        /// <summary>A move taken back; <paramref name="moveLine"/> is its line (<see cref="MoveLine"/>, <see cref="DarkChessLine"/> or <see cref="MoveBackTo"/>).</summary>
        public static string MoveTakenBack(string moveLine) => Lang.Get("log.move.taken_back", moveLine);

        /// <summary>A taken-back move with neither notation nor dark-chess rules: the piece and its from-square.</summary>
        public static string MoveBackTo(PieceType type, int x, int y) => Lang.Get("log.debug.back_to", type, x, y);

        /// <summary><paramref name="side"/>'s clock ran out and the game goes on.</summary>
        public static string TimeRanOut(PlayerSide side) => Lang.Get("log.debug.time_ran_out", side);

        /// <summary>The game ended.</summary>
        public static string GameEnded(PlayerSide winner, GameOverReason reason) => Lang.Get("log.debug.game_ended", winner, reason);

        // ----- Names -----

        /// <summary>
        /// A side's name in the log lines, by the colour it plays (<c>GameManager.ColorOf</c>):
        /// 紅方 / 黑方 (the colour is per game: Player1 plays the colour that moves first);
        /// 先手方 / 後手方 while a dark-chess game has not decided the colours yet.
        /// </summary>
        public static string SideName(PlayerSide side, PieceColor color) => color switch
        {
            PieceColor.Red => Lang.Get("side.red"),
            PieceColor.Black => Lang.Get("side.black"),
            _ => Lang.Get(side == PlayerSide.Player2 ? "side.second" : "side.first"),
        };

        // ----- Info board -----

        /// <summary>Appended to the name of the side to move while it is in check (將軍), on the info board.</summary>
        public static string InCheckSuffix => Lang.Get("info.in_check");

        /// <summary>
        /// The info board's name for a player with no name: the colour it plays — 紅方玩家 /
        /// 黑方玩家, or 先手玩家 / 後手玩家 while the colours are not decided yet (dark chess).
        /// </summary>
        public static string InfoBoardColorName(PlayerSide side, PieceColor color) => color switch
        {
            PieceColor.Red => Lang.Get("info.player.red"),
            PieceColor.Black => Lang.Get("info.player.black"),
            // Player1 always moves first.
            _ => Lang.Get(side == PlayerSide.Player1 ? "info.player.first" : "info.player.second"),
        };

        /// <summary>
        /// 三國's info-board line under a player's name: its team and points (計分: the points and
        /// the team's threshold), or 未定 before it has a team; 棄權 / 出局 when it no longer plays.
        /// </summary>
        public static string ThreeKingdomsStatus(int team, ThreeKingdomsTeamSplit split, int score, int? threshold, bool resigned, bool timedOut, bool isOut)
        {
            string points = threshold != null
                ? Lang.Get("three_kingdoms.status.points_of_threshold", score, threshold)
                : Lang.Get("three_kingdoms.status.points", score);
            string state = resigned ? Lang.Get("three_kingdoms.status.resigned")
                : timedOut ? Lang.Get("three_kingdoms.status.timed_out")
                : isOut ? Lang.Get("three_kingdoms.status.out")
                : "";
            return Lang.Get("three_kingdoms.status.line", TeamName(team, split), points, state);
        }

        /// <summary>The name of an unnamed local player: 玩家一／玩家二／玩家三 (Player1..Player3, by turn order).</summary>
        public static string DefaultPlayerName(PlayerSide side) => side switch
        {
            PlayerSide.Player2 => Lang.Get("player.default.2"),
            PlayerSide.Player3 => Lang.Get("player.default.3"),
            _ => Lang.Get("player.default.1"),
        };
    }
}
