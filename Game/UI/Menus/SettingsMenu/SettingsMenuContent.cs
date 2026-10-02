/* ----- ----- ----- ----- */
// SettingsMenuContent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/02
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>Which settings screen a <see cref="UISettingsMenu"/> is (both are opened from the main menu).</summary>
    public enum SettingsScreen
    {
        /// <summary>遊戲設定: tabs 畫面 / 聲音 / 遊戲.</summary>
        Game,

        /// <summary>單機規則設定: one tab per game kind (local games only; a network game does not use these rules).</summary>
        Rules,
    }

    /// <summary>What control a setting's row has (the right side of the row).</summary>
    public enum SettingsItemKind
    {
        /// <summary>On/off: a switch (<see cref="SettingsToggleItem"/>).</summary>
        Toggle,

        /// <summary>One of a few named choices: a button that steps to the next one for now; a dropdown later (<see cref="SettingsChoiceItem"/>).</summary>
        Choice,

        /// <summary>A number in a range: its value shown for now; a slider later (<see cref="SettingsNumberItem"/>).</summary>
        Number,

        /// <summary>A line of text: a text field (<see cref="SettingsTextItem"/>).</summary>
        Text,
    }

    /// <summary>
    /// One setting's row: its name and how its control reads and edits the value. The value
    /// lives in the <see cref="PlayerSettings"/> given to the accessors - except for settings
    /// that are not implemented yet (<see cref="IsImplemented"/> false), whose value is only
    /// kept for this session (<see cref="UnimplementedSettings"/>), not saved, and does nothing.
    /// </summary>
    public abstract class SettingsMenuItem
    {
        protected SettingsMenuItem(string name, bool isImplemented)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsImplemented = isImplemented;
        }

        /// <summary>The setting's name (the row's left side).</summary>
        public string Name { get; }

        /// <summary>Whether changing the setting does something yet (false: 未實作; not shown on screen, see <see cref="RowText"/>).</summary>
        public bool IsImplemented { get; }

        /// <summary>The kind of control the row has.</summary>
        public abstract SettingsItemKind Kind { get; }

        /// <summary>
        /// The row's text in <paramref name="settings"/>: the name. A setting that is not
        /// implemented is not marked (the author's choice, 2026-10-02); <see cref="IsImplemented"/>
        /// still says which ones do nothing.
        /// </summary>
        public virtual string RowText(PlayerSettings settings) => Name;
    }

    /// <summary>An on/off setting (a switch).</summary>
    public sealed class SettingsToggleItem : SettingsMenuItem
    {
        private readonly Func<PlayerSettings, bool> _isAvailable;
        private readonly Func<string, string> _unavailableText;

        /// <param name="name">The setting's name.</param>
        /// <param name="get">Reads the value.</param>
        /// <param name="set">Writes the value.</param>
        /// <param name="isAvailable">Whether the switch can be used in a settings instance (null: always); an unavailable switch is shown disabled.</param>
        /// <param name="unavailableText">The row text while unavailable, from the name (null: the name).</param>
        /// <param name="isImplemented">See <see cref="SettingsMenuItem.IsImplemented"/>.</param>
        public SettingsToggleItem(string name, Func<PlayerSettings, bool> get, Action<PlayerSettings, bool> set,
            Func<PlayerSettings, bool> isAvailable = null, Func<string, string> unavailableText = null, bool isImplemented = true)
            : base(name, isImplemented)
        {
            Get = get ?? throw new ArgumentNullException(nameof(get));
            Set = set ?? throw new ArgumentNullException(nameof(set));
            _isAvailable = isAvailable;
            _unavailableText = unavailableText;
        }

        public override SettingsItemKind Kind => SettingsItemKind.Toggle;

        /// <summary>Reads the value.</summary>
        public Func<PlayerSettings, bool> Get { get; }

        /// <summary>Writes the value.</summary>
        public Action<PlayerSettings, bool> Set { get; }

        /// <summary>Whether the switch can be used in <paramref name="settings"/> (e.g. a time-limit switch only with countdown clocks).</summary>
        public bool IsAvailable(PlayerSettings settings) => _isAvailable?.Invoke(settings) ?? true;

        public override string RowText(PlayerSettings settings) =>
            !IsAvailable(settings) && _unavailableText != null ? _unavailableText(base.RowText(settings)) : base.RowText(settings);
    }

    /// <summary>A setting that is one of a few named choices.</summary>
    public sealed class SettingsChoiceItem : SettingsMenuItem
    {
        /// <param name="name">The setting's name.</param>
        /// <param name="options">The choices' texts, in order (at least one).</param>
        /// <param name="getIndex">Reads the chosen index.</param>
        /// <param name="setIndex">Writes the chosen index.</param>
        /// <param name="isImplemented">See <see cref="SettingsMenuItem.IsImplemented"/>.</param>
        public SettingsChoiceItem(string name, IReadOnlyList<string> options, Func<PlayerSettings, int> getIndex, Action<PlayerSettings, int> setIndex,
            bool isImplemented = true)
            : base(name, isImplemented)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.Count == 0)
                throw new ArgumentException("A choice needs at least one option.", nameof(options));
            GetIndex = getIndex ?? throw new ArgumentNullException(nameof(getIndex));
            SetIndex = setIndex ?? throw new ArgumentNullException(nameof(setIndex));
        }

        public override SettingsItemKind Kind => SettingsItemKind.Choice;

        /// <summary>The choices' texts, in order.</summary>
        public IReadOnlyList<string> Options { get; }

        /// <summary>Reads the chosen index.</summary>
        public Func<PlayerSettings, int> GetIndex { get; }

        /// <summary>Writes the chosen index.</summary>
        public Action<PlayerSettings, int> SetIndex { get; }

        /// <summary>The chosen option's text in <paramref name="settings"/>.</summary>
        public string ValueText(PlayerSettings settings) => Options[Math.Clamp(GetIndex(settings), 0, Options.Count - 1)];

        /// <summary>Chooses the next option (after the last: the first).</summary>
        public void Cycle(PlayerSettings settings) => SetIndex(settings, (GetIndex(settings) + 1) % Options.Count);
    }

    /// <summary>A number setting in a range.</summary>
    public sealed class SettingsNumberItem : SettingsMenuItem
    {
        private readonly Func<float, string> _format;

        /// <param name="name">The setting's name.</param>
        /// <param name="get">Reads the value.</param>
        /// <param name="set">Writes the value (given within the range, on a step).</param>
        /// <param name="min">Smallest value.</param>
        /// <param name="max">Largest value.</param>
        /// <param name="step">Distance between two values a control offers.</param>
        /// <param name="format">The value's text (e.g. with its unit).</param>
        /// <param name="isImplemented">See <see cref="SettingsMenuItem.IsImplemented"/>.</param>
        public SettingsNumberItem(string name, Func<PlayerSettings, float> get, Action<PlayerSettings, float> set,
            float min, float max, float step, Func<float, string> format, bool isImplemented = true)
            : base(name, isImplemented)
        {
            if (!(min <= max))
                throw new ArgumentException($"min {min} is above max {max}.", nameof(min));
            if (!(step > 0f))
                throw new ArgumentOutOfRangeException(nameof(step), step, "The step must be positive.");
            Get = get ?? throw new ArgumentNullException(nameof(get));
            Set = set ?? throw new ArgumentNullException(nameof(set));
            Min = min;
            Max = max;
            Step = step;
            _format = format ?? throw new ArgumentNullException(nameof(format));
        }

        public override SettingsItemKind Kind => SettingsItemKind.Number;

        /// <summary>Reads the value.</summary>
        public Func<PlayerSettings, float> Get { get; }

        /// <summary>Writes the value.</summary>
        public Action<PlayerSettings, float> Set { get; }

        /// <summary>Smallest value.</summary>
        public float Min { get; }

        /// <summary>Largest value.</summary>
        public float Max { get; }

        /// <summary>Distance between two values a control offers.</summary>
        public float Step { get; }

        /// <summary>The value's text in <paramref name="settings"/>.</summary>
        public string ValueText(PlayerSettings settings) => _format(Get(settings));
    }

    /// <summary>A text setting (one line).</summary>
    public sealed class SettingsTextItem : SettingsMenuItem
    {
        /// <param name="name">The setting's name.</param>
        /// <param name="get">Reads the value.</param>
        /// <param name="set">Writes the value (at most <paramref name="maxLength"/> characters).</param>
        /// <param name="maxLength">Longest value.</param>
        /// <param name="placeholder">Shown while the value is empty.</param>
        /// <param name="isImplemented">See <see cref="SettingsMenuItem.IsImplemented"/>.</param>
        public SettingsTextItem(string name, Func<PlayerSettings, string> get, Action<PlayerSettings, string> set, int maxLength,
            string placeholder = "", bool isImplemented = true)
            : base(name, isImplemented)
        {
            Get = get ?? throw new ArgumentNullException(nameof(get));
            Set = set ?? throw new ArgumentNullException(nameof(set));
            MaxLength = maxLength >= 0 ? maxLength : throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "Cannot be negative.");
            Placeholder = placeholder ?? string.Empty;
        }

        public override SettingsItemKind Kind => SettingsItemKind.Text;

        /// <summary>Reads the value.</summary>
        public Func<PlayerSettings, string> Get { get; }

        /// <summary>Writes the value.</summary>
        public Action<PlayerSettings, string> Set { get; }

        /// <summary>Longest value.</summary>
        public int MaxLength { get; }

        /// <summary>Shown while the value is empty.</summary>
        public string Placeholder { get; }
    }

    /// <summary>A group of settings under an optional header.</summary>
    public sealed class SettingsMenuSection
    {
        public SettingsMenuSection(string header, IReadOnlyList<SettingsMenuItem> items)
        {
            Header = header;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>The header text (also says when the settings take effect); null for no header.</summary>
        public string Header { get; }

        public IReadOnlyList<SettingsMenuItem> Items { get; }
    }

    /// <summary>One tab of a settings screen: its title and sections.</summary>
    public sealed class SettingsMenuPage
    {
        public SettingsMenuPage(string title, IReadOnlyList<SettingsMenuSection> sections)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        }

        /// <summary>The tab's text.</summary>
        public string Title { get; }

        public IReadOnlyList<SettingsMenuSection> Sections { get; }
    }

    /// <summary>
    /// Values of the settings that are not implemented yet (未實作): kept for this session only
    /// (never saved, not part of "unsaved changes") so their controls work; nothing reads them.
    /// Defaults are what a later implementation would most likely start with.
    /// </summary>
    public static class UnimplementedSettings
    {
        public static int ResolutionIndex = 2;    // 1920 × 1080
        public static int DisplayModeIndex = 0;   // 視窗
        public static bool VSync = true;
        public static int FpsIndex = 1;           // 60
        public static float UiScalePercent = 100f;
        public static float MasterVolume = 100f;
        public static float MusicVolume = 80f;
        public static float EffectsVolume = 80f;
        public static bool Mute = false;
        public static int MoveAnimationSpeedIndex = 1;  // 普通
        public static int BoardStyleIndex = 0;
        public static int PieceStyleIndex = 0;
        public static int LanguageIndex = 0;      // 繁體中文
    }

    /// <summary>
    /// What the two settings screens list (docs/SETTINGS.md §4): 遊戲設定 - the general
    /// options of <see cref="PlayerSettings"/> plus the usual game options not implemented yet;
    /// 單機規則設定 - per game kind its rule options (<see cref="RuleSettings.OptionsFor"/>, the
    /// 三國 choices) and clock settings. Folders are only edited in <c>settings.ini</c>.
    /// </summary>
    public static class SettingsMenuContent
    {
        #region Ranges of the not-implemented numbers

        private const float UiScaleMin = 50f, UiScaleMax = 200f, UiScaleStep = 10f;
        private const float VolumeMin = 0f, VolumeMax = 100f, VolumeStep = 5f;

        #endregion

        /// <summary>The tabs of <paramref name="screen"/>, in order.</summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="screen"/> is not a <see cref="SettingsScreen"/>.</exception>
        public static IReadOnlyList<SettingsMenuPage> PagesFor(SettingsScreen screen) => screen switch
        {
            SettingsScreen.Game => GamePages,
            SettingsScreen.Rules => RulePages,
            _ => throw new ArgumentOutOfRangeException(nameof(screen), screen, "Unknown settings screen"),
        };

        #region 遊戲設定

        private static SettingsMenuSection Section(params SettingsMenuItem[] items) => new(null, items);

        private static SettingsChoiceItem Placeholder(string name, string[] options, Func<int> get, Action<int> set) =>
            new(name, options, _ => get(), (_, v) => set(v), isImplemented: false);

        private static SettingsNumberItem Placeholder(string name, float min, float max, float step, Func<float, string> format, Func<float> get, Action<float> set) =>
            new(name, _ => get(), (_, v) => set(v), min, max, step, format, isImplemented: false);

        private static SettingsToggleItem Placeholder(string name, Func<bool> get, Action<bool> set) =>
            new(name, _ => get(), (_, v) => set(v), isImplemented: false);

        /// <summary>遊戲設定's tabs: 畫面, 聲音, 遊戲.</summary>
        public static IReadOnlyList<SettingsMenuPage> GamePages { get; } = new[]
        {
            new SettingsMenuPage(GameMenuTexts.TabDisplay, new[]
            {
                Section(
                    Placeholder(GameMenuTexts.Resolution, GameMenuTexts.ResolutionOptions,
                        () => UnimplementedSettings.ResolutionIndex, v => UnimplementedSettings.ResolutionIndex = v),
                    Placeholder(GameMenuTexts.DisplayMode, GameMenuTexts.DisplayModeOptions,
                        () => UnimplementedSettings.DisplayModeIndex, v => UnimplementedSettings.DisplayModeIndex = v),
                    Placeholder(GameMenuTexts.VSync, () => UnimplementedSettings.VSync, v => UnimplementedSettings.VSync = v),
                    Placeholder(GameMenuTexts.Fps, GameMenuTexts.FpsOptions,
                        () => UnimplementedSettings.FpsIndex, v => UnimplementedSettings.FpsIndex = v),
                    Placeholder(GameMenuTexts.UiScale, UiScaleMin, UiScaleMax, UiScaleStep, GameMenuTexts.Percent,
                        () => UnimplementedSettings.UiScalePercent, v => UnimplementedSettings.UiScalePercent = v),
                    new SettingsNumberItem(GameMenuTexts.WheelScrollStep, s => s.WheelScrollStep, (s, v) => s.WheelScrollStep = v,
                        PlayerSettings.WheelScrollStepMin, PlayerSettings.WheelScrollStepMax, 1f, GameMenuTexts.PlainNumber)),
            }),

            new SettingsMenuPage(GameMenuTexts.TabSound, new[]
            {
                Section(
                    Placeholder(GameMenuTexts.MasterVolume, VolumeMin, VolumeMax, VolumeStep, GameMenuTexts.PlainNumber,
                        () => UnimplementedSettings.MasterVolume, v => UnimplementedSettings.MasterVolume = v),
                    Placeholder(GameMenuTexts.MusicVolume, VolumeMin, VolumeMax, VolumeStep, GameMenuTexts.PlainNumber,
                        () => UnimplementedSettings.MusicVolume, v => UnimplementedSettings.MusicVolume = v),
                    Placeholder(GameMenuTexts.EffectsVolume, VolumeMin, VolumeMax, VolumeStep, GameMenuTexts.PlainNumber,
                        () => UnimplementedSettings.EffectsVolume, v => UnimplementedSettings.EffectsVolume = v),
                    Placeholder(GameMenuTexts.Mute, () => UnimplementedSettings.Mute, v => UnimplementedSettings.Mute = v)),
            }),

            new SettingsMenuPage(GameMenuTexts.TabGame, new[]
            {
                Section(
                    new SettingsTextItem(GameMenuTexts.PlayerName, s => s.PlayerName, (s, v) => s.PlayerName = v,
                        PlayerSettings.PlayerNameMaxLength, GameMenuTexts.PlayerNamePlaceholder),
                    new SettingsToggleItem(GameMenuTexts.LegalMoveHints, s => s.ShowLegalMoveHints, (s, v) => s.ShowLegalMoveHints = v),
                    new SettingsToggleItem(GameMenuTexts.HangingPieceHints, s => s.ShowHangingPieceHints, (s, v) => s.ShowHangingPieceHints = v),
                    Placeholder(GameMenuTexts.MoveAnimationSpeed, GameMenuTexts.MoveAnimationSpeedOptions,
                        () => UnimplementedSettings.MoveAnimationSpeedIndex, v => UnimplementedSettings.MoveAnimationSpeedIndex = v),
                    Placeholder(GameMenuTexts.BoardStyle, GameMenuTexts.BoardStyleOptions,
                        () => UnimplementedSettings.BoardStyleIndex, v => UnimplementedSettings.BoardStyleIndex = v),
                    Placeholder(GameMenuTexts.PieceStyle, GameMenuTexts.PieceStyleOptions,
                        () => UnimplementedSettings.PieceStyleIndex, v => UnimplementedSettings.PieceStyleIndex = v),
                    Placeholder(GameMenuTexts.Language, GameMenuTexts.LanguageOptions,
                        () => UnimplementedSettings.LanguageIndex, v => UnimplementedSettings.LanguageIndex = v),
                    new SettingsToggleItem(GameMenuTexts.DebugLog, s => s.ShowDebugLog, (s, v) => s.ShowDebugLog = v)),
            }),
        };

        #endregion

        #region 單機規則設定

        /// <summary>The name of an on/off rule option.</summary>
        private static string OptionName(RuleOption option) => option switch
        {
            RuleOption.GeneralCanSeeGeneral => GameMenuTexts.GeneralCanSeeGeneral,
            RuleOption.GeneralCanLeavePalace => GameMenuTexts.GeneralCanLeavePalace,
            RuleOption.AdvisorCanLeavePalace => GameMenuTexts.AdvisorCanLeavePalace,
            RuleOption.ElephantEyeBlocks => GameMenuTexts.ElephantEyeBlocks,
            RuleOption.HorseLegBlocks => GameMenuTexts.HorseLegBlocks,
            RuleOption.CanCaptureHiddenPiece => GameMenuTexts.CanCaptureHiddenPiece,
            RuleOption.CaptureHiddenStrongerSuicide => GameMenuTexts.CaptureHiddenStrongerSuicide,
            RuleOption.AllowChainCapture => GameMenuTexts.AllowChainCapture,
            RuleOption.ChariotRushHorseDiagonal => GameMenuTexts.ChariotRushHorseDiagonal,
            RuleOption.CannonMustJump => GameMenuTexts.CannonMustJump,
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown rule option"),
        };

        private static readonly TimerMode[] TimerModes = { TimerMode.CountDown, TimerMode.CountUp };
        private static readonly string[] TimerModeTexts = { GameMenuTexts.TimerModeCountDown, GameMenuTexts.TimerModeCountUp };

        private static readonly HalfCrossTeamVariant[] TeamVariants = { HalfCrossTeamVariant.Standard, HalfCrossTeamVariant.Handicap };
        private static readonly string[] TeamVariantTexts = { GameMenuTexts.HalfCrossTeamStandard, GameMenuTexts.HalfCrossTeamHandicap };

        private static readonly HalfCrossWinCondition[] WinConditions =
        {
            HalfCrossWinCondition.Points, HalfCrossWinCondition.Annihilation, HalfCrossWinCondition.Recall,
            HalfCrossWinCondition.ScoreBalance, HalfCrossWinCondition.FirstTo200,
        };
        private static readonly string[] WinConditionTexts =
        {
            GameMenuTexts.HalfCrossWinPoints, GameMenuTexts.HalfCrossWinAnnihilation, GameMenuTexts.HalfCrossWinRecall,
            GameMenuTexts.HalfCrossWinScoreBalance, GameMenuTexts.HalfCrossWinFirstTo200,
        };

        /// <summary>A choice over <paramref name="values"/> (an enum setting), shown as <paramref name="texts"/>.</summary>
        private static SettingsChoiceItem EnumChoice<T>(string name, T[] values, string[] texts, Func<PlayerSettings, T> get, Action<PlayerSettings, T> set,
            bool isImplemented = true) where T : struct, Enum =>
            new(name, texts, s => Math.Max(0, Array.IndexOf(values, get(s))), (s, i) => set(s, values[i]), isImplemented);

        /// <summary>
        /// A switch that only means something with countdown clocks: with count-up clocks
        /// (正數, only measuring time) it is disabled and its row says 正數不限時.
        /// </summary>
        private static SettingsToggleItem CountDownToggle(string name, Func<PlayerSettings, RuleSettings> rules,
            Func<RuleSettings, bool> get, Action<RuleSettings, bool> set) =>
            new(name, s => get(rules(s)), (s, v) => set(rules(s), v),
                isAvailable: s => rules(s).TimerMode != TimerMode.CountUp, unavailableText: GameMenuTexts.WithCountUpNote);

        /// <summary>One game kind's tab: its rule options, then its clock settings.</summary>
        private static SettingsMenuPage RulePage(GameKind kind)
        {
            RuleSettings R(PlayerSettings s) => s.RulesFor(kind);

            var rules = RuleSettings.OptionsFor(kind)
                .Select(option => (SettingsMenuItem)new SettingsToggleItem(OptionName(option), s => R(s).Get(option), (s, v) => R(s).Set(option, v)))
                .ToList();
            if (kind == GameKind.ThreeKingdoms)
            {
                rules.Add(EnumChoice(GameMenuTexts.HalfCrossTeamVariant, TeamVariants, TeamVariantTexts,
                    s => R(s).HalfCrossTeamVariant, (s, v) => R(s).HalfCrossTeamVariant = v, isImplemented: false));
                rules.Add(EnumChoice(GameMenuTexts.HalfCrossWinCondition, WinConditions, WinConditionTexts,
                    s => R(s).HalfCrossWinCondition, (s, v) => R(s).HalfCrossWinCondition = v, isImplemented: false));
            }

            var timer = new SettingsMenuItem[]
            {
                EnumChoice(GameMenuTexts.TimerMode, TimerModes, TimerModeTexts, s => R(s).TimerMode, (s, v) => R(s).TimerMode = v),
                new SettingsNumberItem(GameMenuTexts.TotalTime, s => R(s).TotalTimeMinutes, (s, v) => R(s).TotalTimeMinutes = (int)MathF.Round(v),
                    RuleSettings.TotalTimeMinutesMin, RuleSettings.TotalTimeMinutesMax, 1f, GameMenuTexts.Minutes),
                new SettingsNumberItem(GameMenuTexts.StepTime, s => R(s).StepTimeSeconds, (s, v) => R(s).StepTimeSeconds = (int)MathF.Round(v),
                    RuleSettings.StepTimeSecondsMin, RuleSettings.StepTimeSecondsMax, 1f, GameMenuTexts.Seconds),
                new SettingsNumberItem(GameMenuTexts.Increment, s => R(s).IncrementSeconds, (s, v) => R(s).IncrementSeconds = (int)MathF.Round(v),
                    RuleSettings.IncrementSecondsMin, RuleSettings.IncrementSecondsMax, 1f, GameMenuTexts.Seconds),
                CountDownToggle(GameMenuTexts.StepTimer, R, r => r.StepTimerEnabled, (r, v) => r.StepTimerEnabled = v),
                CountDownToggle(GameMenuTexts.LoseOnTimeUp, R, r => r.EndGameWhenTimesUp, (r, v) => r.EndGameWhenTimesUp = v),
            };

            var sections = new List<SettingsMenuSection>();
            if (rules.Count > 0)
                sections.Add(new SettingsMenuSection(GameMenuTexts.SectionRules, rules));
            sections.Add(new SettingsMenuSection(GameMenuTexts.SectionTimer, timer));
            return new SettingsMenuPage(GameMenuTexts.GameKindName(kind), sections);
        }

        /// <summary>單機規則設定's tabs: one per game kind, in new-game menu order.</summary>
        public static IReadOnlyList<SettingsMenuPage> RulePages { get; } =
            Enum.GetValues<GameKind>().Select(RulePage).ToArray();

        #endregion
    }
}
