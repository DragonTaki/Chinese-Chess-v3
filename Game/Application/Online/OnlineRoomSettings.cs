/* ----- ----- ----- ----- */
// OnlineRoomSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// A custom room's settings between the client's rule model and the protocol: builds the
    /// CreateRoom data from a <see cref="RuleSettings"/> (the same option model the local rule
    /// settings use: its <see cref="RuleSettings.OptionsFor"/> switches and its clock values) and
    /// turns a room's settings back into the sentences a room shows. The room's settings are the
    /// game's rules (ONLINE-PLAY §3.3), not the player's local settings.
    /// </summary>
    public static class OnlineRoomSettings
    {
        // The rule switches' names on the wire: the rules' property names, camelCase (the server's openRules).
        private static readonly Dictionary<RuleOption, string> WireNames = new()
        {
            [RuleOption.GeneralCanSeeGeneral] = "canGeneralSeeGeneral",
            [RuleOption.GeneralCanLeavePalace] = "canGeneralLeavePalace",
            [RuleOption.AdvisorCanLeavePalace] = "canAdvisorLeavePalace",
            [RuleOption.ElephantEyeBlocks] = "canElephantEyeBlocked",
            [RuleOption.HorseLegBlocks] = "canHorseLegHobbled",
            [RuleOption.CaptureOwnPiece] = "canCaptureOwnPiece",
        };

        private static readonly RuleSettings Defaults = new RuleSettings();

        /// <summary>The server's name of a game kind (the enum member's name, e.g. Traditional).</summary>
        /// <param name="kind">The game kind.</param>
        /// <returns>The name.</returns>
        public static string KindName(GameKind kind) => kind.ToString();

        /// <summary>The server's name of a first mover choice.</summary>
        /// <param name="firstMover">The choice.</param>
        /// <returns>Host, Guest or Random.</returns>
        public static string FirstMoverName(OnlineFirstMover firstMover) => firstMover switch
        {
            OnlineFirstMover.Host => RoomNames.FirstMoverHost,
            OnlineFirstMover.Guest => RoomNames.FirstMoverGuest,
            OnlineFirstMover.Random => RoomNames.FirstMoverRandom,
            _ => throw new ArgumentOutOfRangeException(nameof(firstMover), firstMover, "Unknown first mover"),
        };

        /// <summary>
        /// A custom room's CreateRoom data: <paramref name="kind"/>'s rule switches that have a
        /// name on the wire, and the clocks, from <paramref name="rules"/>.
        /// </summary>
        /// <param name="kind">The game kind.</param>
        /// <param name="rules">The room's rule and clock choices.</param>
        /// <param name="firstMover">Who moves first.</param>
        /// <param name="allowPause">Whether pausing is allowed.</param>
        /// <returns>The settings.</returns>
        public static RoomSettingsDto Create(GameKind kind, RuleSettings rules, OnlineFirstMover firstMover, bool allowPause)
        {
            ArgumentNullException.ThrowIfNull(rules);

            var switches = new Dictionary<string, bool>();
            foreach (RuleOption option in RuleSettings.OptionsFor(kind))
            {
                if (WireNames.TryGetValue(option, out string name))
                    switches[name] = rules.Get(option);
            }

            return new RoomSettingsDto
            {
                Kind = KindName(kind),
                Mode = RoomNames.ModeCustom,
                Rules = switches,
                Timer = new TimerSettingsDto
                {
                    Mode = rules.TimerMode == TimerMode.CountUp ? RoomNames.TimerCountUp : RoomNames.TimerCountDown,
                    TotalMinutes = rules.TotalTimeMinutes,
                    StepSeconds = rules.StepTimeSeconds,
                    StepTimer = rules.StepTimerEnabled,
                    IncrementSeconds = rules.IncrementSeconds,
                },
                FirstMover = FirstMoverName(firstMover),
                AllowPause = allowPause,
            };
        }

        /// <summary>The game kind a room's settings name, or null when this client does not know it.</summary>
        /// <param name="settings">The room's settings.</param>
        /// <returns>The kind, or null.</returns>
        public static GameKind? KindOf(RoomSettingsDto settings) =>
            settings != null && Enum.TryParse(settings.Kind, ignoreCase: false, out GameKind kind) && Enum.IsDefined(kind) ? kind : null;

        /// <summary>The name a room's game kind is shown with (the new-game menu's), or the server's name when unknown.</summary>
        /// <param name="settings">The room's settings.</param>
        /// <returns>The name.</returns>
        public static string KindLabel(RoomSettingsDto settings) =>
            KindOf(settings) is GameKind kind ? SettingsTexts.GameKindName(kind) : settings?.Kind ?? "";

        /// <summary>
        /// A room's settings as lines to show: the game kind, each rule switch (a missing one at its
        /// default), the clocks, the first mover and pausing.
        /// </summary>
        /// <param name="settings">The room's settings.</param>
        /// <returns>The lines.</returns>
        public static IReadOnlyList<string> Summary(RoomSettingsDto settings)
        {
            var lines = new List<string>();
            if (settings == null)
                return lines;

            lines.Add(KindLabel(settings));

            if (KindOf(settings) is GameKind kind)
            {
                foreach (RuleOption option in RuleSettings.OptionsFor(kind))
                {
                    if (!WireNames.TryGetValue(option, out string name))
                        continue;
                    bool on = settings.Rules != null && settings.Rules.TryGetValue(name, out bool value) ? value : Defaults.Get(option);
                    lines.Add(OnlineTexts.RuleLine(option, on));
                }
            }

            TimerSettingsDto timer = settings.Timer;
            if (timer != null)
            {
                bool countUp = timer.Mode == RoomNames.TimerCountUp;
                lines.Add(SettingsTexts.SettingText(SettingsTexts.TimerMode, countUp ? SettingsTexts.TimerModeCountUp : SettingsTexts.TimerModeCountDown));
                if (!countUp)
                {
                    lines.Add(SettingsTexts.SettingText(SettingsTexts.TotalTime, SettingsTexts.Minutes(timer.TotalMinutes)));
                    lines.Add(SettingsTexts.SettingText(SettingsTexts.StepTime, timer.StepTimer ? SettingsTexts.Seconds(timer.StepSeconds) : SettingsTexts.Off));
                    lines.Add(SettingsTexts.SettingText(SettingsTexts.Increment, SettingsTexts.Seconds(timer.IncrementSeconds)));
                }
            }

            lines.Add(SettingsTexts.SettingText(OnlineTexts.FirstMover, OnlineTexts.FirstMoverName(settings.FirstMover)));
            lines.Add(SettingsTexts.SettingText(OnlineTexts.AllowPause, settings.AllowPause ? SettingsTexts.On : SettingsTexts.Off));
            return lines;
        }
    }
}
