/* ----- ----- ----- ----- */
// UINewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;

using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UINewGameMenu.
    /// </summary>
    public class UINewGameMenuHandler : UIMenuHandler<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>
    {
        public UINewGameMenuHandler() { }

        public void StartNewGame(UINewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");

            var gameMenu = _navigationManager.Show<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>();
            var gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();

            gameMenu.ResetGameUI();  //reset gameManager
        }
    }
}
