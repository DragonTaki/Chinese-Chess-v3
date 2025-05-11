/* ----- ----- ----- ----- */
// IAreaSelector.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace StarAnimation.Utils.Area
{
    /// <summary>
    /// Defines a selector that returns a random area shape within canvas bounds.
    /// </summary>
    public interface IAreaSelector
    {
        IAreaShape GetArea(float canvasWidth, float canvasHeight);
    }
}