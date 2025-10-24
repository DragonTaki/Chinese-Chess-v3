/* ----- ----- ----- ----- */
// NewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.UI.Screens.Games;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class NewGameMenuHandler : UIMenuHandler<NewGameMenuHandler>
    {

        public NewGameMenuHandler() {}

        public void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
            //mainMenu.CancelCurrentSub_menu();
            var gameMenu = _navigationManager.Show<GameMenu, GameMenuHandler>();
            gameMenu.ResetGameUI();
        }
    }
}