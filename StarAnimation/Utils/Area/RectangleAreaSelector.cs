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

using Engine.Randomization;

namespace StarAnimation.Utils.Area
{
    public class RectangleAreaSelector : IAreaSelector
    {
        private readonly float _minWidth, _minHeight;
        private readonly float _maxWidth, _maxHeight;
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        public RectangleAreaSelector(float minWidth, float minHeight, float maxWidth, float maxHeight)
        {
            _minWidth = minWidth;
            _minHeight = minHeight;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }

        public IAreaShape GetArea(float canvasWidth, float canvasHeight)
        {
            if (canvasWidth <= 0 || canvasHeight <= 0)
                throw new ArgumentException("Canvas size must be positive.");
            if (_minWidth <= 0 || _minHeight <= 0 || _maxWidth <= 0 || _maxHeight <= 0)
                throw new ArgumentException("Size must be positive.");
                
            float width, height;
            float x, y;

            // Handle width
            if (_minWidth >= canvasWidth)
            {
                width = canvasWidth;
                x = 0;
            }
            else
            {
                float limitedMaxWidth = Math.Min(_maxWidth, canvasWidth);
                float effectiveMinWidth = Math.Min(_minWidth, limitedMaxWidth);
                width = (limitedMaxWidth == effectiveMinWidth)
                    ? limitedMaxWidth
                    : Rand.NextFloat(effectiveMinWidth, limitedMaxWidth);
                x = Rand.NextFloat(0, canvasWidth - width);
            }

            // Handle height
            if (_minHeight >= canvasHeight)
            {
                height = canvasHeight;
                y = 0;
            }
            else
            {
                float limitedMaxHeight = Math.Min(_maxHeight, canvasHeight);
                float effectiveMinHeight = Math.Min(_minHeight, limitedMaxHeight);
                height = (limitedMaxHeight == effectiveMinHeight)
                    ? limitedMaxHeight
                    : Rand.NextFloat(effectiveMinHeight, limitedMaxHeight);
                y = Rand.NextFloat(0, canvasHeight - height);
            }

            return new RectangleAreaShape(new RectangleF(x, y, width, height));
        }
    }
}
