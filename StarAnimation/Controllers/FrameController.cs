/* ----- ----- ----- ----- */
// FrameController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Models;
using StarAnimation.Renderers;
using StarAnimation.Utils.Area;

namespace StarAnimation.Controllers
{
    /// <summary>
    /// Controls temporary visual frames for debugging effect areas.
    /// Each frame appears for a specified duration before fading out.
    /// </summary>
    public class FrameController
    {
        private readonly int _width;
        private readonly int _height;
        private readonly FrameRenderer _renderer;

        private readonly List<Frame> _activeFrames = new();

        #region Settings (Adjustable Parameters)

        /// <summary>
        /// Duration in seconds before a frame disappears.
        /// </summary>
        public float LifetimeSeconds { get; set; } = 3.0f;

        /// <summary>
        /// Default frame border thickness.
        /// </summary>
        public float DefaultThickness { get; set; } = 4.0f;

        /// <summary>
        /// Default frame color.
        /// </summary>
        public Color DefaultColor { get; set; } = Color.Red;

        #endregion

        public FrameController(int width, int height)
        {
            _width = width;
            _height = height;

            _renderer = new FrameRenderer(_width, _height);
        }

        /// <summary>
        /// Adds a new debug frame to be drawn using a bounding rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to highlight with a frame.</param>
        /// <param name="color">Optional color override (default: DefaultColor).</param>
        /// <param name="thickness">Optional border thickness (default: DefaultThickness).</param>
        public void AddFrame(RectangleF rect, Color? color = null, int? thickness = null)
        {
            _activeFrames.Add(new Frame
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
            _activeFrames.RemoveAll(f => f.ExpirationTime < now);
        }

        /// <summary>
        /// Draws all currently active debug frames.
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(Graphics g)
        {
            _renderer.Draw(g, _activeFrames);
        }
    }
}