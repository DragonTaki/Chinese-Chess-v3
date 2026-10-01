/* ----- ----- ----- ----- */
// PlayerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings the player chooses (what a future in-game settings screen exposes).
    /// The property initializers ARE the code defaults - the one place they are defined
    /// (rule and clock defaults are read from a default <see cref="Rules"/>, which owns
    /// what those rules mean). The values in use are loaded from the player's
    /// <c>settings.ini</c> by <see cref="PlayerSettingsFile"/>; the launchers register the
    /// loaded instance in DI. Every key, default and meaning: docs/SETTINGS.md.
    /// </summary>
    public sealed class PlayerSettings
    {
        // Default rule set: source of the rule / clock defaults below.
        private static readonly Rules RuleDefaults = new Rules();

        /// <summary>A new instance holding only the code defaults.</summary>
        public static PlayerSettings Defaults => new PlayerSettings();

        #region [player]

        /// <summary>Name shown in the log's greeting. Default: "Player"</summary>
        public string PlayerName { get; set; } = "Player";

        #endregion

        #region [timer]

        /// <summary>Each side's total time, in whole minutes (1-600); countdown only, count-up has no limit. Default: from <see cref="Rules.TotalTimeLimit"/> (30)</summary>
        public int TotalTimeMinutes { get; set; } = (int)RuleDefaults.TotalTimeLimit.TotalMinutes;

        /// <summary>Time per move, in whole seconds (1-3600); countdown only. Default: from <see cref="Rules.StepTimeLimit"/> (300)</summary>
        public int StepTimeSeconds { get; set; } = (int)RuleDefaults.StepTimeLimit.TotalSeconds;

        /// <summary>Seconds added to a side's total after each of its moves (0-600); only applies to countdown clocks, count-up ignores it. Default: from <see cref="Rules.IncrementPerMove"/> (0)</summary>
        public int IncrementSeconds { get; set; } = (int)RuleDefaults.IncrementPerMove.TotalSeconds;

        /// <summary>Whether the per-move limit applies; countdown only (count-up always measures the step, without a limit). Default: from <see cref="Rules.EnableStepTimer"/> (true)</summary>
        public bool StepTimerEnabled { get; set; } = RuleDefaults.EnableStepTimer;

        /// <summary>Clocks count down to the limits or up from zero (count-up only measures: the limit, increment and time-up settings are ignored). Default: from <see cref="Rules.TimerMode"/> (CountDown)</summary>
        public TimerMode TimerMode { get; set; } = RuleDefaults.TimerMode;

        /// <summary>Whether a side whose clock runs out loses; countdown only (a count-up clock never runs out). Default: from <see cref="Rules.EndGameWhenTimesUp"/> (true)</summary>
        public bool EndGameWhenTimesUp { get; set; } = RuleDefaults.EndGameWhenTimesUp;

        #endregion

        #region [rules]

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

        #endregion

        #region [dark_chess]

        /// <summary>暗棋 mode. Default: from <see cref="Rules.IsHiddenChess"/> (true)</summary>
        public bool IsHiddenChess { get; set; } = RuleDefaults.IsHiddenChess;

        /// <summary>暗吃. Default: from <see cref="Rules.CanCaptureHiddenPiece"/> (false)</summary>
        public bool CanCaptureHiddenPiece { get; set; } = RuleDefaults.CanCaptureHiddenPiece;

        /// <summary>a 暗吃 revealing a stronger target kills the attacker (false: it returns alive). Default: from <see cref="Rules.IsCaptureHiddenPieceStrongerSuicide"/> (true)</summary>
        public bool IsCaptureHiddenPieceStrongerSuicide { get; set; } = RuleDefaults.IsCaptureHiddenPieceStrongerSuicide;

        /// <summary>連吃. Default: from <see cref="Rules.IsAllowChainCapture"/> (false)</summary>
        public bool IsAllowChainCapture { get; set; } = RuleDefaults.IsAllowChainCapture;

        /// <summary>車衝. Default: from <see cref="Rules.CanChariotRush"/> (false)</summary>
        public bool CanChariotRush { get; set; } = RuleDefaults.CanChariotRush;

        /// <summary>馬斜. Default: from <see cref="Rules.IsHorseMoveDiagonally"/> (false)</summary>
        public bool IsHorseMoveDiagonally { get; set; } = RuleDefaults.IsHorseMoveDiagonally;

        /// <summary>包跳吃子. Default: from <see cref="Rules.IsCannonMustJumpToCapture"/> (true)</summary>
        public bool IsCannonMustJumpToCapture { get; set; } = RuleDefaults.IsCannonMustJumpToCapture;

        #endregion

        #region [hints]

        /// <summary>Rings on the selected piece's legal destinations. Default: true</summary>
        public bool ShowLegalMoveHints { get; set; } = true;

        /// <summary>Rings around hanging pieces (無根子可被吃). Default: true</summary>
        public bool ShowHangingPieceHints { get; set; } = true;

        #endregion

        #region [input]

        /// <summary>Scroll distance per mouse-wheel notch, in UI design units (1-500). Default: 30</summary>
        public float WheelScrollStep { get; set; } = 30f;

        #endregion

        #region [log]

        /// <summary>Whether DEBUG-level log lines are shown (the log's verbosity). Default: true</summary>
        public bool ShowDebugLog { get; set; } = true;

        #endregion

        #region [endgame]

        /// <summary>
        /// Folder of the player's own endgame puzzles; empty for the default
        /// (<see cref="SystemSettings.DefaultUserEndgameFolder"/>). A relative path is taken
        /// relative to <see cref="SystemSettings.UserDataFolder"/>. Default: "" (empty)
        /// </summary>
        public string EndgameUserFolder { get; set; } = string.Empty;

        /// <summary>The player's endgame folder actually used: <see cref="EndgameUserFolder"/> resolved, or the default when empty.</summary>
        public string ResolvedEndgameUserFolder =>
            string.IsNullOrWhiteSpace(EndgameUserFolder)
                ? SystemSettings.DefaultUserEndgameFolder
                : ResolveUserFolder(EndgameUserFolder);

        #endregion

        #region [opening]

        /// <summary>
        /// Folder of the player's own openings (開局練習); empty for the default
        /// (<see cref="SystemSettings.DefaultUserOpeningFolder"/>). A relative path is taken
        /// relative to <see cref="SystemSettings.UserDataFolder"/>. Default: "" (empty)
        /// </summary>
        public string OpeningUserFolder { get; set; } = string.Empty;

        /// <summary>The player's opening folder actually used: <see cref="OpeningUserFolder"/> resolved, or the default when empty.</summary>
        public string ResolvedOpeningUserFolder =>
            string.IsNullOrWhiteSpace(OpeningUserFolder)
                ? SystemSettings.DefaultUserOpeningFolder
                : ResolveUserFolder(OpeningUserFolder);

        #endregion

        /// <summary>A folder setting: environment variables expanded, relative to <see cref="SystemSettings.UserDataFolder"/>.</summary>
        private static string ResolveUserFolder(string folder) =>
            System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(folder.Trim()), SystemSettings.UserDataFolder);

        /// <summary>
        /// The rule set a new <see cref="GameManager"/> plays by: the default
        /// <see cref="Rules"/> with this player's clock and rule choices applied.
        /// </summary>
        public Rules CreateRules()
        {
            var rules = new Rules();
            ApplyTo(rules);
            return rules;
        }

        /// <summary>
        /// Sets <paramref name="rules"/>' clock and rule properties from this player's choices
        /// (every property <see cref="CreateRules"/> sets; the rest are left alone). Lets the
        /// settings screen update the rules new games start with (<c>GameManager.DefaultRules</c>)
        /// without replacing the object; a game already started keeps its own copy.
        /// </summary>
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
            rules.IsHiddenChess = IsHiddenChess;
            rules.CanCaptureHiddenPiece = CanCaptureHiddenPiece;
            rules.IsCaptureHiddenPieceStrongerSuicide = IsCaptureHiddenPieceStrongerSuicide;
            rules.IsAllowChainCapture = IsAllowChainCapture;
            rules.CanChariotRush = CanChariotRush;
            rules.IsHorseMoveDiagonally = IsHorseMoveDiagonally;
            rules.IsCannonMustJumpToCapture = IsCannonMustJumpToCapture;
        }
    }
}
