/* ----- ----- ----- ----- */
// UIInitializer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Launcher.Cross
{
    /// <summary>
    /// Cross-platform counterpart to <c>Launcher.UIInitializer</c> (identical
    /// logic — kept as a separate copy purely because the two entry points
    /// live in mutually TFM-excluded folders and can't share a file).
    /// </summary>
    public static class UIInitializer
    {
        public static UIRootNode Initialize(IServiceProvider sp)
        {
            var root = sp.GetRequiredService<UIRootNode>();

            var dialogManager = sp.GetRequiredService<DialogManager<UIConfirmDialog>>();
            var navigationManager = sp.GetRequiredService<NavigationManager>();

            dialogManager.Init(root);
            navigationManager.Init(root);

            var factory = sp.GetRequiredService<IUiFactory>();

            UIMainMenu mainMenu = factory.CreateDIElement<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();
            navigationManager.RegisterScreen(mainMenu);

            navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();

            return root;
        }
    }
}
