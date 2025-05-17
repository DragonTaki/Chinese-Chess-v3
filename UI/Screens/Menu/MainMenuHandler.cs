/* ----- ----- ----- ----- */
// MainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Menu;
using Chinese_Chess_v3.UI.Screens.Menu.Submenus;

namespace Chinese_Chess_v3.UI.Screens.Menu
{
    /// <summary>
    /// Handles logic and interactions for the MainMenu.
    /// </summary>
    public class MainMenuHandler
    {
        private readonly MainMenu mainMenu;
        private MainMenuType? currentSubmenu = null;
        private readonly IUiFactory uiFactory;
        private readonly Dictionary<MainMenuType, UIElement> submenus = new();

        public MainMenuHandler(MainMenu mainMenu, IUiFactory uiFactory)
        {
            this.mainMenu = mainMenu;
            this.uiFactory = uiFactory;

            // Initialize submenus
            //submenus[MainMenuType.NewGame] = CreateSubMenu<NewGameMenu>();
            submenus[MainMenuType.NewGame] = CreateSubMenu(() =>
            {
                var menu = uiFactory.Create<NewGameMenu>();
                var handler = new NewGameMenuHandler(menu, CancelCurrentSubmenu);
                menu.Setup(handler.StartNewGame);
                return menu;
            });
            submenus[MainMenuType.LoadGame]     = CreateSubMenu(() => uiFactory.Create<LoadGameMenu>());
            submenus[MainMenuType.RuleSettings] = CreateSubMenu(() => uiFactory.Create<LoadGameMenu>());
            submenus[MainMenuType.Help]         = CreateSubMenu(() => uiFactory.Create<LoadGameMenu>());
            submenus[MainMenuType.Settings]     = CreateSubMenu(() => uiFactory.Create<LoadGameMenu>());
        }

        /// <summary>
        /// Create a submenu and set it to invisible by default.
        /// </summary>
        private static UIElement CreateSubMenu<T>() where T : UIElement, new()
        {
            var menu = new T();
            menu.IsVisible = false;
            return menu;
        }
        private static UIElement CreateSubMenu(Func<UIElement> factory)
        {
            var menu = factory();
            menu.IsVisible = false;
            return menu;
        }

        /// <summary>
        /// Switch to the selected submenu. Clicking the same submenu closes it.
        /// </summary>
        public void SwitchSubmenu(MainMenuType selectedMenu)
        {
            CancelCurrentSubmenu();

            if (currentSubmenu == selectedMenu)
            {
                // Same menu clicked again, collapse
                currentSubmenu = null;
            }
            else
            {
                // Show new submenu
                currentSubmenu = selectedMenu;
                var submenu = submenus[currentSubmenu.Value];
                submenu.IsVisible = true;
                mainMenu.AddChild(submenu);
            }
        }

        /// <summary>
        /// Cancel and remove current submenu from the view.
        /// </summary>
        public void CancelCurrentSubmenu()
        {
            if (currentSubmenu.HasValue)
            {
                var submenu = submenus[currentSubmenu.Value];
                submenu.IsVisible = false;
                mainMenu.RemoveChild(submenu);
                currentSubmenu = null;
            }
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public void ExitApplication()
        {
            Application.Exit();
        }

        public Dictionary<MainMenuType, UIElement> Submenus => submenus;
        public MainMenuType? CurrentSubmenu => currentSubmenu;
    }
}
