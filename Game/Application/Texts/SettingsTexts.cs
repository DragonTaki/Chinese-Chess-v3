/* ----- ----- ----- ----- */
// SettingsTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Globalization;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;

using Engine.Localization;

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
        public static string SettingsSaveFailed => Lang.Get("settings.settings_save_failed");

        /// <summary>The footer button resetting the shown tab to the defaults.</summary>
        public static string ResetTab => Lang.Get("settings.reset_tab");

        /// <summary>The confirmation before resetting the shown tab.</summary>
        public static string ResetTabToDefaults => Lang.Get("settings.reset_tab_to_defaults");

        /// <summary>A setting's button text: name, then its value.</summary>
        public static string SettingText(string name, string value) => Lang.Get("settings.setting_text", name, value);

        public static string On => Lang.Get("settings.on");
        public static string Off => Lang.Get("settings.off");

        /// <summary>
        /// <c>SettingText</c>, <c>On</c> and <c>Off</c> were the old button texts (<c>名稱：開</c>);
        /// the tabbed screens show a switch instead. Kept, unused.
        /// </summary>
        public static string SectionRules => Lang.Get("settings.section_rules");
        public static string SectionDarkChess => Lang.Get("settings.section_dark_chess");
        public static string SectionTimer => Lang.Get("settings.section_timer");
        public static string SectionHints => Lang.Get("settings.section_hints");
        public static string SectionOther => Lang.Get("settings.section_other");

        public static string GeneralCanSeeGeneral => Lang.Get("settings.general_can_see_general");
        public static string GeneralCanLeavePalace => Lang.Get("settings.general_can_leave_palace");
        public static string AdvisorCanLeavePalace => Lang.Get("settings.advisor_can_leave_palace");
        public static string ElephantEyeBlocks => Lang.Get("settings.elephant_eye_blocks");
        public static string HorseLegBlocks => Lang.Get("settings.horse_leg_blocks");

        public static string HiddenChess => Lang.Get("settings.hidden_chess");
        public static string CanCaptureHiddenPiece => Lang.Get("settings.can_capture_hidden_piece");
        public static string CaptureHiddenStrongerSuicide => Lang.Get("settings.capture_hidden_stronger_suicide");
        public static string AllowChainCapture => Lang.Get("settings.allow_chain_capture");
        public static string ChariotRushHorseDiagonal => Lang.Get("settings.chariot_rush_horse_diagonal");
        public static string CannonMustJump => Lang.Get("settings.cannon_must_jump");
        public static string CaptureOwnPiece => Lang.Get("settings.capture_own_piece");
        public static string Suicide => Lang.Get("settings.suicide");

        public static string StepTimer => Lang.Get("settings.step_timer");
        public static string LoseOnTimeUp => Lang.Get("settings.lose_on_time_up");
        public static string TimerMode => Lang.Get("settings.timer_mode");
        public static string TimerModeCountDown => Lang.Get("settings.timer_mode_count_down");
        public static string TimerModeCountUp => Lang.Get("settings.timer_mode_count_up");
        public static string TimerPreset => Lang.Get("settings.timer_preset");
        public static string TimerPresetCustom => Lang.Get("settings.timer_preset_custom");
        public static string TotalTime => Lang.Get("settings.total_time");
        public static string StepTime => Lang.Get("settings.step_time");
        public static string Increment => Lang.Get("settings.increment");

        /// <summary>The value of a time-limit setting while the clocks count up (正數 only measures time).</summary>
        public static string NotWithCountUp => Lang.Get("settings.not_with_count_up");

        public static string LegalMoveHints => Lang.Get("settings.legal_move_hints");
        public static string HangingPieceHints => Lang.Get("settings.hanging_piece_hints");

        // DEBUG tab (除錯功能): one section per kind of debug feature.
        public static string SectionDebugLog => Lang.Get("settings.section_debug_log");
        public static string SectionDebugVisual => Lang.Get("settings.section_debug_visual");
        public static string SectionDebugPerformance => Lang.Get("settings.section_debug_performance");
        public static string DebugLog => Lang.Get("settings.debug_log");
        public static string ConsoleTrace => Lang.Get("settings.console_trace");
        public static string LabelBackgrounds => Lang.Get("settings.label_backgrounds");
        public static string LayoutOutlines => Lang.Get("settings.layout_outlines");
        public static string StarEffectFrames => Lang.Get("settings.star_effect_frames");
        public static string ShowFps => Lang.Get("settings.show_fps");
        public static string ShowNetworkLatency => Lang.Get("settings.show_network_latency");

        // Tabs of 遊戲設定.
        public static string TabDisplay => Lang.Get("settings.tab_display");
        public static string TabSound => Lang.Get("settings.tab_sound");
        public static string TabGame => Lang.Get("settings.tab_game");
        public static string TabDebug => Lang.Get("settings.tab_debug");

        /// <summary>A rules screen tab: the game kind's name (as on the new-game menu).</summary>
        public static string GameKindName(GameKind kind) => kind switch
        {
            GameKind.Traditional => Lang.Get("game_kind.traditional"),
            GameKind.Flip => Lang.Get("game_kind.flip"),
            GameKind.DarkHalf => Lang.Get("game_kind.dark_half"),
            GameKind.OpenHalf => Lang.Get("game_kind.open_half"),
            GameKind.ThreeKingdoms => Lang.Get("game_kind.three_kingdoms"),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>
        /// A setting that can be changed but does nothing yet: its name with this mark. No longer
        /// shown (the author asked on 2026-10-02 for no 未實作 mark in the settings screens; which
        /// items do nothing is only in the code). Kept, unused.
        /// </summary>
        public static string NotImplemented(string name) => Lang.Get("settings.not_implemented", name);

        /// <summary>A time-limit switch while the clocks count up: its name with <see cref="NotWithCountUp"/>.</summary>
        public static string WithCountUpNote(string name) => Lang.Get("settings.with_count_up_note", name, NotWithCountUp);

        // 畫面 (display; all but the wheel step are not implemented yet).
        public static string Resolution => Lang.Get("settings.resolution");
        public static readonly string[] ResolutionOptions = { "1280 × 720", "1600 × 900", "1920 × 1080", "2560 × 1440" };
        public static string DisplayMode => Lang.Get("settings.display_mode");
        public static string[] DisplayModeOptions => new[] { Lang.Get("settings.display_mode_options.0"), Lang.Get("settings.display_mode_options.1"), Lang.Get("settings.display_mode_options.2") };
        public static string VSync => Lang.Get("settings.vsync");
        public static string Fps => Lang.Get("settings.fps");
        public static readonly string[] FpsOptions = { "30", "60", "120", "144" };
        public static string UiScale => Lang.Get("settings.ui_scale");
        public static string WheelScrollStep => Lang.Get("settings.wheel_scroll_step");

        // 聲音 (sound; not implemented yet: there is no sound system).
        public static string MasterVolume => Lang.Get("settings.master_volume");
        public static string MusicVolume => Lang.Get("settings.music_volume");
        public static string EffectsVolume => Lang.Get("settings.effects_volume");
        public static string Mute => Lang.Get("settings.mute");

        // 遊戲 (game).
        public static string PlayerName => Lang.Get("settings.player_name");
        public static string PlayerNamePlaceholder => Lang.Get("settings.player_name_placeholder");
        public static string Player1Name => Lang.Get("settings.player1_name");
        public static string Player2Name => Lang.Get("settings.player2_name");
        public static string Player3Name => Lang.Get("settings.player3_name");

        public static string MoveAnimationSpeed => Lang.Get("settings.move_animation_speed");
        public static string[] MoveAnimationSpeedOptions => new[] { Lang.Get("settings.move_animation_speed_options.0"), Lang.Get("settings.move_animation_speed_options.1"), Lang.Get("settings.move_animation_speed_options.2"), Lang.Get("settings.move_animation_speed_options.3") };
        public static string BoardStyle => Lang.Get("settings.board_style");
        public static string[] BoardStyleOptions => new[] { Lang.Get("settings.board_style_options.0"), Lang.Get("settings.board_style_options.1"), Lang.Get("settings.board_style_options.2") };
        public static string PieceStyle => Lang.Get("settings.piece_style");
        public static string[] PieceStyleOptions => new[] { Lang.Get("settings.piece_style_options.0"), Lang.Get("settings.piece_style_options.1"), Lang.Get("settings.piece_style_options.2") };
        public static string Language => Lang.Get("settings.language");
        public static string[] LanguageOptions => new[] { Lang.Get("settings.language_options.0"), Lang.Get("settings.language_options.1") };

        // 三國半盤 rule options.

        /// <summary>A 自訂分隊 menu row: the piece's colour and character, e.g. 紅俥 隊伍.</summary>
        public static string TeamOf(PieceColor color, PieceType type) =>
            Lang.Get("settings.team_of", Lang.Get(color == PieceColor.Red ? "color.red" : "color.black"), PieceConstants.GetPieceText(type, color));
        public static string HalfCrossWinCondition => Lang.Get("settings.half_cross_win_condition");
        public static string HalfCrossWinPoints => Lang.Get("settings.half_cross_win_points");
        public static string HalfCrossWinAnnihilation => Lang.Get("settings.half_cross_win_annihilation");
        public static string HalfCrossWinScoreBalance => Lang.Get("settings.half_cross_win_score_balance");
        public static string HalfCrossWinFirstTo200 => Lang.Get("settings.half_cross_win_first_to200");

        // Number values.
        public static string MinutesUnit => Lang.Get("settings.minutes_unit");
        public static string SecondsUnit => Lang.Get("settings.seconds_unit");
        public static string Minutes(float value) => Lang.Get("settings.value.minutes", value.ToString("0", CultureInfo.InvariantCulture));
        public static string Seconds(float value) => Lang.Get("settings.value.seconds", value.ToString("0", CultureInfo.InvariantCulture));
        public static string Percent(float value) => Lang.Get("settings.value.percent", value.ToString("0", CultureInfo.InvariantCulture));
        public static string PlainNumber(float value) => $"{value:0.#}";
    }
}
