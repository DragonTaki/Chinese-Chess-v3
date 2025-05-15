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
using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Interface.UI.Constants;
using Chinese_Chess_v3.Utils.GraphicsUtils;
using SharedLib.RandomTable;
using SharedLib.Timing;

namespace Chinese_Chess_v3.Renderers
{
    public class SidebarRenderer
    {
        private readonly List<Star> stars = new List<Star>();
        private readonly Timer animationTimer;
        private readonly int maxStars = 60;
        private readonly IRandomProvider Rand = GlobalRandom.Instance;
        public SidebarRenderer()
        {
            for (int i = 0; i < maxStars; i++)
                stars.Add(new Star(Rand));

            animationTimer = new Timer { Interval = TimerSettings.GameAnimationInterval };
            animationTimer.Tick += (s, e) => UpdateStars();
            animationTimer.Start();
        }

        public void DrawSidebar(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            float x = UILayoutConstants.Sidebar.Position.X;
            float y = UILayoutConstants.Sidebar.Position.Y;
            float w = UILayoutConstants.Sidebar.Size.X;
            float h = UILayoutConstants.Sidebar.Size.Y;

            // Fill black background
            g.FillRectangle(Brushes.Black, x, y, w, h);

            // Draw stars
            foreach (var star in stars)
            {
                float alpha = Math.Max(0.2f, Rand.NextFloat());
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
                if (star.Y > UILayoutConstants.Sidebar.Size.Y)
                {
                    star.X = Rand.NextFloat(UILayoutConstants.Sidebar.Size.X);
                    star.Y = 0;
                    star.Size = Rand.NextInt(1, 3);
                    star.Speed = (float)(0.5 + Rand.NextFloat() * 1.5);
                }
            }
        }

        private class Star
        {
            public float X;
            public float Y;
            public float Size;
            public float Speed;

            public Star(IRandomProvider rand)
            {
                X = rand.NextFloat(UILayoutConstants.Sidebar.Size.X);
                Y = rand.NextFloat(UILayoutConstants.Sidebar.Size.Y);
                Size = rand.NextInt(1, 3);
                Speed = 0.5f + rand.NextFloat() * 1.5f;
            }
        }
    }
}
