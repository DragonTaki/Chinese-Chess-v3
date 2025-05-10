/* ----- ----- ----- ----- */
// BackgroundRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace StarAnimation.Renderers
{
    /// <summary>
    /// Renders the static background layer. Default is a solid black fill.
    /// </summary>
    public class BackgroundRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Background brush used for rendering. Default is solid black.
        /// </summary>
        public Brush BackgroundBrush { get; set; } = Brushes.Black;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundRenderer"/> class.
        /// </summary>
        /// <param name="width">Initial width of the canvas.</param>
        /// <param name="height">Initial height of the canvas.</param>
        public BackgroundRenderer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Draws the background onto the specified Graphics surface.
        /// </summary>
        /// <param name="g">The Graphics object used for rendering.</param>
        public void Draw(Graphics g)
        {
            g.FillRectangle(BackgroundBrush, 0, 0, Width, Height);
        }

        /// <summary>
        /// Updates the background size when the render area is resized.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
