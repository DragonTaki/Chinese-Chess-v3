/* ----- ----- ----- ----- */
// GameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Menu;

namespace Chinese_Chess_v3.UI.Screens.Game
{
    /// <summary>
    /// Handles logic and interactions for the GameMenu.
    /// </summary>
    public class GameMenuHandler
    {
        private GameMenu menu;
        private NavigationManager _nav;

        public GameMenuHandler() {}
        public void Init(IUiFactory factory, GameMenu menu)
        {
            this.menu = menu;
            _nav = factory.Resolve<NavigationManager>();
        }

        public void GameMenuAction(GameMenuType selectedAction)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedAction}");
        }

        public void OnEnter()
        {
            //
        }
        
        public void OnExit()
        {
            _nav.ShowGameScreen();
        }
    }
}
