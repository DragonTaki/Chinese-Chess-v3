/* ----- ----- ----- ----- */
// GameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.UI.Screens.Games.Options;
using Chinese_Chess_v3.Game.UI.Screens.Menus;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus;
using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games
{
    /// <summary>
    /// Handles logic and interactions for the GameMenu.
    /// </summary>
    public class GameMenuHandler : UIMenuHandler<GameMenu, GameMenuHandler, GameMenuRenderer>
    {
        public GameMenuHandler() { }

        public void GameMenuAction(GameMenuType selectedAction)
        {
            Console.WriteLine($"GameMenu: selected: {selectedAction}");
            switch (selectedAction)
            {
                case GameMenuType.Default:
                    break;
                case GameMenuType.Restart:
                    break;
                case GameMenuType.Undo:
                    break;
                case GameMenuType.SaveGame:
                    break;
                case GameMenuType.LoadLayout:
                    break;
                case GameMenuType.Surrender:
                    break;
                case GameMenuType.ReturnToMain:
                    _navigationManager.Show<MainMenu, MainMenuHandler, MainMenuRenderer>();
                    break;
                default:
                    break;
            }
        }
    }
}
