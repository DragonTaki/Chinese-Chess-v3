/* ----- ----- ----- ----- */
// StarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/14
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Models;

namespace StarAnimation.Renderers
{
    public class StarRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        private int width;
        public int Width
        {
            get => width;
            set
            {
                width = Math.Max(value, 1);
            }
        }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        private int height;
        public int Height
        {
            get => height;
            set
            {
                height = Math.Max(value, 1);
            }
        }

        public StarRenderer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Clear canvas and render all visible stars.
        /// </summary>
        public void Draw(Graphics g, List<Star> stars)
        {
            foreach (var star in stars)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(star.Opacity * 255), star.Color.Current)))
                {
                    g.FillEllipse(brush, star.Position.Current.X, star.Position.Current.Y, star.Size, star.Size);
                }
            }
        }
    }
}
