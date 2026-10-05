/* ----- ----- ----- ----- */
// UINewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;

using Engine.Diagnostics;
using Engine.Logging;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UINewGameMenu: each mode button starts that
    /// mode's game on the game screen (<see cref="GameSession.StartNew"/>: 傳統大盤 the standard
    /// position, 暗棋／明棋半盤 a shuffled HalfCenter game, <see cref="GameManager.StartHalfCenter"/>).
    /// A mode without a game yet (揭棋大盤, 三國半盤, <see cref="GameSession.CanStartNew"/>) shows a
    /// message and stays on this menu.
    /// </summary>
    public class UINewGameMenuHandler : UIMenuHandler<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>
    {
        public UINewGameMenuHandler() { }

        /// <summary>The app's confirm dialogs (the <see cref="IDialogService"/> registered in DI).</summary>
        private IDialogService Dialogs => _factory.ServiceProvider.GetRequiredService<IDialogService>();

        /// <summary>The game flow (the <see cref="GameSession"/> registered in DI).</summary>
        private GameSession Session => _factory.ServiceProvider.GetRequiredService<GameSession>();

        public void StartNewGame(GameKind kind)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"NewGameMenu: selected: {kind}");

            if (!GameSession.CanStartNew(kind))
            {
                AppLogger.Log($"(NewGame) {kind} is not implemented yet; staying on the new-game menu", LogLevel.WARN);
                Dialogs.ShowConfirm(MenuTexts.NewGameModeUnavailable(NewGameOptions.LabelOf(kind)), ConfirmDialogType.Ok, _ => { });
                return;
            }

            // Game screen, restart (views reset, log cleared), then the chosen mode's game.
            Session.StartNew(kind);
        }
    }
}
