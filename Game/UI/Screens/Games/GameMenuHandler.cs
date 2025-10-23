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

using Engine.UI.Core.Base;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games
{
    /// <summary>
    /// Handles logic and interactions for the GameMenu.
    /// </summary>
    public class GameMenuHandler : InitializableOnceBase<(IUiFactory factory, GameMenu menu)>
    {
        private GameMenu _menu;
        private NavigationManager _navigationManager;

        public GameMenuHandler() {}
        protected override void OnInit((IUiFactory factory, GameMenu menu) arg)
        {
            _menu = arg.menu;
            _navigationManager = arg.factory.Resolve<NavigationManager>();
        }

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
                    _menu.DisposeChildren();

                    // 4. 導航回主選單
                    _navigationManager.Show<MainMenu, MainMenuHandler, MainMenuRenderer>();
                    break;
                default:
                    break;
            }
        }

        public void OnEnter()
        {
            //
        }
        
        public void OnExit()
        {
            _navigationManager.Show<GameMenu, GameMenuHandler, GameMenuRenderer>();
        }
    }
}
