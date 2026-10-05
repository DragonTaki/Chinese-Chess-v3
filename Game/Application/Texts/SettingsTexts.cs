/* ----- ----- ----- ----- */
// SettingsTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts of the settings screens (遊戲設定 / 單機規則設定) used by the logic layer: the tabs,
    /// section headers, setting names, choices and value formats the screens' content lists
    /// (<c>SettingsMenuContent</c>), the footer button, and the messages of the settings screen model.
    /// </summary>
    public static class SettingsTexts
    {
        /// <summary>The save failed (details are in the log).</summary>
        public const string SettingsSaveFailed = "設定檔寫入失敗，請查看紀錄。";

        /// <summary>The footer button resetting the shown tab to the defaults.</summary>
        public const string ResetTab = "恢復初始";

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
        public const string TimerPreset = "計時預設";
        public const string TimerPresetCustom = "自訂";
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
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>
        /// A setting that can be changed but does nothing yet: its name with this mark. No longer
        /// shown (the author asked on 2026-10-02 for no 未實作 mark in the settings screens; which
        /// items do nothing is only in the code). Kept, unused.
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
