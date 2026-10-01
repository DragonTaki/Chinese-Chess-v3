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

using Engine.Platform;
using StarAnimation.Models;

namespace StarAnimation.Renderers
{
    /// <summary>
    /// Renders temporary visual frames for debugging effect areas.
    /// Frame lifetime is managed by FrameController; this class only draws the outlines.
    /// </summary>
    public class FrameRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = Math.Max(value, 1);
            }
        }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                _height = Math.Max(value, 1);
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
        /// <param name="activeFrames">The frames to draw as rectangle outlines.</param>
        public void Draw(IGraphics g, List<Frame> activeFrames)
        {
            foreach (var frame in activeFrames)
            {
                using IPen pen = GraphicsBackend.Factory.CreatePen(frame.Color, frame.Thickness);
                g.DrawRectangle(pen, frame.Rect);
            }
        }
    }
}
