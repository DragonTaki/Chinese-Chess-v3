/* ----- ----- ----- ----- */
// Navigator.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.UI.Core.Bases;
using Engine.UI.Infrastructure;

namespace Chinese_Chess_v3.Game.UI.Navigation
{
    /// <summary>
    /// The UI's <see cref="INavigator"/>: maps each <see cref="ScreenId"/> to its screen type
    /// and shows it through the engine's <see cref="NavigationManager"/>.
    /// </summary>
    public sealed class Navigator : INavigator
    {
        private readonly NavigationManager _navigationManager;

        public Navigator(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        }

        public void Show(ScreenId screen)
        {
            switch (screen)
            {
                case ScreenId.MainMenu:
                    _navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();
                    break;
                case ScreenId.Game:
                    _navigationManager.Show<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(screen), screen, "Unknown screen");
            }
        }

        public bool IsShown(ScreenId screen)
        {
            UIElementBase instance = screen switch
            {
                ScreenId.MainMenu => _navigationManager.GetScreen<UIMainMenu>(),
                ScreenId.Game => _navigationManager.GetScreen<UIGameMenu>(),
                _ => throw new ArgumentOutOfRangeException(nameof(screen), screen, "Unknown screen"),
            };
            // A screen another Show replaced is off the root (Parent null) but may still be IsVisible.
            return instance != null && instance.Parent != null && instance.IsVisible;
        }
    }
}
