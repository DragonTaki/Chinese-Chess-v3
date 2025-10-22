/* ----- ----- ----- ----- */
// InfoBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards
{
    /// <summary>
    /// Handles the data and logic of the InfoBoard.
    /// </summary>
    public class InfoBoardHandler : InitializableOnceBase<(IUiFactory factory, InfoBoard infoBoard)>
    {
        private NavigationManager _nav;
        private InfoBoard _infoBoard;
        public string BlackPlayerName { get; set; } = "黑方玩家";
        public string RedPlayerName { get; set; } = "紅方玩家";

        public TimeSpan BlackTime { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RedTime { get; set; } = TimeSpan.FromMinutes(5);

        public PlayerSide CurrentTurn { get; set; } = PlayerSide.Red;

        public InfoBoardHandler() {}
        protected override void OnInit((IUiFactory factory, InfoBoard infoBoard) arg)
        {
            _infoBoard = arg.infoBoard;
            _nav = arg.factory.Resolve<NavigationManager>();
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
        public void OnEnter()
        {
            //
        }
        
        public void OnExit()
        {
            //
        }
    }
}
