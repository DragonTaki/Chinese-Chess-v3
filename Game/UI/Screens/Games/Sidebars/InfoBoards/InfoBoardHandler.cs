/* ----- ----- ----- ----- */
// InfoBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Models;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
{
    /// <summary>
    /// Handles the data and logic of the InfoBoard.
    /// </summary>
    public class InfoBoardHandler : UIContainerHandler<InfoBoardHandler>
    {
        public string BlackPlayerName { get; set; } = "黑方玩家";
        public string RedPlayerName { get; set; } = "紅方玩家";

        public TimeSpan BlackTime { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RedTime { get; set; } = TimeSpan.FromMinutes(5);

        public PlayerSide CurrentTurn { get; set; } = PlayerSide.Red;
        private GameManager _gameManager;
        public GameManager GameManager => _gameManager;

        public InfoBoardHandler() { }
        public void SetGameManager(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        /// <summary>
        /// Updates timers based on delta time.
        /// </summary>
        public void UpdateTimers(TimeSpan delta)
        {
            if (CurrentTurn == PlayerSide.Black)
                BlackTime -= delta;
            else
                RedTime -= delta;
        }

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == PlayerSide.Black ? PlayerSide.Red : PlayerSide.Black;
        }
    }
}
