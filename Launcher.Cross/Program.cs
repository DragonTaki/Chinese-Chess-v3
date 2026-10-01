/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Boards;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.EndgameMenu;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.OpeningMenu;
using Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;
using Chinese_Chess_v3.Game.UI.Sidebars;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Infrastructure;
using Engine.UI.Input;
using Engine.Randomization;
using Engine.Network;
using Engine.Logging;
using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;

namespace Launcher.Cross
{
    /// <summary>
    /// Cross-platform entry point for the Chinese Chess application — same
    /// DI wiring as <c>Launcher.Program</c>, backed by SkiaSharp (rendering)
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
            // (e.g. DefaultStyles.DefaultButtonStyle below, which triggers
            // UILayoutStyles's static constructor) — otherwise FontManager
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

            // Player settings (docs/SETTINGS.md): loaded once at startup from settings.ini
            // in the per-user data folder (created / repaired there as needed) and
            // registered in DI below for the screens that read them.
            var playerSettings = PlayerSettingsFile.Load();
            Settings.EnableDebugMode = playerSettings.ShowDebugLog;
            Settings.CurrentUser = playerSettings.PlayerName;
            AppLogger.EnableDebug = Settings.EnableDebugMode;
            AppLogger.CurrentUser = Settings.CurrentUser;
            DefaultStyles.DefaultButtonStyle = UILayoutStyles.MainMenu.Button.Style;

            var services = new ServiceCollection();

            services.AddSingleton<IUiFactory, UiFactory>();
            services.AddSingleton<IScrollInputHandler>(_ => new ScrollInputHandler { WheelStep = playerSettings.WheelScrollStep });
            services.AddSingleton(playerSettings);

            services.AddSingleton<RandomTable>(new RandomTable(size: SystemSettings.RandomTableSize, seed: SystemSettings.RandomTableSeed));

            services.AddSingleton<NavigationManager>();
            services.AddSingleton<UIRootNode>();
            services.AddSingleton(sp => new DialogManager<UIConfirmDialog>(
                () => new UIConfirmDialog(new UIConfirmDialogRenderer(), sp.GetRequiredService<IUiFactory>())));
            services.AddSingleton<NetworkManager>();
            services.AddSingleton(sp => new GameManager(sp.GetRequiredService<PlayerSettings>().CreateRules()));

            services.AddSingletonUiModule<UIMainMenu,     UIMainMenuHandler,     UIMainMenuRenderer>();
            services.AddSingletonUiModule<UINewGameMenu,  UINewGameMenuHandler,  UINewGameMenuRenderer>();
            services.AddSingletonUiModule<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>();
            services.AddSingletonUiModule<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>();
            services.AddSingletonUiModule<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer>();
            services.AddSingletonUiModule<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer>();
            services.AddSingletonUiModule<UIGameMenu,     UIGameMenuHandler,     UIGameMenuRenderer>();

            services.AddTransientUiModule<UIBoard,     UIBoardHandler,     UIBoardRenderer>();
            services.AddTransientUiModule<UISidebar,   UISidebarHandler,   UISidebarRenderer>();
            services.AddTransientUiModule<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
            services.AddTransientUiModule<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>();
            // Two settings submenus (遊戲設定, 規則設定 - each its own instance with its own
            // Scope): transient, so each CreateDIElement gets a new element, handler and renderer.
            services.AddTransientUiModule<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>();

            var sp = services.BuildServiceProvider();

            using var app = new CrossPlatformApp(sp, window);
            window.Run();
        }
    }

    /// <summary>
    /// Cross-platform counterpart to <c>Launcher.ServiceCollectionExtensions</c>
    /// (identical logic — kept as a separate copy for the same reason as
    /// <see cref="UIInitializer"/>).
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingletonUiModule<TModule, THandler, TRenderer>(this IServiceCollection services)
            where TModule : class
            where THandler : class
            where TRenderer : class
        {
            services.AddSingleton<TModule>();
            services.AddSingleton<THandler>();
            services.AddSingleton<TRenderer>();
            return services;
        }

        public static IServiceCollection AddTransientUiModule<TModule, THandler, TRenderer>(this IServiceCollection services)
            where TModule : class
            where THandler : class
            where TRenderer : class
        {
            services.AddTransient<TModule>();
            services.AddTransient<THandler>();
            services.AddTransient<TRenderer>();
            return services;
        }
    }
}
