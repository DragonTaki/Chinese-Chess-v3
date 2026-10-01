/* ----- ----- ----- ----- */
// Settings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Static mirror of two player settings for static callers. The launchers set both from
    /// the loaded <see cref="PlayerSettings"/> at startup (before pushing them into
    /// <c>AppLogger</c>); the initial values are the <see cref="PlayerSettings"/> code
    /// defaults, not separate literals.
    /// </summary>
    public static class Settings
    {
        /// <summary>Whether DEBUG log lines are shown (<see cref="PlayerSettings.ShowDebugLog"/>).</summary>
        public static bool EnableDebugMode { get; set; } = PlayerSettings.Defaults.ShowDebugLog;

        /// <summary>The player's name (<see cref="PlayerSettings.PlayerName"/>).</summary>
        public static string CurrentUser { get; set; } = PlayerSettings.Defaults.PlayerName;
    }
}