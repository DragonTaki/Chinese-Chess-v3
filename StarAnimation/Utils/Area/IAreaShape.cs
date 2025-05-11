/* ----- ----- ----- ----- */
// IAreaShape.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using StarAnimation.Core;

namespace StarAnimation.Utils.Area
{
    /// <summary>
    /// Represents a geometric area used for effect application and hit testing.
    /// </summary>
    public interface IAreaShape
    {
        RectangleF BoundingBox { get; }

        /// <summary>
        /// Checks whether a given point is inside the area.
        /// </summary>
        bool Contains(Vector2F p);
    }
}
