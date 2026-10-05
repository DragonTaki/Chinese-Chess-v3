/* ----- ----- ----- ----- */
// AppStartup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Styles;

namespace Chinese_Chess_v3.Composition
{
    /// <summary>
    /// The startup steps both launchers share, after their platform-specific first steps
    /// (setting <c>GraphicsBackend.Factory</c>, loading the custom fonts, the exit callback).
    /// </summary>
    public static class AppStartup
    {
        /// <summary>
        /// Loads the player settings, builds the service provider (the shared registrations plus
        /// the launcher's own), applies the settings that don't need the game through
        /// <see cref="ISettingsApplier.ApplyToEngine"/>, and sets the default button style.
        /// <para>
        /// Call after <c>GraphicsBackend.Factory</c> is set and <see cref="FontManager.LoadFonts"/>
        /// has run: the default button style below triggers the style classes' static
        /// initializers, which create fonts and brushes through the factory and need the custom
        /// fonts registered.
        /// </para>
        /// </summary>
        /// <param name="addPlatformServices">Adds the launcher's own services (e.g. the WinForms main form); may be null.</param>
        /// <returns>The built service provider.</returns>
        public static IServiceProvider BuildServices(Action<IServiceCollection> addPlatformServices = null)
        {
            // Player settings: loaded once at startup from settings.ini
            // in the per-user data folder (created / repaired there as needed) and
            // registered in DI for the screens that read them.
            var playerSettings = PlayerSettingsFile.Load();

            var services = new ServiceCollection().AddChineseChess(playerSettings);
            addPlatformServices?.Invoke(services);
            var sp = services.BuildServiceProvider();

            // Push the settings to the engine (debug switches, log user, wheel step, FPS) the same
            // way the settings screen does on change; the frame timer follows later FPS changes
            // itself. The game takes its rules and player names from the settings when created.
            sp.GetRequiredService<ISettingsApplier>().ApplyToEngine(playerSettings);

            DefaultStyles.DefaultButtonStyle = UILayoutStyles.MainMenu.Button.Style;

            return sp;
        }
    }
}
