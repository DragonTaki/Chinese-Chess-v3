/* ----- ----- ----- ----- */
// InfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Models;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
{
    public class InfoBoard : UIContainer<InfoBoard, InfoBoardHandler, InfoBoardRenderer>, IResettable
    {
        public string BlackPlayerName { get; set; } = "黑方玩家";
        public string RedPlayerName { get; set; } = "紅方玩家";

        public TimeSpan BlackTotalTime { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan RedTotalTime { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan BlackStepTime { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RedStepTime { get; set; } = TimeSpan.FromMinutes(5);

        public PlayerSide CurrentTurn { get; set; } = PlayerSide.Red;
        public GameManager GameManager;

        public InfoBoard() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.Infoboard.Layout;
        }
    }
}
