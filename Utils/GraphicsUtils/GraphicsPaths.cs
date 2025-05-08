/* ----- ----- ----- ----- */
// GraphicsPaths.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chinese_Chess_v3.Utils.GraphicsUtils
{
    public static class GraphicsPaths
    {
        /// <summary>
        /// Create a basic shield-shaped path.
        /// </summary>
        public static GraphicsPath CreateShieldPath(int width, int height)
        {
            GraphicsPath path = new GraphicsPath();

            int curveHeight = 20;
            int bottomPointHeight = 40;

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
        public static GraphicsPath ApplyVerticalSkew(GraphicsPath originalPath, float topScale, float bottomScale, int height)
        {
            // Create a custom skew matrix
            GraphicsPath transformed = (GraphicsPath)originalPath.Clone();

            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(0, -height / 2f); // Center to origin
                matrix.Shear((bottomScale - topScale) / height, 0); // Shear horizontally based on difference
                matrix.Translate(0, height / 2f); // Translate back
                transformed.Transform(matrix);
            }

            return transformed;
        }
    }
}