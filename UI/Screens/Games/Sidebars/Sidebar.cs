/* ----- ----- ----- ----- */
// Sidebar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using Chinese_Chess_v3.Constants.UI;
using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.LoggerBoxes;

namespace Chinese_Chess_v3.UI.Screens.Games.Sidebars
{
    /// <summary>
    /// Represents the logical data structure of the sidebar UI,
    /// storing both players' names, remaining time, and current turn.
    /// </summary>
    public class Sidebar
        : InitializableOnceElement<(IUiFactory factory, SidebarHandler handler, SidebarRenderer renderer)>
        , IScreen
    {
        private SidebarHandler _handler;
        private SidebarRenderer _renderer;
        private InfoBoard _infoBoard;
        private LoggerBox _loggerBox;

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
        public Sidebar() {}
        protected override void OnInit((IUiFactory factory, SidebarHandler handler, SidebarRenderer renderer) arg)
        {
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.Sidebar.Position;
            Size = UILayoutConstants.Sidebar.Size;

            _infoBoard = arg.factory.CreateInfoBoard();
            AddChild(_infoBoard);
            _loggerBox = arg.factory.CreateLoggerBox();
            AddChild(_loggerBox);
        }

        public void OnEnter()
        {
            _handler.OnEnter();
        }

        public void OnExit()
        {
            _handler.OnExit();
        }

        /// <summary>
        /// Updates the sidebar each frame (delegates to handler and renderer).
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last frame.</param>
        protected override void OnUpdate()
        {
            _handler.OnUpdate();
            _renderer.OnUpdate();
        }

        /// <summary>
        /// Draws the sidebar (delegates to renderer).
        /// </summary>
        /// <param name="g">Graphics context.</param>
        protected override void OnDraw(Graphics g)
        {
            _renderer.Draw(g);
        }
    }
}
