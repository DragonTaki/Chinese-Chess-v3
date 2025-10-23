/* ----- ----- ----- ----- */
// LoggerBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Base;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class LoggerBoxHandler : InitializableOnceBase<(IUiFactory factory, LoggerBox loggerBox)>
    {
        private LoggerBox _loggerBox;
        private NavigationManager _navigationManager;

        public LoggerBoxHandler() {}
        protected override void OnInit((IUiFactory factory, LoggerBox loggerBox) arg)
        {
            _loggerBox = arg.loggerBox;
            _navigationManager = arg.factory.Resolve<NavigationManager>();
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