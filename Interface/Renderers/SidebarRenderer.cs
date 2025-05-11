/* ----- ----- ----- ----- */
// SidebarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/08
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs.Sidebar;
using Chinese_Chess_v3.Utils.GraphicsUtils;

using SharedLib.Timing;

namespace Chinese_Chess_v3.Interface.Renderers
{
    public class SidebarRenderer
    {
        private readonly List<Star> stars = new List<Star>();
        private readonly Timer animationTimer;
        private readonly Random rand = new Random();
        private readonly int maxStars = 60;
        public SidebarRenderer()
        {
            for (int i = 0; i < maxStars; i++)
                stars.Add(new Star(rand));

            animationTimer = new Timer { Interval = TimerSettings.GameAnimationInterval };
            animationTimer.Tick += (s, e) => UpdateStars();
            animationTimer.Start();
        }

        public void DrawSidebar(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            int x = SidebarSettings.SidebarStartX;
            int y = SidebarSettings.SidebarStartY;
            int w = SidebarSettings.SidebarWidth;
            int h = SidebarSettings.SidebarHeight;

            // Fill black background
            g.FillRectangle(Brushes.Black, x, y, w, h);

            // Draw stars
            foreach (var star in stars)
            {
                float alpha = Math.Max(0.2f, (float)rand.NextDouble());
                Color starColor = Color.FromArgb((int)(alpha * 255), 255, 255, 255);
                using (Brush brush = new SolidBrush(starColor))
                {
                    g.FillEllipse(brush, x + star.X, y + star.Y, star.Size, star.Size);
                }
            }
        }

        private void UpdateStars()
        {
            foreach (var star in stars)
            {
                star.Y += star.Speed;
                if (star.Y > SidebarSettings.SidebarHeight)
                {
                    star.X = rand.Next(SidebarSettings.SidebarWidth);
                    star.Y = 0;
                    star.Size = rand.Next(1, 3);
                    star.Speed = (float)(0.5 + rand.NextDouble() * 1.5);
                }
            }
        }

        private class Star
        {
            public float X;
            public float Y;
            public float Size;
            public float Speed;

            public Star(Random rand)
            {
                X = rand.Next(SidebarSettings.SidebarWidth);
                Y = rand.Next(SidebarSettings.SidebarHeight);
                Size = rand.Next(1, 3);
                Speed = (float)(0.5 + rand.NextDouble() * 1.5);
            }
        }
    }
}
