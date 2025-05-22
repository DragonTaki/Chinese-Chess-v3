/* ----- ----- ----- ----- */
// SolidBrushFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.Geometry;

namespace Chinese_Chess_v3.Utils.StyleUtils
{
    public class SolidBrushFactory : IBrushFactory
    {
        public Color Color { get; set; }

        public SolidBrushFactory(Color color)
        {
            Color = color;
        }

        public Brush Create(LayoutF bounds) => new SolidBrush(Color);
    }
}
