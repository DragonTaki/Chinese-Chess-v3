/* ----- ----- ----- ----- */
// UIInitializer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.UI.Screens.Menus;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Launcher
{
    /// <summary>
    /// Responsible for initializing and connecting all UI components.
    /// <para>
    /// Acts as the bootstrapper for the entire UI system, including the MainMenu, GameMenu,
    /// navigation system, and dialog management.
    /// </para>
    /// </summary>
    public static class UIInitializer
    {
        /// <summary>
        /// Initializes the full UI hierarchy and returns the root UI node.
        /// <para>
        /// This method sets up the <see cref="UIRootNode"/>, dialog manager, navigation manager,
        /// registers all screens, and displays the initial screen.
        /// </para>
        /// </summary>
        /// <param name="sp">The <see cref="IServiceProvider"/> used for resolving required UI services.</param>
        /// <returns>The root UI node (<see cref="UIRootNode"/>) containing the full UI hierarchy.</returns>
        public static UIRootNode Initialize(IServiceProvider sp)
        {
            // Resolve the root UI node
            var _root = sp.GetRequiredService<UIRootNode>();

            // Resolve managers for dialogs and navigation
            var _dialogManager = sp.GetRequiredService<DialogManager>();
            var _navigationManager = sp.GetRequiredService<NavigationManager>();

            // Initialize managers with the root node
            _dialogManager.Init(_root);
            _navigationManager.Init(_root);

            // Resolve the UI factory used for creating screens
            var _factory = sp.GetRequiredService<IUiFactory>();

            // Create and register the main menu screen
            MainMenu _mainMenu = _factory.CreateDIElement<MainMenu, MainMenuHandler, MainMenuRenderer>();
            _navigationManager.RegisterScreen(_mainMenu);

            // Show the initial screen (MainMenu)
            _navigationManager.Show<MainMenu, MainMenuHandler, MainMenuRenderer>();

            return _root; // Return the fully initialized root UI node
        }
    }
}
