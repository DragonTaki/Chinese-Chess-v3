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

using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class NewGameMenuHandler
    {
        private readonly NewGameMenu _menu;
        private readonly NavigationManager _navigationManager;

        public NewGameMenuHandler(IUiFactory factory, NewGameMenu menu)
        {
            _menu = menu;
            _navigationManager = factory.Resolve<NavigationManager>();
        }

        public void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
            //mainMenu.CancelCurrentSub_menu();
            _navigationManager.Show<GameMenu, GameMenuHandler, GameMenuRenderer>();
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