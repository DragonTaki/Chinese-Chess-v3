/* ----- ----- ----- ----- */
// GraphicsPaths.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chinese_Chess_v3.Utils.GraphicsUtils
{
    public static class GraphicsPaths
    {
        /// <summary>
        /// Create a rounded rectangle path (like a sticky label with slightly rounded corners).
        /// </summary>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="cornerRadius">Optional: Radius of the corner (default: auto-calculated).</param>
        public static GraphicsPath CreateRoundedRectPath(float width, float height, float? cornerRadius = null)
        {
            GraphicsPath path = new GraphicsPath();

            float radius = cornerRadius ?? Math.Min(width, height) * 0.08f;  // 8% of size (or override)
            radius = MathF.Min(radius, MathF.Min(width, height) / 2f);       // Avoid over-rounding

            float diameter = radius * 2;

            RectangleF topLeft = new RectangleF(0, 0, diameter, diameter);
            RectangleF topRight = new RectangleF(width - diameter, 0, diameter, diameter);
            RectangleF bottomRight = new RectangleF(width - diameter, height - diameter, diameter, diameter);
            RectangleF bottomLeft = new RectangleF(0, height - diameter, diameter, diameter);

            path.StartFigure();
            path.AddArc(topLeft, 180, 90);                           // Top-left corner
            path.AddLine(radius, 0, width - radius, 0);              // Top edge
            path.AddArc(topRight, 270, 90);                          // Top-right corner
            path.AddLine(width, radius, width, height - radius);     // Right edge
            path.AddArc(bottomRight, 0, 90);                         // Bottom-right corner
            path.AddLine(width - radius, height, radius, height);    // Bottom edge
            path.AddArc(bottomLeft, 90, 90);                         // Bottom-left corner
            path.AddLine(0, height - radius, 0, radius);             // Left edge
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Create a basic shield-shaped path.
        /// </summary>
        public static GraphicsPath CreateShieldPath(float width, float height)
        {
            GraphicsPath path = new GraphicsPath();

            float curveHeight = 20.0f;
            float bottomPointHeight = 40.0f;

            path.StartFigure();
            path.AddArc(0, 0, curveHeight * 2, curveHeight * 2, 180, 90); // Top-left corner
            path.AddLine(curveHeight, 0, width - curveHeight, 0);         // Top edge
            path.AddArc(width - curveHeight * 2, 0, curveHeight * 2, curveHeight * 2, 270, 90); // Top-right corner
            path.AddLine(width, curveHeight, width, height - bottomPointHeight); // Right edge

            path.AddBezier(width, height - bottomPointHeight,
                           width * 0.75f, height,
                           width * 0.25f, height,
                           0, height - bottomPointHeight); // Bottom tip

            path.AddLine(0, height - bottomPointHeight, 0, curveHeight); // Left edge
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Apply vertical distortion (e.g., top-narrow, bottom-wide) to a given path.
        /// </summary>
        public static GraphicsPath ApplyVerticalSkew(GraphicsPath originalPath, float topScale, float bottomScale, float height)
        {
            // Create a custom skew matrix
            GraphicsPath transformed = (GraphicsPath)originalPath.Clone();

            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(0, -height / 2.0f); // Center to origin
                matrix.Shear((bottomScale - topScale) / height, 0); // Shear horizontally based on difference
                matrix.Translate(0, height / 2.0f); // Translate back
                transformed.Transform(matrix);
            }

            return transformed;
        }
    }
}