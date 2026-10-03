/* ----- ----- ----- ----- */
// PlayerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/04
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;

using Engine.Diagnostics;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings the player chooses (what a future in-game settings screen exposes).
    /// The property initializers ARE the code defaults - the one place they are defined
    /// (the rule and clock defaults of each game kind live in <see cref="RuleSettings"/>,
    /// read from a default <see cref="Rules"/>, which owns what those rules mean). The values in use are loaded from the player's
    /// <c>settings.ini</c> by <see cref="PlayerSettingsFile"/>; the launchers register the
    /// loaded instance in DI.
    /// </summary>
    public sealed class PlayerSettings
    {
        /// <summary>A new instance holding only the code defaults.</summary>
        public static PlayerSettings Defaults => new PlayerSettings();

        #region [player]

        /// <summary>Longest <see cref="PlayerName"/> (characters).</summary>
        public const int PlayerNameMaxLength = 32;

        /// <summary>Name shown in the log's greeting (at most <see cref="PlayerNameMaxLength"/> characters). Default: "Player"</summary>
        public string PlayerName { get; set; } = "Player";

        /// <summary>Local Player1's name (先手, 玩家一; at most <see cref="PlayerNameMaxLength"/> characters). Default: "玩家一"</summary>
        public string Player1Name { get; set; } = "玩家一";

        /// <summary>Local Player2's name (後手, 玩家二). Default: "玩家二"</summary>
        public string Player2Name { get; set; } = "玩家二";

        /// <summary>Local Player3's name (三國's third player, 玩家三). Default: "玩家三"</summary>
        public string Player3Name { get; set; } = "玩家三";

        /// <summary>Gives <paramref name="game"/> the local players' names (<see cref="GameManager.SetPlayerNames"/>).</summary>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is null.</exception>
        public void ApplyPlayerNamesTo(GameManager game)
        {
            ArgumentNullException.ThrowIfNull(game);
            game.SetPlayerNames(Player1Name, Player2Name, Player3Name);
        }

        #endregion

        #region [rules.*]

        // One rule set per game kind (local games only; a network game does not use them).
        private readonly Dictionary<GameKind, RuleSettings> _rules =
            Enum.GetValues<GameKind>().ToDictionary(kind => kind, _ => new RuleSettings());

        /// <summary>
        /// The rule and clock choices for new games of <paramref name="kind"/> (the stored
        /// object, edited in place; settings.ini <c>[rules.*]</c>). Default: every kind with
        /// the <see cref="RuleSettings"/> defaults.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a <see cref="GameKind"/>.</exception>
        public RuleSettings RulesFor(GameKind kind) =>
            _rules.TryGetValue(kind, out var rules)
                ? rules
                : throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind");

        #endregion

        #region [display]

        /// <summary>The frame rates <see cref="Fps"/> can be (the settings screen's FPS choices).</summary>
        public static readonly IReadOnlyList<int> FpsOptions = new[] { 30, 60, 120, 144 };

        /// <summary>Animation frame rate (one of <see cref="FpsOptions"/>), pushed to the engine's frame timer. Default: 60</summary>
        public int Fps { get; set; } = 60;

        #endregion

        #region [hints]

        /// <summary>Rings on the selected piece's legal destinations. Default: true</summary>
        public bool ShowLegalMoveHints { get; set; } = true;

        /// <summary>Rings around hanging pieces (無根子可被吃). Default: true</summary>
        public bool ShowHangingPieceHints { get; set; } = true;

        #endregion

        #region [input]

        /// <summary>Smallest / largest <see cref="WheelScrollStep"/>.</summary>
        public const float WheelScrollStepMin = 1f, WheelScrollStepMax = 500f;

        /// <summary>Scroll distance per mouse-wheel notch, in UI design units (<see cref="WheelScrollStepMin"/>-<see cref="WheelScrollStepMax"/>). Default: 30</summary>
        public float WheelScrollStep { get; set; } = 30f;

        #endregion

        #region [debug]

        // The debug features' switches (the settings screen's DEBUG tab), pushed into the
        // engine's DebugOptions by ApplyDebugOptions. Each debug feature in the code runs or
        // draws only while its switch is on.

        /// <summary>Whether DEBUG-level log lines are written (the log's verbosity). Default: true</summary>
        public bool VerboseLog { get; set; } = true;

        /// <summary>Whether developer trace lines (UI init / menu selections / network status) are printed to the console. Default: true</summary>
        public bool ConsoleTrace { get; set; } = true;

        /// <summary>Whether labels get a semi-transparent red background (visual debugging). Default: false</summary>
        public bool LabelBackgrounds { get; set; } = false;

        /// <summary>Whether menus and text boxes get a grey layout outline (visual debugging). Default: false</summary>
        public bool LayoutOutlines { get; set; } = false;

        /// <summary>Whether the star background outlines each effect's area (visual debugging). Default: false</summary>
        public bool StarEffectFrames { get; set; } = false;

        /// <summary>Whether the measured frame rate is shown in the window's top-left corner. Default: false</summary>
        public bool ShowFps { get; set; } = false;

        /// <summary>Whether the network latency is shown under the frame rate (a dash until there is a connection). Default: false</summary>
        public bool ShowNetworkLatency { get; set; } = false;

        /// <summary>Pushes the debug switches into the engine (<see cref="DebugOptions"/>), where the debug features read them.</summary>
        public void ApplyDebugOptions()
        {
            DebugOptions.VerboseLog = VerboseLog;
            DebugOptions.ConsoleTrace = ConsoleTrace;
            DebugOptions.LabelBackgrounds = LabelBackgrounds;
            DebugOptions.LayoutOutlines = LayoutOutlines;
            DebugOptions.StarEffectFrames = StarEffectFrames;
            DebugOptions.ShowFps = ShowFps;
            DebugOptions.ShowNetworkLatency = ShowNetworkLatency;
        }

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
        /// The rule sets a new <see cref="GameManager"/> starts games with: per kind, the
        /// default <see cref="Rules"/> with that kind's choices applied.
        /// </summary>
        public GameRuleSets CreateRuleSets()
        {
            var sets = new GameRuleSets();
            ApplyTo(sets);
            return sets;
        }

        /// <summary>
        /// Applies every kind's choices to that kind's rules in <paramref name="sets"/>
        /// (<see cref="RuleSettings.ApplyTo"/>, the objects updated in place). Lets the settings
        /// screen update the rules new games start with (<c>GameManager.DefaultRuleSets</c>); a
        /// game already started keeps its own copy.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="sets"/> is null.</exception>
        public void ApplyTo(GameRuleSets sets)
        {
            ArgumentNullException.ThrowIfNull(sets);
            foreach (var (kind, rules) in _rules)
                rules.ApplyTo(sets[kind]);
        }
    }
}
