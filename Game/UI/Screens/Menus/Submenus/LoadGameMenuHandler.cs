/* ----- ----- ----- ----- */
// LoadGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the LoadGameMenu.
    /// </summary>
    public class LoadGameMenuHandler : UIMenuHandler<LoadGameMenu, LoadGameMenuHandler, LoadGameMenuRenderer>
    {
        public LoadGameMenuHandler() { }

        public void StartNewGame()
        {
            Console.WriteLine($"LoadGameMenu: selected");
            //mainMenu.CancelCurrentSub_menu();
        }
    }
}