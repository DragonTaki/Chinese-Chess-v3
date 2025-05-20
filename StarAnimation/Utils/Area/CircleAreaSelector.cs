/* ----- ----- ----- ----- */
// CircleAreaSelector.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using SharedLib.RandomTable;

namespace StarAnimation.Utils.Area
{
    public class CircleAreaSelector : IAreaSelector
    {
        private readonly float _minRadius, _maxRadius;
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        public CircleAreaSelector(float minRadius, float maxRadius)
        {
            _minRadius = minRadius;
            _maxRadius = maxRadius;
        }

        public IAreaShape GetArea(float canvasWidth, float canvasHeight)
        {
            if (canvasWidth <= 0 || canvasHeight <= 0)
                throw new ArgumentException("Canvas size must be positive.");
            if (_minRadius <= 0 || _maxRadius <= 0)
                throw new ArgumentException("Radius must be positive.");
            
            float canvasRadius = (float)Math.Sqrt(Math.Pow(canvasWidth, 2) + Math.Pow(canvasHeight, 2)) / 2;
            float radius;
            float cx, cy;

            // Handle radius
            if (_minRadius >= canvasRadius)
            {
                radius = canvasRadius;
                cx = canvasWidth / 2;
                cy = canvasHeight / 2;
            }
            else
            {
                float limitedRadius = Math.Min(_maxRadius, canvasRadius);
                float effectiveRadius = Math.Min(_minRadius, limitedRadius);
                radius = (limitedRadius == effectiveRadius)
                    ? limitedRadius
                    : Rand.NextFloat(effectiveRadius, limitedRadius);
                cx = Rand.NextFloat(radius, canvasRadius - radius);
                cy = Rand.NextFloat(radius, canvasRadius - radius);
            }

            return new CircleAreaShape(new PointF(cx, cy), radius);
        }
    }
}
