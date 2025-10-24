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

using Chinese_Chess_v3.Game.Constants.UI;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars
{
    /// <summary>
    /// Responsible for assembling and rendering the Sidebar UI,
    /// composed of InfoBoard (player info) and LoggerBox (game log).
    /// </summary>
    public class SidebarRenderer
    {
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

            float margin = 3.0f;
            RectangleF rect = new RectangleF(UILayoutConstants.Sidebar.Position.X + margin,
                UILayoutConstants.MainMenu.Position.Y + margin,
                _sidebar.Size.X - margin * 2,
                _sidebar.Size.Y - margin * 2);
            DrawOutline(g);
        }
        private void DrawOutline(Graphics g)
        {
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                float margin = 3.0f;
                debugPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(debugPen,
                UILayoutConstants.Sidebar.Position.X + margin,
                UILayoutConstants.Sidebar.Position.Y + margin,
                UILayoutConstants.Sidebar.Size.X - margin * 2,
                UILayoutConstants.Sidebar.Size.Y - margin * 2);
            }
        }
    }
}
