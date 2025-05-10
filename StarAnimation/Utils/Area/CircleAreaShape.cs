/* ----- ----- ----- ----- */
// CircleAreaShape.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace StarAnimation.Utils.Area
{
    /// <summary>
    /// Represents a circular area defined by center and radius.
    /// </summary>
    public class CircleAreaShape : IAreaShape
    {
        /// <summary>
        /// Center of the circle.
        /// </summary>
        public PointF Center { get; }

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius { get; }

        /// <summary>
        /// Bounding box that fully contains this circular area.
        /// </summary>
        public RectangleF BoundingBox { get; }

        public CircleAreaShape(PointF center, float radius)
        {
            Center = center;
            Radius = radius;

            // Calculate bounding rectangle based on center and radius
            BoundingBox = new RectangleF(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2
            );
        }

        /// <summary>
        /// Checks whether a given point is within the circular area.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <returns>True if point is within the circle, false otherwise.</returns>
        public bool Contains(PointF p)
        {
            float dx = p.X - Center.X;
            float dy = p.Y - Center.Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }
    }
}
