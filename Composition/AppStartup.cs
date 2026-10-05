/* ----- ----- ----- ----- */
// AppStartup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Configs;
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
        /// Sets up the player's settings (the single entry, <see cref="SettingsFile"/>, with every
        /// settings area registered in file order), builds the service provider (the shared
        /// registrations plus the launcher's own), reads the file and lets every area put its values
        /// into effect (<see cref="SettingsFile.Load"/>), and sets the default button style.
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
            // Player settings: one entry reads settings.ini (in the per-user data folder, created /
            // repaired there as needed) and hands each area its values. The areas are registered
            // here, in file order; each lives in its own layer.
            var playerSettings = PlayerSettings.Defaults;
            var settingsFile = new SettingsFile(SystemSettings.PlayerSettingsFilePath, PlayerSettingsFile.Header);
            foreach (var area in playerSettings.Areas)
                settingsFile.Register(area);

            var services = new ServiceCollection().AddChineseChess(playerSettings, settingsFile);
            addPlatformServices?.Invoke(services);
            var sp = services.BuildServiceProvider();

            // Read after the registrations, so the game wiring (names, rules) is in place; the
            // game itself is still created on first use and takes its rules and names then.
            settingsFile.Load();

            DefaultStyles.DefaultButtonStyle = UILayoutStyles.MainMenu.Button.Style;

            return sp;
        }
    }
}
