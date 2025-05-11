/* ----- ----- ----- ----- */
// ConcentrateInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using StarAnimation.Utils.Area;

using SharedLib.MathUtils;

namespace StarAnimation.Core.Effect
{
    public class ConcentrateInstance : EffectInstance
    {
        private List<Star> affectedStars = new();
        public ConcentrateInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance)
            : base(center, area, duration, effectAppliedChance) { }

        protected override void OnApplyTo(List<Star> stars)
        {

        }
        protected override void OnUpdate(float t)
        {
            foreach (var star in affectedStars)
            {
                star.Position.Current.X = MathUtil.Lerp(star.Position.Current.X, Center.X, t);
                star.Position.Current.Y = MathUtil.Lerp(star.Position.Current.Y, Center.Y, t);
            }
        }
        protected override void Reset()
        {
            affectedStars.Clear();
        }
    }
}