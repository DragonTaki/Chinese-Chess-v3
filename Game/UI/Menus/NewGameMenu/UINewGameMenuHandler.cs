/* ----- ----- ----- ----- */
// UINewGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/01
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;

using Engine.Logging;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UINewGameMenu: each mode button starts that
    /// mode's game on the game screen (傳統大盤 the standard position, 暗棋／明棋半盤 a shuffled
    /// HalfCenter game, <see cref="GameManager.StartHalfCenter"/>). A mode without a game yet
    /// (揭棋大盤, 三國半盤) shows a message and stays on this menu.
    /// </summary>
    public class UINewGameMenuHandler : UIMenuHandler<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>
    {
        public UINewGameMenuHandler() { }

        public void StartNewGame(UINewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewGameMenu: selected: {selectedGamemode}");

            // The game set-up per mode; null for a mode that cannot be played yet.
            Action<GameManager> start = selectedGamemode switch
            {
                UINewGameMenuType.Default or UINewGameMenuType.Traditional => game => game.ResetBoardToDefault(),
                UINewGameMenuType.DarkHalf => game => game.StartHalfCenter(hiddenChess: true),
                UINewGameMenuType.OpenHalf => game => game.StartHalfCenter(hiddenChess: false),
                // 揭棋: Board.IsJieqi has no start position yet; 三國: its own rule system is
                // still being specified (docs/DARK-CHESS-RULES.md §1.2).
                UINewGameMenuType.FlipChess or UINewGameMenuType.ThreeKingdomsHalf => null,
                _ => throw new ArgumentOutOfRangeException(nameof(selectedGamemode), selectedGamemode, "Unknown new-game mode"),
            };
            if (start == null)
            {
                AppLogger.Log($"(NewGame) {selectedGamemode} is not implemented yet; staying on the new-game menu", LogLevel.WARN);
                DialogManager.ShowConfirm(GameMenuTexts.NewGameModeUnavailable(LabelOf(selectedGamemode)), ConfirmDialogType.Ok, _ => { });
                return;
            }

            var gameMenu = _navigationManager.Show<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>();
            var gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();

            // Reset first (clears the log, restarts the board), then the chosen mode's game.
            gameMenu.ResetGameUI();
            start(gameManager);
        }

        /// <summary>The button text of <paramref name="mode"/> (see <see cref="UINewGameMenuOptions"/>).</summary>
        private static string LabelOf(UINewGameMenuType mode) =>
            UINewGameMenuOptions.Create(_ => { }).Find(e => e.Type == mode)?.Label ?? mode.ToString();
    }
}
