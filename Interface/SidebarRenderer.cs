/* ----- ----- ----- ----- */
// SidebarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Interface
{
    public class SidebarRenderer
    {
        public void DrawSidebar(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            using (HatchBrush woodBrush = new HatchBrush(
                HatchStyle.LightVertical,
                Color.FromArgb(255, 194, 138), // 深色線條
                Color.FromArgb(255, 222, 173)  // 淺色底色
            ))
            {
                g.FillRectangle(
                    woodBrush,
                    SidebarSettings.StartX,
                    SidebarSettings.StartY,
                    SidebarSettings.Width,
                    SidebarSettings.Height
                );
            }

        }
    }
}