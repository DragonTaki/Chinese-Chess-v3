/* ----- ----- ----- ----- */
// RectangleAreaShape.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.MathUtils;

namespace StarAnimation.Utils.Area
{
    public class RectangleAreaShape : IAreaShape
    {
        public RectangleF BoundingBox { get; }

        public RectangleAreaShape(RectangleF rect)
        {
            BoundingBox = rect;
        }

        public bool Contains(Vector2F p)
        {
            return BoundingBox.Contains(p.ToPointF);
        }
    }
}
