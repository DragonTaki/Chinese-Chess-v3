/* ----- ----- ----- ----- */
// LayoutF.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using SharedLib.MathUtils;

namespace SharedLib.Geometry
{
    /// <summary>
    /// Represents a layout rectangle with floating-point position and size.
    /// </summary>
    public class LayoutF
    {
        public Vector2F Position { get; set; }
        public Vector2F Size { get; set; }

        public LayoutF(Vector2F position, Vector2F size)
        {
            Position = position;
            Size = size;
        }

        public LayoutF(float x, float y, float width, float height)
            : this(new Vector2F(x, y), new Vector2F(width, height)) {}
            
        public LayoutF(float x, float y, Vector2F size)
            : this(new Vector2F(x, y), size) { }

        public LayoutF(Vector2F position, float width, float height)
            : this(position, new Vector2F(width, height)) {}

        public LayoutF(PointF position, Vector2F size)
            : this((Vector2F)position, size) {}

        public LayoutF(Vector2F position, PointF size)
            : this(position, (Vector2F)size) {}

        public LayoutF(PointF position, PointF size)
            : this((Vector2F)position, (Vector2F)size) {}

        public LayoutF(SizeF position, Vector2F size)
            : this((Vector2F)position, size) {}

        public LayoutF(Vector2F position, SizeF size)
            : this(position, (Vector2F)size) {}

        public LayoutF(SizeF position, PointF size)
            : this((Vector2F)position, (Vector2F)size) {}

        public LayoutF(PointF position, SizeF size)
            : this((Vector2F)position, (Vector2F)size) {}

        public LayoutF(SizeF position, SizeF size)
            : this((Vector2F)position, (Vector2F)size) {}

        // Optionally, helper properties or methods
        public Vector2F Center => new Vector2F(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f);
        public bool Contains(Vector2F point) =>
            point.X >= Position.X && point.X <= Position.X + Size.X &&
            point.Y >= Position.Y && point.Y <= Position.Y + Size.Y;
    }
}