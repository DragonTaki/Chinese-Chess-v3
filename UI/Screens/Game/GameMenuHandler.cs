/* ----- ----- ----- ----- */
// GameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Menu;

namespace Chinese_Chess_v3.UI.Screens.Game
{
    /// <summary>
    /// Handles logic and interactions for the GameMenu.
    /// </summary>
    public class GameMenuHandler
    {
        private readonly GameMenu GameMenu;

        public GameMenuHandler(GameMenu GameMenu)
        {
            this.GameMenu = GameMenu;
        }

        public void GameMenuAction(GameMenuType selectedAction)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedAction}");
        }
    }
}
