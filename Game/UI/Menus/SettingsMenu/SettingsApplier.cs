/* ----- ----- ----- ----- */
// SettingsApplier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;

using Engine.Logging;
using Engine.Timing;
using Engine.UI.Input;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// The app's <see cref="ISettingsApplier"/>: copies each kind's rule / clock settings onto
    /// <see cref="GameManager.DefaultRuleSets"/> (<see cref="PlayerSettings.ApplyTo"/>) and the
    /// local players' names onto the game (<see cref="PlayerSettings.ApplyPlayerNamesTo"/>), the
    /// DEBUG tab's switches onto the engine's debug switchboard
    /// (<see cref="PlayerSettings.ApplyDebugOptions"/>), the player name onto the log greeting
    /// (<see cref="AppLogger.CurrentUser"/>), the wheel step onto the shared
    /// <see cref="ScrollInputHandler"/> and the frame rate onto the engine's frame timer
    /// (<see cref="TimerSettings.GameAnimationFPS"/>). The launchers push these once at startup
    /// themselves; this applies the settings screen's later changes. It is in the UI layer
    /// because the scroll handler is an engine UI service.
    /// </summary>
    public sealed class SettingsApplier : ISettingsApplier
    {
        private readonly Func<GameManager> _game;
        private readonly IScrollInputHandler _scroll;

        /// <param name="game">Gets the game (called on each apply, so the game is still created on first use).</param>
        /// <param name="scroll">The shared scroll handler (its wheel step is set when it is a <see cref="ScrollInputHandler"/>).</param>
        public SettingsApplier(Func<GameManager> game, IScrollInputHandler scroll)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _scroll = scroll;
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

            settings.ApplyDebugOptions();

            AppLogger.CurrentUser = settings.PlayerName;

            if (_scroll is ScrollInputHandler scroll)
                scroll.WheelStep = settings.WheelScrollStep;

            TimerSettings.GameAnimationFPS = settings.Fps;
        }
    }
}
