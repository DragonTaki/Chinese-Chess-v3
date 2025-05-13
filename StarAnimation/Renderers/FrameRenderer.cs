/* ----- ----- ----- ----- */
// FrameRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/09
// Version: v1.1 (Added Area Support and Update method)
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Models;

namespace StarAnimation.Renderers
{
    /// <summary>
    /// Renders temporary visual frames for debugging effect areas.
    /// Each frame appears for a specified duration before fading out.
    /// </summary>
    public class FrameRenderer
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

        public FrameRenderer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Draws all currently active debug frames.
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(Graphics g, List<Frame> activeFrames)
        {
            foreach (var frame in activeFrames)
            {
                using Pen pen = new Pen(frame.Color, frame.Thickness);
                g.DrawRectangle(pen, frame.Rect);
            }
        }
    }
}
