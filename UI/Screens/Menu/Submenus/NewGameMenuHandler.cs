/* ----- ----- ----- ----- */
// NewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Menu;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class NewGameMenuHandler
    {
        private readonly NewGameMenu menu;
        private readonly NavigationManager navigation;

        public NewGameMenuHandler(IUiFactory factory, NewGameMenu menu)
        {
            this.menu = menu;
            this.navigation = factory.Resolve<NavigationManager>();
        }

        public void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
            //mainMenu.CancelCurrentSubmenu();
            navigation.ShowGameScreen();
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