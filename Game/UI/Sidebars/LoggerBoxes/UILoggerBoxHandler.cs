/* ----- ----- ----- ----- */
// UILoggerBoxHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.GameLog;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    /// <summary>
    /// Logger box handler; also the <see cref="IGameLog"/> sink the game log's composer writes to
    /// (AddMessage(string) is inherited from UITextBoxHandler).
    /// </summary>
    public class UILoggerBoxHandler : UITextBoxHandler<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>, IGameLog
    {
        public UILoggerBoxHandler() { }
    }
}
