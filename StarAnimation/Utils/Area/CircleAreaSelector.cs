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

namespace StarAnimation.Utils.Area
{
    public class CircleAreaSelector : IAreaSelector
    {
        private readonly float minRadius;
        private float maxRadius;

        public CircleAreaSelector(float minRadius, float maxRadius)
        {
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
        }

        public IAreaShape GetArea(float canvasWidth, float canvasHeight, Random rand)
        {
            if (canvasWidth <= 0 || canvasHeight <= 0)
                throw new ArgumentException("Canvas size must be positive.");
            if (minRadius <= 0 || maxRadius <= 0)
                throw new ArgumentException("Radius must be positive.");
            
            float canvasRadius = (float)Math.Sqrt(Math.Pow(canvasWidth, 2) + Math.Pow(canvasHeight, 2)) / 2;
            float radius;
            float cx, cy;

            // Handle radius
            if (minRadius >= canvasRadius)
            {
                radius = canvasRadius;
                cx = canvasWidth / 2;
                cy = canvasHeight / 2;
            }
            else
            {
                float limitedRadius = Math.Min(maxRadius, canvasRadius);
                float effectiveRadius = Math.Min(minRadius, limitedRadius);
                radius = (limitedRadius == effectiveRadius)
                    ? limitedRadius
                    : MathUtil.GetRandomFloat(effectiveRadius, limitedRadius, rand);
                cx = MathUtil.GetRandomFloat(radius, canvasRadius - radius, rand);
                cy = MathUtil.GetRandomFloat(radius, canvasRadius - radius, rand);
            }

            return new CircleAreaShape(new PointF(cx, cy), radius);
        }
    }
}
