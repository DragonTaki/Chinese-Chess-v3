/* ----- ----- ----- ----- */
// IDisplaySettingsApplier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Configs;

namespace Chinese_Chess_v3.Game.Application.Settings
{
    /// <summary>
    /// Puts the settings that belong to the display's engine UI services (the mouse wheel's
    /// scroll step) into effect; implemented by the display layer, called by
    /// <see cref="SettingsApplier"/>, so the logic layer never references a UI type.
    /// </summary>
    public interface IDisplaySettingsApplier
    {
        /// <summary>Applies the display's part of <paramref name="settings"/>.</summary>
        /// <param name="settings">The live settings.</param>
        void Apply(PlayerSettings settings);
    }
}
