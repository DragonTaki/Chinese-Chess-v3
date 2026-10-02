/* ----- ----- ----- ----- */
// SettingsMenuContent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>Which settings a <see cref="UISettingsMenu"/> lists.</summary>
    public enum SettingsMenuScope
    {
        /// <summary>Every setting the menu offers (遊戲設定).</summary>
        All,

        /// <summary>Only the rule sections (規則設定).</summary>
        Rules,
    }

    /// <summary>
    /// One setting's button: its label, the text of its current value, and what a click does
    /// (flip the bool / go to the next choice). Edits the <see cref="PlayerSettings"/> it is given.
    /// </summary>
    public sealed class SettingsMenuItem
    {
        public SettingsMenuItem(string name, Func<PlayerSettings, string> value, Action<PlayerSettings> change)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Change = change ?? throw new ArgumentNullException(nameof(change));
        }

        /// <summary>The setting's name (the left part of its button text).</summary>
        public string Name { get; }

        /// <summary>The text of the setting's current value in a settings instance.</summary>
        public Func<PlayerSettings, string> Value { get; }

        /// <summary>Toggles the setting / selects the next choice in a settings instance.</summary>
        public Action<PlayerSettings> Change { get; }

        /// <summary>The button text for <paramref name="settings"/>: <c>名稱：開</c>.</summary>
        public string Text(PlayerSettings settings) => GameMenuTexts.SettingText(Name, Value(settings));
    }

    /// <summary>A titled group of settings.</summary>
    public sealed class SettingsMenuSection
    {
        public SettingsMenuSection(string header, IReadOnlyList<SettingsMenuItem> items, bool isRules)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            IsRules = isRules;
        }

        /// <summary>The header text (also says when the section takes effect).</summary>
        public string Header { get; }

        public IReadOnlyList<SettingsMenuItem> Items { get; }

        /// <summary>Whether the section is a rule section (listed by <see cref="SettingsMenuScope.Rules"/> too).</summary>
        public bool IsRules { get; }
    }

    /// <summary>
    /// The settings the menu offers (docs/SETTINGS.md): the bools and choices of
    /// <see cref="PlayerSettings"/>; numbers, texts and folders are only edited in
    /// <c>settings.ini</c>.
    /// </summary>
    public static class SettingsMenuContent
    {
        private static SettingsMenuItem Toggle(string name, Func<PlayerSettings, bool> get, Action<PlayerSettings, bool> set) =>
            new(name, s => get(s) ? GameMenuTexts.On : GameMenuTexts.Off, s => set(s, !get(s)));

        /// <summary>
        /// A <see cref="Toggle"/> that only means something with countdown clocks (a time limit or
        /// what happens at it): with count-up clocks (正數, only measuring time) it shows
        /// <see cref="GameMenuTexts.NotWithCountUp"/> and a click changes nothing.
        /// </summary>
        private static SettingsMenuItem CountDownToggle(string name, Func<PlayerSettings, bool> get, Action<PlayerSettings, bool> set) =>
            new(name,
                s => s.TimerMode == TimerMode.CountUp ? GameMenuTexts.NotWithCountUp : (get(s) ? GameMenuTexts.On : GameMenuTexts.Off),
                s =>
                {
                    if (s.TimerMode != TimerMode.CountUp)
                        set(s, !get(s));
                });

        /// <summary>All sections, in menu order.</summary>
        public static IReadOnlyList<SettingsMenuSection> Sections { get; } = new[]
        {
            new SettingsMenuSection(GameMenuTexts.SectionRules, new[]
            {
                Toggle(GameMenuTexts.GeneralCanSeeGeneral, s => s.CanGeneralSeeGeneral, (s, v) => s.CanGeneralSeeGeneral = v),
                Toggle(GameMenuTexts.GeneralCanLeavePalace, s => s.CanGeneralLeavePalace, (s, v) => s.CanGeneralLeavePalace = v),
                Toggle(GameMenuTexts.AdvisorCanLeavePalace, s => s.CanAdvisorLeavePalace, (s, v) => s.CanAdvisorLeavePalace = v),
                Toggle(GameMenuTexts.ElephantEyeBlocks, s => s.ElephantEyeCanBeBlocked, (s, v) => s.ElephantEyeCanBeBlocked = v),
                Toggle(GameMenuTexts.HorseLegBlocks, s => s.HorseLegCanBeHobbled, (s, v) => s.HorseLegCanBeHobbled = v),
            }, isRules: true),

            new SettingsMenuSection(GameMenuTexts.SectionDarkChess, new[]
            {
                Toggle(GameMenuTexts.HiddenChess, s => s.IsHiddenChess, (s, v) => s.IsHiddenChess = v),
                Toggle(GameMenuTexts.CanCaptureHiddenPiece, s => s.CanCaptureHiddenPiece, (s, v) => s.CanCaptureHiddenPiece = v),
                Toggle(GameMenuTexts.CaptureHiddenStrongerSuicide, s => s.IsCaptureHiddenPieceStrongerSuicide, (s, v) => s.IsCaptureHiddenPieceStrongerSuicide = v),
                Toggle(GameMenuTexts.AllowChainCapture, s => s.IsAllowChainCapture, (s, v) => s.IsAllowChainCapture = v),
                Toggle(GameMenuTexts.ChariotRushHorseDiagonal, s => s.IsChariotRushHorseDiagonal, (s, v) => s.IsChariotRushHorseDiagonal = v),
                Toggle(GameMenuTexts.CannonMustJump, s => s.IsCannonMustJumpToCapture, (s, v) => s.IsCannonMustJumpToCapture = v),
            }, isRules: true),

            new SettingsMenuSection(GameMenuTexts.SectionTimer, new[]
            {
                CountDownToggle(GameMenuTexts.StepTimer, s => s.StepTimerEnabled, (s, v) => s.StepTimerEnabled = v),
                CountDownToggle(GameMenuTexts.LoseOnTimeUp, s => s.EndGameWhenTimesUp, (s, v) => s.EndGameWhenTimesUp = v),
                new SettingsMenuItem(GameMenuTexts.TimerMode,
                    s => s.TimerMode == TimerMode.CountDown ? GameMenuTexts.TimerModeCountDown : GameMenuTexts.TimerModeCountUp,
                    s => s.TimerMode = s.TimerMode == TimerMode.CountDown ? TimerMode.CountUp : TimerMode.CountDown),
            }, isRules: false),

            new SettingsMenuSection(GameMenuTexts.SectionHints, new[]
            {
                Toggle(GameMenuTexts.LegalMoveHints, s => s.ShowLegalMoveHints, (s, v) => s.ShowLegalMoveHints = v),
                Toggle(GameMenuTexts.HangingPieceHints, s => s.ShowHangingPieceHints, (s, v) => s.ShowHangingPieceHints = v),
            }, isRules: false),

            new SettingsMenuSection(GameMenuTexts.SectionOther, new[]
            {
                Toggle(GameMenuTexts.DebugLog, s => s.ShowDebugLog, (s, v) => s.ShowDebugLog = v),
            }, isRules: false),
        };
    }
}
