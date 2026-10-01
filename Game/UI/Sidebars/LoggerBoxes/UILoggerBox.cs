/* ----- ----- ----- ----- */
// UILoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    public class UILoggerBox : UITextBox<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>, IResettable
    {
        public UILoggerBox() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.LoggerBox.Layout;
            ScrollContainer.Layout = UILayoutConstants.Sidebar.LoggerBox.ScrollContainer.Layout;

            // Flex item of the sidebar column; the scroll area follows the box, inset by the
            // logger margin (see UILayoutSheet.GameScreen.LoggerBox*).
            LayoutRules.Apply(UILayoutSheet.GameScreen.LoggerBox);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.GameScreen.LoggerBoxScrollContainer);

            // The logger's own style settings (defined but never applied before).
            BackgroundColor = UILoggerBoxSettings.BackgroundColor;
            TextColor = UILoggerBoxSettings.TextColor;
            Font = UILoggerBoxSettings.Font;
        }

        protected override void OnReset()
        {
            ClearLogs();
        }
    }
}
