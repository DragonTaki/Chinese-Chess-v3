/* ----- ----- ----- ----- */
// UILoadGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UILoadGameMenu.
    /// </summary>
    public class UILoadGameMenuHandler : UIMenuHandler<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>
    {
        public UILoadGameMenuHandler() { }

        public void StartNewGame()
        {
            Console.WriteLine($"UILoadGameMenu: selected");
            //mainMenu.CancelCurrentSub_menu();
        }
    }
}