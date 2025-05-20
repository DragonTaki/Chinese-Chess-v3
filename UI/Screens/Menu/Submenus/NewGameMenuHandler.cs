/* ----- ----- ----- ----- */
// NewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Screens.Menu.Options;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class NewGameMenuHandler
    {
        private readonly NewGameMenu _menu;
        private readonly NavigationManager _navigation;

        public NewGameMenuHandler(IUiFactory factory, NewGameMenu menu)
        {
            _menu = menu;
            _navigation = factory.Resolve<NavigationManager>();
        }

        public void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
            //mainMenu.CancelCurrentSub_menu();
            _navigation.ShowGameScreen();
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