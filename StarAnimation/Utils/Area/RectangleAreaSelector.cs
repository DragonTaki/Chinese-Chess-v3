/* ----- ----- ----- ----- */
// RectangleAreaSelector.cs
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
    public class RectangleAreaSelector : IAreaSelector
    {
        private readonly float minWidth, minHeight;
        private float maxWidth, maxHeight;
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        public RectangleAreaSelector(float minWidth, float minHeight, float maxWidth, float maxHeight)
        {
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }

        public IAreaShape GetArea(float canvasWidth, float canvasHeight)
        {
            if (canvasWidth <= 0 || canvasHeight <= 0)
                throw new ArgumentException("Canvas size must be positive.");
            if (minWidth <= 0 || minHeight <= 0 || maxWidth <= 0 || maxHeight <= 0)
                throw new ArgumentException("Size must be positive.");
                
            float width, height;
            float x, y;

            // Handle width
            if (minWidth >= canvasWidth)
            {
                width = canvasWidth;
                x = 0;
            }
            else
            {
                float limitedMaxWidth = Math.Min(maxWidth, canvasWidth);
                float effectiveMinWidth = Math.Min(minWidth, limitedMaxWidth);
                width = (limitedMaxWidth == effectiveMinWidth)
                    ? limitedMaxWidth
                    : Rand.NextFloat(effectiveMinWidth, limitedMaxWidth);
                x = Rand.NextFloat(0, canvasWidth - width);
            }

            // Handle height
            if (minHeight >= canvasHeight)
            {
                height = canvasHeight;
                y = 0;
            }
            else
            {
                float limitedMaxHeight = Math.Min(maxHeight, canvasHeight);
                float effectiveMinHeight = Math.Min(minHeight, limitedMaxHeight);
                height = (limitedMaxHeight == effectiveMinHeight)
                    ? limitedMaxHeight
                    : Rand.NextFloat(effectiveMinHeight, limitedMaxHeight);
                y = Rand.NextFloat(0, canvasHeight - height);
            }

            return new RectangleAreaShape(new RectangleF(x, y, width, height));
        }
    }
}
