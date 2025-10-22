/* ----- ----- ----- ----- */
// LoadGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Screens.Menus.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the LoadGameMenu.
    /// </summary>
    public class LoadGameMenuHandler
    {
        private readonly LoadGameMenu _menu;
        private readonly NavigationManager _nav;

        public LoadGameMenuHandler(IUiFactory factory, LoadGameMenu menu)
        {
            _menu = menu;
            _nav = factory.Resolve<NavigationManager>();
        }

        public void StartNewGame()
        {
            Console.WriteLine($"LoadGameMenu: selected");
            //mainMenu.CancelCurrentSub_menu();
        }

        public void OnEnter()
        {
            //
        }
        
        public void OnExit()
        {
            //
        }
    }
}