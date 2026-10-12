/* ----- ----- ----- ----- */
// UIKindMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/12
// Version: v1.5
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Core;

using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    /// <summary>
    /// Routes each game-kind button to the main menu presenter according to the submenu's purpose.
    /// Unwired flows keep the normal buttons and leave the current menu open.
    /// </summary>
    public class UIKindMenuHandler : UIMenuHandler<UIKindMenu, UIKindMenuHandler, UIKindMenuRenderer>
    {
        public UIKindMenuHandler() { }

        /// <summary>The main menu's decisions (the <see cref="MainMenuPresenter"/> registered in DI).</summary>
        private MainMenuPresenter Presenter => _factory.ServiceProvider.GetRequiredService<MainMenuPresenter>();

        /// <summary>Passes the chosen kind to the presenter for this submenu's purpose.</summary>
        /// <param name="kind">The chosen game kind.</param>
        public void ChooseKind(GameKind kind)
        {
            switch (Element.Purpose)
            {
                case KindMenuPurpose.SinglePlayer:
                    Presenter.StartSinglePlayer(kind);
                    break;
                case KindMenuPurpose.VsComputer:
                    Presenter.StartVsComputer(kind);
                    break;
            }
        }
    }
}
