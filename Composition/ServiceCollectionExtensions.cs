/* ----- ----- ----- ----- */
// ServiceCollectionExtensions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.GameScreen;
using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Boards;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.EndgameMenu;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadSavedGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.OpeningMenu;
using Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;
using Chinese_Chess_v3.Game.UI.Navigation;
using Chinese_Chess_v3.Game.UI.Sidebars;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.Network;
using Engine.Randomization;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Infrastructure;
using Engine.UI.Input;

namespace Chinese_Chess_v3.Composition
{
    /// <summary>
    /// The app's dependency injection registrations, shared by both launchers (WinForms and
    /// Skia/Silk.NET). A launcher adds only its own platform parts (e.g. the WinForms main form).
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers every service, view model, presenter and UI module the game uses.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="playerSettings">The player settings loaded at startup (registered as the single live instance).</param>
        /// <returns>The updated <see cref="IServiceCollection"/> to allow chaining.</returns>
        public static IServiceCollection AddChineseChess(this IServiceCollection services, PlayerSettings playerSettings)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(playerSettings);

            // Register core UI services and factories
            services.AddSingleton<IUiFactory, UiFactory>();
            // The wheel step is set at startup by ISettingsApplier.ApplyToEngine (AppStartup).
            services.AddSingleton<IScrollInputHandler, ScrollInputHandler>();
            services.AddSingleton(playerSettings);

            // Register utility services
            services.AddSingleton<RandomTable>(new RandomTable(size: SystemSettings.RandomTableSize, seed: SystemSettings.RandomTableSeed));

            // Register managers and core systems
            services.AddSingleton<NavigationManager>();
            services.AddSingleton<INavigator, Navigator>();
            services.AddSingleton<IAppLifetime, AppLifetime>();
            services.AddSingleton<UIRootNode>();
            services.AddSingleton(sp => new DialogManager<UIConfirmDialog>(
                () => new UIConfirmDialog(new UIConfirmDialogRenderer(), sp.GetRequiredService<IUiFactory>())));
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<NetworkManager>();
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<PlayerSettings>();
                var game = new GameManager(settings.CreateRuleSets());
                settings.ApplyPlayerNamesTo(game);
                return game;
            });
            // The game log's sentences (from the game's log entries); the sidebar hands it the log box.
            services.AddSingleton<GameLogComposer>();
            // The game flow; takes the game through a factory so the GameManager is still created on first use.
            services.AddSingleton(sp => new GameSession(() => sp.GetRequiredService<GameManager>(), sp.GetRequiredService<INavigator>()));
            // The game screen's decisions; created with the game screen (its handler resolves it).
            services.AddSingleton<GameScreenPresenter>();
            // The main menu's decisions; created with the main menu (its handler resolves it).
            services.AddSingleton<MainMenuPresenter>();
            // What the game screen's info board shows (names, sides, clock texts); read live from the game.
            services.AddSingleton<InfoBoardViewModel>();
            // Pushes the settings screens' edits to the game and engine; takes the game through a factory like the session.
            services.AddSingleton<ISettingsApplier>(sp => new SettingsApplier(() => sp.GetRequiredService<GameManager>(), sp.GetRequiredService<IScrollInputHandler>()));
            // The lists' catalogs (殘局闖關, 開局練習, the saved games); single instances, so a list's
            // switched-off categories are kept while the game runs.
            services.AddSingleton<EndgameCatalog>();
            services.AddSingleton<OpeningCatalog>();
            services.AddSingleton<SavedGameCatalog>();

            // Register singleton UI modules with handlers and renderers
            services.AddSingletonUiModule<UIMainMenu,     UIMainMenuHandler,     UIMainMenuRenderer>();
            services.AddSingletonUiModule<UINewGameMenu,  UINewGameMenuHandler,  UINewGameMenuRenderer>();
            services.AddSingletonUiModule<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>();
            services.AddSingletonUiModule<UILoadSavedGameMenu, UILoadSavedGameMenuHandler, UILoadSavedGameMenuRenderer>();
            services.AddSingletonUiModule<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>();
            services.AddSingletonUiModule<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer>();
            services.AddSingletonUiModule<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer>();
            services.AddSingletonUiModule<UIGameMenu,     UIGameMenuHandler,     UIGameMenuRenderer>();

            // Register transient UI modules with handlers and renderers
            services.AddTransientUiModule<UIBoard,     UIBoardHandler,     UIBoardRenderer>();
            services.AddTransientUiModule<UISidebar,   UISidebarHandler,   UISidebarRenderer>();
            services.AddTransientUiModule<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
            services.AddTransientUiModule<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>();
            // Two settings submenus (遊戲設定, 規則設定 - each its own instance with its own
            // Scope): transient, so each CreateDIElement gets a new element, handler and renderer.
            services.AddTransientUiModule<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>();

            return services;
        }

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
