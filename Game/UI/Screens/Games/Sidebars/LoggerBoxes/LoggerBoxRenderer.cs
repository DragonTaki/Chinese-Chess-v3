/* ----- ----- ----- ----- */
// LoggerBoxRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Constants.UI;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{
    public class LoggerBoxRenderer
    {
        private LoggerBox _loggerBox;

        public LoggerBoxRenderer(LoggerBox loggerBox)
        {
            _loggerBox = loggerBox;
        }

        public void Draw(Graphics g)
        {
            using var bgBrush = new SolidBrush(Color.Red);
            g.FillRectangle(bgBrush, UILayoutConstants.Sidebar.LoggerBox.Layout);
        }
    }
}
