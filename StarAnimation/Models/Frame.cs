/* ----- ----- ----- ----- */
// Frame.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace StarAnimation.Models
{
    /// <summary>
    /// Represents a single debug frame with bounds, color, thickness, and expiration time.
    /// </summary>
    public class Frame
    {
        public RectangleF Rect { get; set; }
        public Color Color { get; set; }
        public float Thickness { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}