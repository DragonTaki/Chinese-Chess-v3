/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Chinese_Chess_v3.Composition;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;

namespace Launcher.Cross
{
    /// <summary>
    /// Cross-platform entry point for the Chinese Chess application — same
    /// shared DI wiring and startup (<c>Chinese_Chess_v3.Composition</c>) as <c>Launcher.Program</c>, backed by SkiaSharp (rendering)
    /// and Silk.NET (windowing/input) instead of GDI+/WinForms, so it also
    /// runs on macOS/Linux.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            // Must be set before anything else — see Launcher.Program.Main.
            GraphicsBackend.Factory = new SkiaGraphicsFactory();

            // Must run before any static class touches a custom font key
            // (e.g. DefaultStyles.DefaultButtonStyle in AppStartup.BuildServices
            // below, which triggers UILayoutStyles's static constructor) —
            // otherwise FontManager
            // hasn't registered "NotoSerif"/"MoeLI" yet, StyleHelper.GetFont
            // silently falls back to GraphicsBackend.Factory.GetSystemFontFamily
            // with that same string as a *system* font name, and since no
            // such family exists, Skia substitutes some default Latin font
            // with no CJK glyphs — Chinese text renders as tofu boxes.
            FontManager.LoadFonts();

            // The window opens at a normal desktop size (1080p) rather than
            // the UI's own (smaller) DesignSize — content is scaled up to
            // fill it via GlobalViewport, same as any later resize. See
            // UILayoutConstants.DefaultWindowSize/MinimumWindowSize.
            var options = WindowOptions.Default with
            {
                Title = SystemSettings.WindowTitle,
                Size = new Vector2D<int>(
                    (int)UILayoutConstants.DefaultWindowSize.X,
                    (int)UILayoutConstants.DefaultWindowSize.Y),
            };
            var window = Window.Create(options);

            AppControl.ExitCallback = window.Close;

            // Settings, the shared DI registrations, and the settings pushed to the engine
            // (Composition/AppStartup.cs).
            var sp = AppStartup.BuildServices();

            using var app = new CrossPlatformApp(sp, window);
            window.Run();
        }
    }
}
