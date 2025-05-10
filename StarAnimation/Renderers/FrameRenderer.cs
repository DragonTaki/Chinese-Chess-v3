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
using StarAnimation.Utils.Area;

namespace StarAnimation.Renderers
{
    /// <summary>
    /// Renders temporary visual frames for debugging effect areas.
    /// Each frame appears for a specified duration before fading out.
    /// </summary>
    public class FrameRenderer
    {
        /// <summary>
        /// Represents a single debug frame with bounds, color, thickness, and expiration time.
        /// </summary>
        private class Frame
        {
            public RectangleF Rect { get; set; }
            public Color Color { get; set; }
            public int Thickness { get; set; }
            public DateTime ExpirationTime { get; set; }
        }

        private readonly List<Frame> activeFrames = new();

        #region Settings (Adjustable Parameters)

        /// <summary>
        /// Duration in seconds before a frame disappears.
        /// </summary>
        public double LifetimeSeconds { get; set; } = 3.0;

        /// <summary>
        /// Default frame border thickness.
        /// </summary>
        public int DefaultThickness { get; set; } = 4;

        /// <summary>
        /// Default frame color.
        /// </summary>
        public Color DefaultColor { get; set; } = Color.Red;

        #endregion

        /// <summary>
        /// Adds a new debug frame to be drawn using a bounding rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to highlight with a frame.</param>
        /// <param name="color">Optional color override (default: DefaultColor).</param>
        /// <param name="thickness">Optional border thickness (default: DefaultThickness).</param>
        public void AddFrame(RectangleF rect, Color? color = null, int? thickness = null)
        {
            activeFrames.Add(new Frame
            {
                Rect = rect,
                Color = color ?? DefaultColor,
                Thickness = thickness ?? DefaultThickness,
                ExpirationTime = DateTime.Now.AddSeconds(LifetimeSeconds)
            });
        }

        /// <summary>
        /// Adds a debug frame based on a shape's bounding box.
        /// </summary>
        /// <param name="area">The area shape to visualize.</param>
        /// <param name="color">Color of the frame border.</param>
        /// <param name="thickness">Thickness of the frame border.</param>
        public void ShowFrame(IAreaShape area, Color color, int thickness)
        {
            if (area == null) return;

            RectangleF bounds = area.BoundingBox;
            AddFrame(bounds, color, thickness);
        }

        /// <summary>
        /// Removes expired frames from the active frame list.
        /// </summary>
        public void Update()
        {
            DateTime now = DateTime.Now;
            activeFrames.RemoveAll(f => f.ExpirationTime < now);
        }

        /// <summary>
        /// Draws all currently active debug frames.
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(Graphics g)
        {
            foreach (var frame in activeFrames)
            {
                using Pen pen = new Pen(frame.Color, frame.Thickness);
                g.DrawRectangle(pen, frame.Rect);
            }
        }
    }
}
