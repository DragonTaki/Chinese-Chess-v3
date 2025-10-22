/* ----- ----- ----- ----- */
// SidebarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;
using Chinese_Chess_v3.Constants.UI;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.LoggerBoxes;

namespace Chinese_Chess_v3.UI.Screens.Games.Sidebars
{
    /// <summary>
    /// Responsible for assembling and rendering the Sidebar UI,
    /// composed of InfoBoard (player info) and LoggerBox (game log).
    /// </summary>
    public class SidebarRenderer
    {
        private readonly InfoBoard _infoBoard;
        private readonly LoggerBox _loggerBox;
        private readonly Sidebar _sidebar;

        public SidebarRenderer(Sidebar sidebar)
        {
            _sidebar = sidebar;
        }

        /// <summary>
        /// Called each frame before rendering child elements.
        /// Can be used to update transitions, highlight effects, etc.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last frame.</param>
        public void OnUpdate()
        {
            // Example: animate highlight or blink current turn
        }

        /// <summary>
        /// Renders the sidebar’s background and layout.
        /// </summary>
        /// <param name="g">Graphics context used for drawing.</param>
        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawOutline(g);
            // Draw sidebar background
            using var bgBrush = new SolidBrush(Color.Red);
            g.FillRectangle(bgBrush, UILayoutConstants.Sidebar.Layout);

            // Draw divider line between InfoBoard and LoggerBox (optional)
            // g.DrawLine(Pens.Gray, new Point(0, _sidebar.InfoBoardHeight), new Point(_sidebar.Width, _sidebar.InfoBoardHeight));
        }
        private void DrawOutline(Graphics g)
        {
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                float margin = 3.0f;
                debugPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(debugPen,
                UILayoutConstants.MainMenu.Position.X + margin,
                UILayoutConstants.MainMenu.Position.Y + margin,
                _sidebar.Size.X - margin * 2,
                _sidebar.Size.Y - margin * 2);
            }
        }
    }
}
