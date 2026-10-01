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

using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Infrastructure;

namespace Launcher
{
    /// <summary>
    /// Responsible for initializing and connecting all UI components.
    /// <para>
    /// Acts as the bootstrapper for the UI system: the root node, the main menu screen,
    /// the navigation system, and dialog management (the other screens, e.g. GameMenu, are
    /// created on demand when navigated to).
    /// </para>
    /// </summary>
    public static class UIInitializer
    {
        /// <summary>
        /// Initializes the full UI hierarchy and returns the root UI node.
        /// <para>
        /// This method sets up the <see cref="UIRootNode"/>, dialog manager, navigation manager,
        /// registers the main menu screen, and displays it as the initial screen.
        /// </para>
        /// </summary>
        /// <param name="sp">The <see cref="IServiceProvider"/> used for resolving required UI services.</param>
        /// <returns>The root UI node (<see cref="UIRootNode"/>) containing the full UI hierarchy.</returns>
        public static UIRootNode Initialize(IServiceProvider sp)
        {
            // Resolve the root UI node
            var root = sp.GetRequiredService<UIRootNode>();

            // Resolve managers for dialogs and navigation
            var dialogManager = sp.GetRequiredService<DialogManager<UIConfirmDialog>>();
            var navigationManager = sp.GetRequiredService<NavigationManager>();

            // Initialize managers with the root node
            dialogManager.Init(root);
            navigationManager.Init(root);

            // Resolve the UI factory used for creating screens
            var factory = sp.GetRequiredService<IUiFactory>();

            // Create and register the main menu screen
            UIMainMenu mainMenu = factory.CreateDIElement<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();
            navigationManager.RegisterScreen(mainMenu);

            // Show the initial screen (MainMenu)
            navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();

            return root; // Return the fully initialized root UI node
        }
    }
}
