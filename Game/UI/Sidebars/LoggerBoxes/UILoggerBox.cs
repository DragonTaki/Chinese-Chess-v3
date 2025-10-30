/* ----- ----- ----- ----- */
// UILoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Interfaces;
using Engine.UI.Elements;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    public class UILoggerBox : UITextBox<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>, IResettable
    {
        public UILoggerBox() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.LoggerBox.Layout;
            ScrollContainer.Layout = UILayoutConstants.Sidebar.LoggerBox.ScrollContainer.Layout;
        }

        protected override void OnReset()
        {
            ClearLogs();
        }
    }
}
