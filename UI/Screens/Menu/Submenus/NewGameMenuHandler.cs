/* ----- ----- ----- ----- */
// NewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
    
using Chinese_Chess_v3.UI.Menu;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class NewGameMenuHandler
    {
        private readonly NewGameMenu newGameMenu;
        private readonly Action cancelMainMenu;
        public NewGameMenuHandler(NewGameMenu newGameMenu, Action cancelMainMenu)
        {
            this.newGameMenu = newGameMenu;
            this.cancelMainMenu = cancelMainMenu;
        }

        public void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
            cancelMainMenu?.Invoke();
            //mainMenu.CancelCurrentSubmenu();
        }
    }
}