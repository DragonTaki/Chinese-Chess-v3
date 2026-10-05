/* ----- ----- ----- ----- */
// SettingsApplier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;

using Engine.Logging;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Settings
{
    /// <summary>
    /// The app's <see cref="ISettingsApplier"/>: copies each kind's rule / clock settings onto
    /// <see cref="GameManager.DefaultRuleSets"/> (<see cref="PlayerSettings.ApplyTo"/>) and the
    /// local players' names onto the game (<see cref="PlayerSettings.ApplyPlayerNamesTo"/>), the
    /// DEBUG tab's switches onto the engine's debug switchboard
    /// (<see cref="PlayerSettings.ApplyDebugOptions"/>), the player name onto the log greeting
    /// (<see cref="AppLogger.CurrentUser"/>), the display's part (the wheel step,
    /// <see cref="IDisplaySettingsApplier"/>) and the frame rate onto the engine's frame timer
    /// (<see cref="TimerSettings.GameAnimationFPS"/>). The shared startup applies the parts that
    /// don't need the game once (<see cref="ApplyToEngine"/>; the game takes its rules and names
    /// from the settings when it is created); the settings screen applies its later changes
    /// with <see cref="Apply"/>.
    /// </summary>
    public sealed class SettingsApplier : ISettingsApplier
    {
        private readonly Func<GameManager> _game;
        private readonly IDisplaySettingsApplier _display;

        /// <param name="game">Gets the game (called on each apply, so the game is still created on first use).</param>
        /// <param name="display">The display layer's part (the wheel step); null for none.</param>
        public SettingsApplier(Func<GameManager> game, IDisplaySettingsApplier display)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _display = display;
        }

        /// <inheritdoc/>
        public void Apply(PlayerSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            var gameManager = _game();
            if (gameManager != null)
            {
                settings.ApplyTo(gameManager.DefaultRuleSets);
                settings.ApplyPlayerNamesTo(gameManager);
            }

            ApplyToEngine(settings);
        }

        /// <inheritdoc/>
        public void ApplyToEngine(PlayerSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);

            settings.ApplyDebugOptions();

            AppLogger.CurrentUser = settings.PlayerName;

            _display?.Apply(settings);

            TimerSettings.GameAnimationFPS = settings.Fps;
        }
    }
}
