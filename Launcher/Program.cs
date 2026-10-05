/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Composition;

using Engine.Platform;
using Engine.Platform.WinForms;
using Engine.Styles;

namespace Launcher
{
    /// <summary>
    /// Main entry point for the Chinese Chess application.
    /// <para>
    /// Responsible for setting up dependency injection (DI), initializing WinForms,
    /// and launching the main form.
    /// </para>
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Application entry point. Configures services, builds the service provider,
        /// and runs the main WinForms form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Must be set before anything else — every style/font/brush
            // constant in Engine.Styles and Game.UI.Constants is created via
            // GraphicsBackend.Factory in its own static initializer, and
            // those can run as soon as the first line below touches them.
            GraphicsBackend.Factory = new WinFormsGraphicsFactory();
            AppControl.ExitCallback = Application.Exit;

            // Must run before any static class touches a custom font key
            // (e.g. DefaultStyles.DefaultButtonStyle in AppStartup.BuildServices
            // below, which triggers UILayoutStyles's static constructor) —
            // otherwise FontManager
            // hasn't registered "NotoSerif"/"MoeLI" yet and StyleHelper.GetFont
            // silently falls back to a *system* font of that same name instead
            // (see FontManager.LoadFonts's caller in Launcher.Cross.Program for
            // the full explanation — this was previously called from
            // MainForm's constructor, which runs too late).
            FontManager.LoadFonts();

            // Settings, the shared DI registrations plus the WinForms main form, and the
            // settings pushed to the engine (Composition/AppStartup.cs).
            var sp = AppStartup.BuildServices(services => services.AddSingleton<MainForm>());

            // Initialize WinForms configuration
            ApplicationConfiguration.Initialize();

            // Run the main form resolved from DI
            Application.Run(sp.GetRequiredService<MainForm>());
        }
    }
}
