/* ----- ----- ----- ----- */
// LoggerBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Screens.Games.Sidebars.LoggerBoxes
{
    /// <summary>
    /// Handles logic and interactions for the NewGameMenu.
    /// </summary>
    public class LoggerBoxHandler : InitializableOnceBase<(IUiFactory factory, LoggerBox loggerBox)>
    {
        private LoggerBox _loggerBox;
        private NavigationManager _nav;

        public LoggerBoxHandler() {}
        protected override void OnInit((IUiFactory factory, LoggerBox loggerBox) arg)
        {
            _loggerBox = arg.loggerBox;
            _nav = arg.factory.Resolve<NavigationManager>();
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