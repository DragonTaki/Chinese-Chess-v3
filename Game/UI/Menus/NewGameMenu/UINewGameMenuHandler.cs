/* ----- ----- ----- ----- */
// UINewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.3
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Core;

using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    /// <summary>
    /// Binds the UINewGameMenu (開新一局) to the <see cref="MainMenuPresenter"/>: each mode
    /// button calls <see cref="MainMenuPresenter.StartNewGame"/>, which starts that mode's game
    /// on the game screen, or, for a mode without a game yet (揭棋大盤, 三國半盤), shows a
    /// message and stays on this menu.
    /// </summary>
    public class UINewGameMenuHandler : UIMenuHandler<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>
    {
        public UINewGameMenuHandler() { }

        /// <summary>The main menu's decisions (the <see cref="MainMenuPresenter"/> registered in DI).</summary>
        private MainMenuPresenter Presenter => _factory.ServiceProvider.GetRequiredService<MainMenuPresenter>();

        /// <summary>A mode button: passed to the presenter (<see cref="MainMenuPresenter.StartNewGame"/>).</summary>
        public void StartNewGame(GameKind kind) => Presenter.StartNewGame(kind);
    }
}
