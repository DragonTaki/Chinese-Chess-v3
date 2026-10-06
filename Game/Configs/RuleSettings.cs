/* ----- ----- ----- ----- */
// RuleSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// A rule option that is an on/off switch, kept per game kind (<see cref="RuleSettings"/>).
    /// Which kinds have which options: <see cref="RuleSettings.OptionsFor"/>.
    /// </summary>
    public enum RuleOption
    {
        /// <summary>王見王 (<see cref="Rules.CanGeneralSeeGeneral"/>).</summary>
        GeneralCanSeeGeneral,

        /// <summary>將帥出宮 (<see cref="Rules.CanGeneralLeavePalace"/>).</summary>
        GeneralCanLeavePalace,

        /// <summary>士出宮 (<see cref="Rules.CanAdvisorLeavePalace"/>).</summary>
        AdvisorCanLeavePalace,

        /// <summary>塞象眼 (<see cref="Rules.CanElephantEyeBlocked"/>).</summary>
        ElephantEyeBlocks,

        /// <summary>蹩馬腳 (<see cref="Rules.CanHorseLegHobbled"/>).</summary>
        HorseLegBlocks,

        /// <summary>暗吃 (<see cref="Rules.CanCaptureHiddenPiece"/>).</summary>
        CanCaptureHiddenPiece,

        /// <summary>暗吃到更大的子時吃方被吃 (<see cref="Rules.IsCaptureHiddenPieceStrongerSuicide"/>).</summary>
        CaptureHiddenStrongerSuicide,

        /// <summary>連吃 (<see cref="Rules.IsAllowChainCapture"/>).</summary>
        AllowChainCapture,

        /// <summary>車衝馬斜 (<see cref="Rules.IsChariotRushHorseDiagonal"/>).</summary>
        ChariotRushHorseDiagonal,

        /// <summary>包跳吃子 (<see cref="Rules.IsCannonMustJumpToCapture"/>).</summary>
        CannonMustJump,

        /// <summary>吃己棋 (<see cref="Rules.CanCaptureOwnPiece"/>).</summary>
        CaptureOwnPiece,

        /// <summary>自殺 (<see cref="Rules.CanSuicide"/>).</summary>
        Suicide,
    }

    /// <summary>
    /// The player's rule and clock choices for one <see cref="GameKind"/> (one
    /// <c>[rules.*]</c> section of settings.ini). These rules are for local
    /// games only (a network game does not use them). The property initializers are the code
    /// defaults, read from a default <see cref="Rules"/>. Every kind holds every property, but
    /// only its clock settings and its <see cref="OptionsFor"/> options (plus the 三國 choices
    /// for <see cref="GameKind.ThreeKingdoms"/>) are in the file and the menu; the others stay
    /// at their defaults.
    /// </summary>
    public sealed class RuleSettings
    {
        // Default rule set: source of the defaults below.
        private static readonly Rules RuleDefaults = new Rules();

        #region Limits

        /// <summary>Smallest / largest <see cref="TotalTimeMinutes"/>.</summary>
        public const int TotalTimeMinutesMin = 1, TotalTimeMinutesMax = 600;

        /// <summary>Smallest / largest <see cref="StepTimeSeconds"/>.</summary>
        public const int StepTimeSecondsMin = 1, StepTimeSecondsMax = 3600;

        /// <summary>Smallest / largest <see cref="IncrementSeconds"/>.</summary>
        public const int IncrementSecondsMin = 0, IncrementSecondsMax = 600;

        #endregion

        #region Clock

        /// <summary>Each side's total time, in whole minutes (<see cref="TotalTimeMinutesMin"/>-<see cref="TotalTimeMinutesMax"/>); countdown only. Default: from <see cref="Rules.TotalTimeLimit"/> (30)</summary>
        public int TotalTimeMinutes { get; set; } = (int)RuleDefaults.TotalTimeLimit.TotalMinutes;

        /// <summary>Time per move, in whole seconds (<see cref="StepTimeSecondsMin"/>-<see cref="StepTimeSecondsMax"/>); countdown only. Default: from <see cref="Rules.StepTimeLimit"/> (300)</summary>
        public int StepTimeSeconds { get; set; } = (int)RuleDefaults.StepTimeLimit.TotalSeconds;

        /// <summary>Seconds added to a side's total after each of its moves (<see cref="IncrementSecondsMin"/>-<see cref="IncrementSecondsMax"/>); countdown only. Default: from <see cref="Rules.IncrementPerMove"/> (0)</summary>
        public int IncrementSeconds { get; set; } = (int)RuleDefaults.IncrementPerMove.TotalSeconds;

        /// <summary>Whether the per-move limit applies; countdown only. Default: from <see cref="Rules.EnableStepTimer"/> (true)</summary>
        public bool StepTimerEnabled { get; set; } = RuleDefaults.EnableStepTimer;

        /// <summary>Clocks count down to the limits or up from zero (count-up only measures). Default: from <see cref="Rules.TimerMode"/> (CountDown)</summary>
        public TimerMode TimerMode { get; set; } = RuleDefaults.TimerMode;

        /// <summary>Whether a side whose clock runs out loses; countdown only. Default: from <see cref="Rules.EndGameWhenTimesUp"/> (true)</summary>
        public bool EndGameWhenTimesUp { get; set; } = RuleDefaults.EndGameWhenTimesUp;

        #endregion

        #region Full board

        /// <summary>王見王 allowed. Default: from <see cref="Rules.CanGeneralSeeGeneral"/> (false)</summary>
        public bool CanGeneralSeeGeneral { get; set; } = RuleDefaults.CanGeneralSeeGeneral;

        /// <summary>將帥出宮 allowed. Default: from <see cref="Rules.CanGeneralLeavePalace"/> (false)</summary>
        public bool CanGeneralLeavePalace { get; set; } = RuleDefaults.CanGeneralLeavePalace;

        /// <summary>士出宮 allowed. Default: from <see cref="Rules.CanAdvisorLeavePalace"/> (false)</summary>
        public bool CanAdvisorLeavePalace { get; set; } = RuleDefaults.CanAdvisorLeavePalace;

        /// <summary>塞象眼 applies. Default: from <see cref="Rules.CanElephantEyeBlocked"/> (true)</summary>
        public bool ElephantEyeCanBeBlocked { get; set; } = RuleDefaults.CanElephantEyeBlocked;

        /// <summary>蹩馬腳 applies. Default: from <see cref="Rules.CanHorseLegHobbled"/> (true)</summary>
        public bool HorseLegCanBeHobbled { get; set; } = RuleDefaults.CanHorseLegHobbled;

        /// <summary>吃己棋 (Full board and HalfCenter). Default: from <see cref="Rules.CanCaptureOwnPiece"/> (false)</summary>
        public bool CanCaptureOwnPiece { get; set; } = RuleDefaults.CanCaptureOwnPiece;

        #endregion

        #region Half board

        /// <summary>暗吃. Default: from <see cref="Rules.CanCaptureHiddenPiece"/> (false)</summary>
        public bool CanCaptureHiddenPiece { get; set; } = RuleDefaults.CanCaptureHiddenPiece;

        /// <summary>A 暗吃 revealing a stronger target kills the attacker (false: it returns alive). Default: from <see cref="Rules.IsCaptureHiddenPieceStrongerSuicide"/> (true)</summary>
        public bool IsCaptureHiddenPieceStrongerSuicide { get; set; } = RuleDefaults.IsCaptureHiddenPieceStrongerSuicide;

        /// <summary>自殺 (moving onto a stronger face-up enemy piece kills the mover). Default: from <see cref="Rules.CanSuicide"/> (false)</summary>
        public bool CanSuicide { get; set; } = RuleDefaults.CanSuicide;

        /// <summary>連吃. Default: from <see cref="Rules.IsAllowChainCapture"/> (false)</summary>
        public bool IsAllowChainCapture { get; set; } = RuleDefaults.IsAllowChainCapture;

        /// <summary>車衝馬斜 (one variant). Default: from <see cref="Rules.IsChariotRushHorseDiagonal"/> (false)</summary>
        public bool IsChariotRushHorseDiagonal { get; set; } = RuleDefaults.IsChariotRushHorseDiagonal;

        /// <summary>包跳吃子. Default: from <see cref="Rules.IsCannonMustJumpToCapture"/> (true)</summary>
        public bool IsCannonMustJumpToCapture { get; set; } = RuleDefaults.IsCannonMustJumpToCapture;

        #endregion

        #region Three Kingdoms

        /// <summary>三國 勝負方式. Default: from <see cref="Rules.HalfCrossWinCondition"/> (Points)</summary>
        public HalfCrossWinCondition HalfCrossWinCondition { get; set; } = RuleDefaults.HalfCrossWinCondition;

        #endregion

        #region Options per kind

        private static readonly RuleOption[] FullBoardOptions =
        {
            RuleOption.GeneralCanSeeGeneral, RuleOption.GeneralCanLeavePalace, RuleOption.AdvisorCanLeavePalace,
            RuleOption.ElephantEyeBlocks, RuleOption.HorseLegBlocks, RuleOption.CaptureOwnPiece,
        };

        private static readonly RuleOption[] DarkHalfOptions =
        {
            RuleOption.CanCaptureHiddenPiece, RuleOption.CaptureHiddenStrongerSuicide,
            RuleOption.AllowChainCapture, RuleOption.ChariotRushHorseDiagonal, RuleOption.CannonMustJump,
            RuleOption.CaptureOwnPiece, RuleOption.Suicide,
        };

        // 明棋半盤: every piece is face up, so the hidden-capture options do not apply.
        private static readonly RuleOption[] OpenHalfOptions =
        {
            RuleOption.AllowChainCapture, RuleOption.ChariotRushHorseDiagonal, RuleOption.CannonMustJump,
            RuleOption.CaptureOwnPiece, RuleOption.Suicide,
        };

        /// <summary>
        /// The on/off rule options that apply to <paramref name="kind"/>, in menu / file order
        /// (its clock settings always apply too; 三國's choices are separate properties).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a <see cref="GameKind"/>.</exception>
        public static IReadOnlyList<RuleOption> OptionsFor(GameKind kind) => kind switch
        {
            GameKind.Traditional or GameKind.Flip => FullBoardOptions,
            GameKind.DarkHalf => DarkHalfOptions,
            GameKind.OpenHalf => OpenHalfOptions,
            GameKind.ThreeKingdoms => Array.Empty<RuleOption>(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>The value of <paramref name="option"/>.</summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="option"/> is not a <see cref="RuleOption"/>.</exception>
        public bool Get(RuleOption option) => option switch
        {
            RuleOption.GeneralCanSeeGeneral => CanGeneralSeeGeneral,
            RuleOption.GeneralCanLeavePalace => CanGeneralLeavePalace,
            RuleOption.AdvisorCanLeavePalace => CanAdvisorLeavePalace,
            RuleOption.ElephantEyeBlocks => ElephantEyeCanBeBlocked,
            RuleOption.HorseLegBlocks => HorseLegCanBeHobbled,
            RuleOption.CanCaptureHiddenPiece => CanCaptureHiddenPiece,
            RuleOption.CaptureHiddenStrongerSuicide => IsCaptureHiddenPieceStrongerSuicide,
            RuleOption.AllowChainCapture => IsAllowChainCapture,
            RuleOption.ChariotRushHorseDiagonal => IsChariotRushHorseDiagonal,
            RuleOption.CannonMustJump => IsCannonMustJumpToCapture,
            RuleOption.CaptureOwnPiece => CanCaptureOwnPiece,
            RuleOption.Suicide => CanSuicide,
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown rule option"),
        };

        /// <summary>Sets <paramref name="option"/> to <paramref name="value"/>.</summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="option"/> is not a <see cref="RuleOption"/>.</exception>
        public void Set(RuleOption option, bool value)
        {
            switch (option)
            {
                case RuleOption.GeneralCanSeeGeneral: CanGeneralSeeGeneral = value; break;
                case RuleOption.GeneralCanLeavePalace: CanGeneralLeavePalace = value; break;
                case RuleOption.AdvisorCanLeavePalace: CanAdvisorLeavePalace = value; break;
                case RuleOption.ElephantEyeBlocks: ElephantEyeCanBeBlocked = value; break;
                case RuleOption.HorseLegBlocks: HorseLegCanBeHobbled = value; break;
                case RuleOption.CanCaptureHiddenPiece: CanCaptureHiddenPiece = value; break;
                case RuleOption.CaptureHiddenStrongerSuicide: IsCaptureHiddenPieceStrongerSuicide = value; break;
                case RuleOption.AllowChainCapture: IsAllowChainCapture = value; break;
                case RuleOption.ChariotRushHorseDiagonal: IsChariotRushHorseDiagonal = value; break;
                case RuleOption.CannonMustJump: IsCannonMustJumpToCapture = value; break;
                case RuleOption.CaptureOwnPiece: CanCaptureOwnPiece = value; break;
                case RuleOption.Suicide: CanSuicide = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown rule option");
            }
        }

        #endregion

        #region Rules

        /// <summary>The default <see cref="Rules"/> with these choices applied (<see cref="ApplyTo"/>).</summary>
        public Rules CreateRules()
        {
            var rules = new Rules();
            ApplyTo(rules);
            return rules;
        }

        /// <summary>
        /// Sets <paramref name="rules"/>' clock and rule properties from these choices (every
        /// property of this class); the rest - e.g. <see cref="Rules.IsHiddenChess"/>, which the
        /// game kind decides - are left alone. Lets the settings screen update the rules new
        /// games start with without replacing the object; a game already started keeps its own copy.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="rules"/> is null.</exception>
        public void ApplyTo(Rules rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            rules.TotalTimeLimit = TimeSpan.FromMinutes(TotalTimeMinutes);
            rules.StepTimeLimit = TimeSpan.FromSeconds(StepTimeSeconds);
            rules.IncrementPerMove = TimeSpan.FromSeconds(IncrementSeconds);
            rules.EnableStepTimer = StepTimerEnabled;
            rules.TimerMode = TimerMode;
            rules.EndGameWhenTimesUp = EndGameWhenTimesUp;
            rules.CanGeneralSeeGeneral = CanGeneralSeeGeneral;
            rules.CanGeneralLeavePalace = CanGeneralLeavePalace;
            rules.CanAdvisorLeavePalace = CanAdvisorLeavePalace;
            rules.CanElephantEyeBlocked = ElephantEyeCanBeBlocked;
            rules.CanHorseLegHobbled = HorseLegCanBeHobbled;
            rules.CanCaptureOwnPiece = CanCaptureOwnPiece;
            rules.CanSuicide = CanSuicide;
            rules.CanCaptureHiddenPiece = CanCaptureHiddenPiece;
            rules.IsCaptureHiddenPieceStrongerSuicide = IsCaptureHiddenPieceStrongerSuicide;
            rules.IsAllowChainCapture = IsAllowChainCapture;
            rules.IsChariotRushHorseDiagonal = IsChariotRushHorseDiagonal;
            rules.IsCannonMustJumpToCapture = IsCannonMustJumpToCapture;
            rules.HalfCrossWinCondition = HalfCrossWinCondition;
        }

        #endregion
    }
}
