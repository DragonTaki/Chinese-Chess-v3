/* ----- ----- ----- ----- */
// InwardCornerDialogStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Utils.GraphicsUtils.GraphicsPaths;
using Chinese_Chess_v3.Utils.StyleUtils;

using SharedLib.Geometry;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Configs.Style
{
    /// <summary>
    /// Dialog style with a single border and inward-rounded corners.
    /// </summary>
    public class InwardCornerDialogStyle : IBoxDrawStyle
    {
        public float CornerRadius { get; set; }
        public BorderStyle Border { get; set; }
        public IBrushFactory BackgroundBrushFactory { get; set; }

        /// <summary>
        /// Draw a dialog box with inward-rounded corners.
        /// </summary>
        public void Draw(Graphics g, LayoutF bounds)
        {
            using var path = InvertedRoundedRectPath.Create(bounds.Size.X, bounds.Size.Y, CornerRadius);
            using var matrix = new Matrix();
            matrix.Translate(bounds.Position.X, bounds.Position.Y);
            path.Transform(matrix);

            using var brush = BackgroundBrushFactory.Create(bounds);
            g.FillPath(brush, path);

            using var pen = new Pen(Border.Color, Border.Width);
            g.DrawPath(pen, path);
        }

        /// <summary>
        /// Draw a dialog box at a given position and size.
        /// </summary>
        public void Draw(Graphics g, Vector2F position, Vector2F size)
            => Draw(g, new LayoutF(position, size));
    }
}
