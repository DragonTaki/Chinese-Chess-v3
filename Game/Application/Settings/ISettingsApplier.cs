/* ----- ----- ----- ----- */
// ISettingsApplier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Configs;

namespace Chinese_Chess_v3.Game.Application.Settings
{
    /// <summary>
    /// Pushes the player settings to where the running app uses them, so the settings screen's
    /// logic does not depend on the game, engine or UI objects that read them. Implemented by
    /// <see cref="SettingsApplier"/>, which leaves the display's engine UI services to
    /// <see cref="IDisplaySettingsApplier"/>.
    /// </summary>
    public interface ISettingsApplier
    {
        /// <summary>
        /// Applies <paramref name="settings"/> at once: each game kind's rule / clock settings
        /// onto the rules new games of that kind start with (a game in progress keeps its own
        /// copy), the local players' names, the debug switches, the player name of the log
        /// greeting, the mouse wheel's scroll step and the frame rate.
        /// </summary>
        /// <param name="settings">The live settings.</param>
        void Apply(PlayerSettings settings);

        /// <summary>
        /// Applies only the parts of <paramref name="settings"/> that do not need the game: the
        /// debug switches, the player name of the log greeting, the mouse wheel's scroll step and
        /// the frame rate. Used at startup, where the game is still created on first use and
        /// takes its rules and player names from the settings itself.
        /// </summary>
        /// <param name="settings">The live settings.</param>
        void ApplyToEngine(PlayerSettings settings);
    }
}
