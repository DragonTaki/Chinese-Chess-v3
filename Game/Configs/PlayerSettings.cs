/* ----- ----- ----- ----- */
// PlayerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Configs;
using Engine.Diagnostics;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings the player chooses, as one set of settings areas (author 2026-10-05: one
    /// entry reads the file and hands each area its values; each area lives in its own layer
    /// with its own defaults). The logic layer's areas are here in <c>Game/Configs</c>; the
    /// engine's (frame rate, input, debug switches) are in the engine. The settings screens edit
    /// these objects; the settings file (<see cref="SettingsFile"/>, built by
    /// <see cref="PlayerSettingsFile.Open"/> or the composition root) reads and writes them.
    /// </summary>
    public sealed class PlayerSettings
    {
        /// <summary>A new set holding only the code defaults.</summary>
        public static PlayerSettings Defaults => new PlayerSettings();

        /// <summary><c>[player]</c>: the players' names.</summary>
        public PlayerNameSettings Names { get; } = new();

        /// <summary><c>[rules.*]</c>: each game kind's rules and clocks.</summary>
        public GameRuleSettings Rules { get; } = new();

        /// <summary><c>[display] fps</c> (engine).</summary>
        public FrameRateSettings FrameRate { get; } = new();

        /// <summary><c>[hints]</c>: the board hints.</summary>
        public HintSettings Hints { get; } = new();

        /// <summary><c>[input]</c> (engine).</summary>
        public InputSettings Input { get; } = new();

        /// <summary><c>[debug]</c>: the debug features' switches (engine).</summary>
        public DebugSettings Debug { get; } = new();

        /// <summary><c>[endgame]</c>, <c>[opening]</c>: the player's own folders.</summary>
        public FolderSettings Folders { get; } = new();

        /// <summary>Every area, in file order.</summary>
        public IReadOnlyList<ISettingsArea> Areas => new ISettingsArea[] { Names, Rules, FrameRate, Hints, Input, Debug, Folders };
    }
}
