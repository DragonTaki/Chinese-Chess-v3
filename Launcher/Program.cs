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

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Boards;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;
using Chinese_Chess_v3.Game.UI.Sidebars;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;
using Engine.Randomization;

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
            // Create service collection for DI
            var services = new ServiceCollection();

            // Register core UI services and factories
            services.AddSingleton<IUiFactory, UiFactory>();
            services.AddSingleton<IScrollInputHandler, ScrollInputHandler>();

            // Register main WinForms form
            services.AddSingleton<MainForm>();

            // Register utility services
            services.AddSingleton<RandomTable>(new RandomTable(size: 10000, seed: 12345));

            // Register managers and core systems
            services.AddSingleton<NavigationManager>();
            services.AddSingleton<UIRootNode>();
            services.AddSingleton<DialogManager>();
            services.AddSingleton<GameManager>();

            // Register singleton UI modules with handlers and renderers
            services.AddSingletonUiModule<UIMainMenu,     UIMainMenuHandler,     UIMainMenuRenderer>();
            services.AddSingletonUiModule<UINewGameMenu,  UINewGameMenuHandler,  UINewGameMenuRenderer>();
            services.AddSingletonUiModule<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>();
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
