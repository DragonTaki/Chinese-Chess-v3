/* ----- ----- ----- ----- */
// Settings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Static mirror of a player setting for static callers. The launchers set it from the
    /// loaded <see cref="PlayerSettings"/> at startup (before pushing it into
    /// <c>AppLogger</c>); the initial value is the <see cref="PlayerSettings"/> code
    /// default, not a separate literal. (The debug switches are not mirrored here: their one
    /// home is the engine's <c>DebugOptions</c>, see <see cref="PlayerSettings.ApplyDebugOptions"/>.)
    /// </summary>
    public static class Settings
    {
        /// <summary>The player's name (<see cref="PlayerSettings.PlayerName"/>).</summary>
        public static string CurrentUser { get; set; } = PlayerSettings.Defaults.PlayerName;
    }
}
