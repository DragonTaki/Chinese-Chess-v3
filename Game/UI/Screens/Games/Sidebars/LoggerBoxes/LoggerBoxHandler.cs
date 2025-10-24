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
using Engine.UI.Elements;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{

    /// <summary>
    /// Handles logic and interactions for the LoggerBox.
    /// 
    /// NOTE: This is currently an exception handler using delayed OnInit pattern.
    /// In the future, this should be refactored to fully use Engine+Game layer separation.
    /// </summary>
    public class LoggerBoxHandler : InitializableOnceBase<(IUiFactory factory, LoggerBox loggerBox)>
    {
        #region Fields

        private LoggerBox _loggerBox;
        
        /// <summary>
        /// The actual engine layer UI element used for rendering and input.
        /// Initialized in OnInit as an exception to the standard factory pattern.
        /// </summary>
        private UITextBox _uiTextBox;

        private NavigationManager _navigationManager;

        #endregion

        #region Constructor

        public LoggerBoxHandler() { }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the handler using the factory and game LoggerBox.
        /// </summary>
        /// <param name="arg">Tuple containing IUiFactory and LoggerBox</param>
        protected override void OnInit((IUiFactory factory, LoggerBox loggerBox) arg)
        {
            _loggerBox = arg.loggerBox;

            // Create UITextBox via factory (exception handling for now)
            //_uiTextBox = arg.factory.Create<UITextBox>();

            _navigationManager = arg.factory.Resolve<NavigationManager>();
        }

        #endregion

        #region Lifecycle Hooks

        public void OnEnter()
        {
            // Called when entering the sidebar or game state
        }

        public void OnExit()
        {
            // Called when exiting the sidebar or game state
        }

        #endregion

        #region Notes

        /*
         * TODO: Refactor LoggerBoxHandler to fully follow Engine+Game separation:
         * - Game layer should wrap Engine UITextBox
         * - Initialization should be consistent with other game UI elements
         * - Remove exception handling once refactor is complete
         */

        #endregion
    }
}