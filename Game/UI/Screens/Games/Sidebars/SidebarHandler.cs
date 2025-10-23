/* ----- ----- ----- ----- */
// SidebarHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes;

using Engine.UI.Core.Base;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars
{
    /// <summary>
    /// Handles runtime logic for the Sidebar,
    /// such as time countdown, turn switching, and state synchronization.
    /// </summary>
    public class SidebarHandler : InitializableOnceBase<(IUiFactory factory, Sidebar sidebar)>
    {
        private Sidebar _sidebar;
        private NavigationManager _navigationManager;
        private InfoBoard _infoBoard;
        private LoggerBox _loggerBox;
        
        private readonly GameManager _gameManager;

        /// <summary>
        /// Initializes a new SidebarHandler for the given Sidebar instance.
        /// </summary>
        public SidebarHandler(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        protected override void OnInit((IUiFactory factory, Sidebar sidebar) arg)
        {
            _sidebar = arg.sidebar;

            _navigationManager = arg.factory.Resolve<NavigationManager>();
        }

        public void OnEnter()
        {
            //
        }

        /// <summary>
        /// Called when Sidebar is hidden or exited.
        /// </summary>
        public void OnExit()
        {
            // Cleanup or pause timers if needed
        }

        /// <summary>
        /// Updates Sidebar logic per frame.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last frame.</param>
        public void OnUpdate()
        {
            // Example: update timers or current turn state
            // _gameManager.RedTimer -= deltaTime;
        }
    }
}
