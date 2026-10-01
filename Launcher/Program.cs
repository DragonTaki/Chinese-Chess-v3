/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;

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
using Chinese_Chess_v3.Game.UI.Sidebars;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;
using Engine.Randomization;
using Engine.Network;
using Engine.Logging;
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
            // (e.g. DefaultStyles.DefaultButtonStyle below, which triggers
            // UILayoutStyles's static constructor) — otherwise FontManager
            // hasn't registered "NotoSerif"/"MoeLI" yet and StyleHelper.GetFont
            // silently falls back to a *system* font of that same name instead
            // (see FontManager.LoadFonts's caller in Launcher.Cross.Program for
            // the full explanation — this was previously called from
            // MainForm's constructor, which runs too late).
            FontManager.LoadFonts();

            // Push Game-level config into Engine (Engine must not read
            // Game.Configs directly — see Engine/Logging/AppLogger.cs).
            // Player settings (docs/SETTINGS.md): loaded once at startup from settings.ini
            // in the per-user data folder (created / repaired there as needed) and
            // registered in DI below for the screens that read them.
            var playerSettings = PlayerSettingsFile.Load();
            Settings.EnableDebugMode = playerSettings.ShowDebugLog;
            Settings.CurrentUser = playerSettings.PlayerName;
            AppLogger.EnableDebug = Settings.EnableDebugMode;
            AppLogger.CurrentUser = Settings.CurrentUser;
            DefaultStyles.DefaultButtonStyle = UILayoutStyles.MainMenu.Button.Style;

            // Create service collection for DI
            var services = new ServiceCollection();

            // Register core UI services and factories
            services.AddSingleton<IUiFactory, UiFactory>();
            services.AddSingleton<IScrollInputHandler>(_ => new ScrollInputHandler { WheelStep = playerSettings.WheelScrollStep });
            services.AddSingleton(playerSettings);

            // Register main WinForms form
            services.AddSingleton<MainForm>();

            // Register utility services
            services.AddSingleton<RandomTable>(new RandomTable(size: SystemSettings.RandomTableSize, seed: SystemSettings.RandomTableSeed));

            // Register managers and core systems
            services.AddSingleton<NavigationManager>();
            services.AddSingleton<UIRootNode>();
            services.AddSingleton(sp => new DialogManager<UIConfirmDialog>(
                () => new UIConfirmDialog(new UIConfirmDialogRenderer(), sp.GetRequiredService<IUiFactory>())));
            services.AddSingleton<NetworkManager>();
            services.AddSingleton(sp => new GameManager(sp.GetRequiredService<PlayerSettings>().CreateRules()));

            // Register singleton UI modules with handlers and renderers
            services.AddSingletonUiModule<UIMainMenu,     UIMainMenuHandler,     UIMainMenuRenderer>();
            services.AddSingletonUiModule<UINewGameMenu,  UINewGameMenuHandler,  UINewGameMenuRenderer>();
            services.AddSingletonUiModule<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>();
            services.AddSingletonUiModule<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>();
            services.AddSingletonUiModule<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer>();
            services.AddSingletonUiModule<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer>();
            services.AddSingletonUiModule<UIGameMenu,     UIGameMenuHandler,     UIGameMenuRenderer>();

            // Register transient UI modules with handlers and renderers
            services.AddTransientUiModule<UIBoard,     UIBoardHandler,     UIBoardRenderer>();
            services.AddTransientUiModule<UISidebar,   UISidebarHandler,   UISidebarRenderer>();
            services.AddTransientUiModule<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
            services.AddTransientUiModule<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>();

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Initialize WinForms configuration
            ApplicationConfiguration.Initialize();

            // Run the main form resolved from DI
            Application.Run(sp.GetRequiredService<MainForm>());
        }
    }
    
    /// <summary>
    /// Extension methods for IServiceCollection to simplify UI module registration.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a UI module along with its corresponding handler and renderer as singletons.
        /// </summary>
        /// <typeparam name="TModule">The UI module type (screen or component).</typeparam>
        /// <typeparam name="THandler">The handler type associated with the UI module.</typeparam>
        /// <typeparam name="TRenderer">The renderer type associated with the UI module.</typeparam>
        /// <param name="services">The service collection to add the services to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> to allow chaining.</returns>
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

        /// <summary>
        /// Registers a UI module along with its corresponding handler and renderer as transients.
        /// </summary>
        /// <typeparam name="TModule">The UI module type (screen or component).</typeparam>
        /// <typeparam name="THandler">The handler type associated with the UI module.</typeparam>
        /// <typeparam name="TRenderer">The renderer type associated with the UI module.</typeparam>
        /// <param name="services">The service collection to add the services to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> to allow chaining.</returns>
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
