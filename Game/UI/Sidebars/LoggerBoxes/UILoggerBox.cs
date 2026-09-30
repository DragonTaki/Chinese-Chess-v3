/* ----- ----- ----- ----- */
// UILoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Constants.Core;
using Engine.UI.Core.Interfaces;
using Engine.UI.Elements;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    public class UILoggerBox : UITextBox<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>, IResettable
    {
        public UILoggerBox() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.LoggerBox.Layout;
            ScrollContainer.Layout = UILayoutConstants.Sidebar.LoggerBox.ScrollContainer.Layout;

            // Flex item of the sidebar column: full width, today's fixed height (placed at
            // the bottom by the sidebar's SpaceBetween). Growing into the remaining height is
            // for the full-window switch - it would change today's 200px box.
            LayoutRules.PositionMode = PositionMode.Flow;
            LayoutRules.Width = LayoutSize.Stretch;
            LayoutRules.Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.LoggerBox.Size.Y);
            LayoutRules.FlexShrink = 0f;

            // The scroll area follows the box, inset by the logger margin.
            var scrollRules = ScrollContainer.LayoutRules;
            scrollRules.PositionMode = PositionMode.Absolute;
            scrollRules.Left = UILayoutConstants.Sidebar.LoggerBox.Margin;
            scrollRules.Top = UILayoutConstants.Sidebar.LoggerBox.Margin;
            scrollRules.Right = UILayoutConstants.Sidebar.LoggerBox.Margin;
            scrollRules.Bottom = UILayoutConstants.Sidebar.LoggerBox.Margin;

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
