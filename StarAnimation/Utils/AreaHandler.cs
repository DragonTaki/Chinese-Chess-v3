/* ----- ----- ----- ----- */
// AreaHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarAnimation.Utils
{
    /// <summary>
    /// Handles region-related logic for the starfield renderer, such as exclusion zones
    /// (e.g., UI-covered areas where stars should not be rendered).
    /// </summary>
    public class AreaHandler
    {
        /// <summary>
        /// The list of excluded screen-space rectangular areas.
        /// Stars within these areas will be skipped or hidden.
        /// </summary>
        private List<Rectangle> excludedAreas = new();

        /// <summary>
        /// Gets or sets the list of excluded rectangular areas.
        /// </summary>
        public List<Rectangle> ExcludedAreas
        {
            get => excludedAreas;
            set => excludedAreas = value ?? new List<Rectangle>();
        }

        /// <summary>
        /// Determines if a given point is located inside any excluded area.
        /// </summary>
        /// <param name="x">The x-coordinate to check (float).</param>
        /// <param name="y">The y-coordinate to check (float).</param>
        /// <returns>True if the point is within an excluded region; otherwise false.</returns>
        public bool IsInExcludedArea(float x, float y)
        {
            foreach (var area in excludedAreas)
            {
                if (area.Contains((int)x, (int)y))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a given point is inside any excluded area.
        /// </summary>
        /// <param name="point">A point to check.</param>
        /// <returns>True if the point is excluded; otherwise false.</returns>
        public bool IsInExcludedArea(PointF point)
        {
            return IsInExcludedArea(point.X, point.Y);
        }

        /// <summary>
        /// Clears all defined exclusion areas.
        /// </summary>
        public void ClearExcludedAreas()
        {
            excludedAreas.Clear();
        }
    }
}
