/* ----- ----- ----- ----- */
// ConcentrateInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Utils;
using StarAnimation.Utils.Area;

namespace StarAnimation.Core.Effect
{
    public class ConcentrateInstance : EffectInstance
    {
        private List<Star> affectedStars = new();
        public ConcentrateInstance(PointF center, IAreaShape area, float duration, Random rand)
            : base(center, area, duration, rand) { }

        protected override void OnApplyTo(List<Star> stars)
        {

        }
        protected override void OnUpdate(float t)
        {
            foreach (var star in affectedStars)
            {
                star.X = MathUtil.Lerp(star.X, Center.X, t);
                star.Y = MathUtil.Lerp(star.Y, Center.Y, t);
            }
        }
        protected override void Reset()
        {
            affectedStars.Clear();
        }
    }
}