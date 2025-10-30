/* ----- ----- ----- ----- */
// UIGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UIGameMenu.
    /// </summary>
    public class UIGameMenuHandler : UIMenuHandler<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>
    {
        public UIGameMenuHandler() { }

        public void UIGameMenuAction(UIGameMenuType selectedAction)
        {
            Console.WriteLine($"UIGameMenu: selected: {selectedAction}");
            switch (selectedAction)
            {
                case UIGameMenuType.Default:
                    break;
                case UIGameMenuType.Restart:
                    break;
                case UIGameMenuType.Undo:
                    break;
                case UIGameMenuType.SaveGame:
                    break;
                case UIGameMenuType.LoadLayout:
                    break;
                case UIGameMenuType.Surrender:
                    break;
                case UIGameMenuType.ReturnToMain:
                    _navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();
                    break;
                default:
                    break;
            }
        }
    }
}
