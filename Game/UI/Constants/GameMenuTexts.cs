/* ----- ----- ----- ----- */
// GameMenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/04
// Version: v1.2
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
        /// 紅方 / 黑方 (the colour is per game: Player1 plays the colour that moves first);
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

        // ----- Settings screens (遊戲設定 / 單機規則設定) -----

        /// <summary>The save failed (details are in the log).</summary>
        public const string SettingsSaveFailed = "設定檔寫入失敗，請查看紀錄。";

        /// <summary>The footer button resetting the shown tab to the defaults.</summary>
        public const string SettingsResetTab = "恢復初始";

        /// <summary>The confirmation before resetting the shown tab.</summary>
        public const string ResetTabToDefaults = "將目前分頁恢復成預設值？";

        /// <summary>A setting's button text: name, then its value.</summary>
        public static string SettingText(string name, string value) => $"{name}：{value}";

        public const string On = "開";
        public const string Off = "關";

        /// <summary>
        /// <c>SettingText</c>, <c>On</c> and <c>Off</c> were the old button texts (<c>名稱：開</c>);
        /// the tabbed screens show a switch instead. Kept, unused.
        /// </summary>
        public const string SectionRules = "── 規則（下一局開始生效）──";
        public const string SectionDarkChess = "── 暗棋規則（下一局開始生效）──";
        public const string SectionTimer = "── 計時（下一局開始生效）──";
        public const string SectionHints = "── 提示（立即生效）──";
        public const string SectionOther = "── 其他 ──";

        public const string GeneralCanSeeGeneral = "無視王見王規則";
        public const string GeneralCanLeavePalace = "將帥無視九宮範圍";
        public const string AdvisorCanLeavePalace = "士無視九宮範圍";
        public const string ElephantEyeBlocks = "象眼可被塞";
        public const string HorseLegBlocks = "馬腳可被蹩";

        public const string HiddenChess = "暗棋（蓋子）";
        public const string CanCaptureHiddenPiece = "暗吃";
        public const string CaptureHiddenStrongerSuicide = "吃更大暗棋會自殺";
        public const string AllowChainCapture = "連吃";
        public const string ChariotRushHorseDiagonal = "車衝馬斜";
        public const string CannonMustJump = "砲需隔一子吃棋";
        public const string CaptureOwnPiece = "可吃己方棋子";
        public const string Suicide = "可撞大子自殺";

        public const string StepTimer = "限制步時";
        public const string LoseOnTimeUp = "超時判負";
        public const string TimerMode = "計時方式";
        public const string TimerModeCountDown = "倒數";
        public const string TimerModeCountUp = "正數";
        public const string TotalTime = "局時";
        public const string StepTime = "步時";
        public const string Increment = "加秒";

        /// <summary>The value of a time-limit setting while the clocks count up (正數 only measures time).</summary>
        public const string NotWithCountUp = "正數不限時";

        public const string LegalMoveHints = "可走位置提示";
        public const string HangingPieceHints = "無根子提示";

        // DEBUG tab (除錯功能): one section per kind of debug feature.
        public const string SectionDebugLog = "── 紀錄（立即生效）──";
        public const string SectionDebugVisual = "── 視覺除錯（立即生效）──";
        public const string SectionDebugPerformance = "── 效能與連線（立即生效）──";
        public const string DebugLog = "除錯訊息";
        public const string ConsoleTrace = "主控台追蹤訊息";
        public const string LabelBackgrounds = "標籤紅色背景";
        public const string LayoutOutlines = "排版外框";
        public const string StarEffectFrames = "星空特效範圍框";
        public const string ShowFps = "顯示 FPS";
        public const string ShowNetworkLatency = "顯示網路延遲";

        /// <summary>Main menu entry and title of the rules screen (local games only; a network game does not use these rules).</summary>
        public const string LocalRuleSettings = "單機規則設定";

        /// <summary>Main menu entry and title of the general settings screen.</summary>
        public const string GameSettings = "遊戲設定";

        // Tabs of 遊戲設定.
        public const string TabDisplay = "畫面";
        public const string TabSound = "聲音";
        public const string TabGame = "遊戲";
        public const string TabDebug = "開發人員選項";

        /// <summary>A rules screen tab: the game kind's name (as on the new-game menu).</summary>
        public static string GameKindName(GameKind kind) => kind switch
        {
            GameKind.Traditional => "傳統大盤",
            GameKind.Flip => "揭棋大盤",
            GameKind.DarkHalf => "暗棋半盤",
            GameKind.OpenHalf => "明棋半盤",
            GameKind.ThreeKingdoms => "三國半盤",
            _ => throw new System.ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>
        /// A setting that can be changed but does nothing yet: its name with this mark. No longer
        /// shown (the author asked on 2026-10-02 for no 未實作 mark in the settings screens; which
        /// items do nothing is only in the code and docs/SETTINGS.md §4). Kept, unused.
        /// </summary>
        public static string NotImplemented(string name) => $"{name}（未實作）";

        /// <summary>A time-limit switch while the clocks count up: its name with <see cref="NotWithCountUp"/>.</summary>
        public static string WithCountUpNote(string name) => $"{name}（{NotWithCountUp}）";

        // 畫面 (display; all but the wheel step are not implemented yet).
        public const string Resolution = "解析度";
        public static readonly string[] ResolutionOptions = { "1280 × 720", "1600 × 900", "1920 × 1080", "2560 × 1440" };
        public const string DisplayMode = "顯示模式";
        public static readonly string[] DisplayModeOptions = { "視窗", "全螢幕", "無邊框" };
        public const string VSync = "垂直同步";
        public const string Fps = "FPS";
        public static readonly string[] FpsOptions = { "30", "60", "120", "144" };
        public const string UiScale = "UI 縮放";
        public const string WheelScrollStep = "滾輪捲動量";

        // 聲音 (sound; not implemented yet: there is no sound system).
        public const string MasterVolume = "主音量";
        public const string MusicVolume = "音樂";
        public const string EffectsVolume = "音效";
        public const string Mute = "靜音";

        // 遊戲 (game).
        public const string PlayerName = "玩家名稱";
        public const string PlayerNamePlaceholder = "（未命名）";
        public const string Player1Name = "玩家一名稱";
        public const string Player2Name = "玩家二名稱";
        public const string Player3Name = "玩家三名稱";

        /// <summary>The name of an unnamed local player: 玩家一／玩家二／玩家三 (Player1..Player3, by turn order).</summary>
        public static string DefaultPlayerName(PlayerSide side) => side switch
        {
            PlayerSide.Player2 => "玩家二",
            PlayerSide.Player3 => "玩家三",
            _ => "玩家一",
        };
        public const string MoveAnimationSpeed = "走子動畫速度";
        public static readonly string[] MoveAnimationSpeedOptions = { "慢", "普通", "快", "關閉" };
        public const string BoardStyle = "棋盤樣式";
        public static readonly string[] BoardStyleOptions = { "預設", "木紋", "簡約" };
        public const string PieceStyle = "棋子樣式";
        public static readonly string[] PieceStyleOptions = { "預設", "傳統", "簡約" };
        public const string Language = "語言";
        public static readonly string[] LanguageOptions = { "繁體中文", "English" };

        // 三國半盤 rule options (not implemented yet: no gameplay reads them).
        public const string HalfCrossTeamVariant = "分隊";
        public const string HalfCrossTeamStandard = "第一種（帥將兵卒／仕相俥傌炮／士象車馬包）";
        public const string HalfCrossTeamHandicap = "第二種（讓子用）";
        public const string HalfCrossWinCondition = "勝負方式";
        public const string HalfCrossWinPoints = "計分（預設）";
        public const string HalfCrossWinAnnihilation = "全滅";
        public const string HalfCrossWinRecall = "收軍";
        public const string HalfCrossWinScoreBalance = "得失分";
        public const string HalfCrossWinFirstTo200 = "先得 200 分";

        // Number values.
        public const string MinutesUnit = "分鐘";
        public const string SecondsUnit = "秒";
        public static string Minutes(float value) => $"{value:0} 分鐘";
        public static string Seconds(float value) => $"{value:0} 秒";
        public static string Percent(float value) => $"{value:0}%";
        public static string PlainNumber(float value) => $"{value:0.#}";
    }
}
