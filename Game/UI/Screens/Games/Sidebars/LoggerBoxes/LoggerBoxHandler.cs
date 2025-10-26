/* ----- ----- ----- ----- */
// LoggerBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{

    /// <summary>
    /// Handles logic and interactions for the LoggerBox.
    /// 
    /// NOTE: This is currently an exception handler using delayed OnInit pattern.
    /// In the future, this should be refactored to fully use Engine+Game layer separation.
    /// </summary>
    public class LoggerBoxHandler : UITextBoxHandler<LoggerBoxHandler>
    {
        public LoggerBoxHandler() { }
    }
}