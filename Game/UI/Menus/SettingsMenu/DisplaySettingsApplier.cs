/* ----- ----- ----- ----- */
// DisplaySettingsApplier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;

using Engine.UI.Input;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// The app's <see cref="IDisplaySettingsApplier"/>: the settings that belong to engine UI
    /// services, which the logic layer's <see cref="SettingsApplier"/> cannot reach - the mouse
    /// wheel's step onto the shared <see cref="ScrollInputHandler"/>.
    /// </summary>
    public sealed class DisplaySettingsApplier : IDisplaySettingsApplier
    {
        private readonly IScrollInputHandler _scroll;

        /// <param name="scroll">The shared scroll handler (its wheel step is set when it is a <see cref="ScrollInputHandler"/>).</param>
        public DisplaySettingsApplier(IScrollInputHandler scroll)
        {
            _scroll = scroll;
        }

        /// <inheritdoc/>
        public void Apply(PlayerSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            if (_scroll is ScrollInputHandler scroll)
                scroll.WheelStep = settings.WheelScrollStep;
        }
    }
}
