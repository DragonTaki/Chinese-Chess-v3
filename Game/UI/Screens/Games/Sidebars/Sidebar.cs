/* ----- ----- ----- ----- */
// Sidebar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Models;
using Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars
{
    /// <summary>
    /// Represents the logical data structure of the sidebar UI,
    /// storing both players' names, remaining time, and current turn.
    /// </summary>
    public class Sidebar : UIContainer<SidebarHandler>, IScreen, IDisposable
    {
        internal InfoBoard InfoBoard { get; private set; }
        internal LoggerBox LoggerBox { get; private set; }

        /// <summary>
        /// Indicates which side's turn it currently is.
        /// Accepts "Red" or "Black".
        /// </summary>
        public PlayerSide CurrentTurn { get; set; } = PlayerSide.Red;

        /// <summary>
        /// Indicates whether the sidebar should visually highlight the current turn.
        /// </summary>
        public bool HighlightTurn { get; set; } = true;

        /// <summary>
        /// Creates a new Sidebar with default player names and timers.
        /// </summary>
        public Sidebar() { }

        protected override void OnInit((IUiFactory, SidebarHandler) arg)
        {
            base.OnInit(arg);

            Layout = UILayoutConstants.Sidebar.Layout;

            _renderer = new SidebarRenderer(this);
        }

        protected override void BuildUIObjects()
        {
            if (InfoBoard == null)
                InfoBoard = _factory.CreateScreen<InfoBoard, InfoBoardHandler>();
            if (!Children.Contains(InfoBoard))
                AddChild(InfoBoard);

            if (LoggerBox == null)
                LoggerBox = _factory.CreateScreen<LoggerBox, LoggerBoxHandler>();
            if (!Children.Contains(LoggerBox))
                AddChild(LoggerBox);

            var gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();
            gameManager.SetLogger(LoggerBox.Handler);
            InfoBoard.Handler.SetGameManager(gameManager);
        }
        public void ResetGameUI() => BuildUIObjects();
    }
}
