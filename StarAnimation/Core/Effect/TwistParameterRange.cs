/* ----- ----- ----- ----- */
// TwistParameterRange.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using StarAnimation.Utils;
using StarAnimation.Utils.Area;
using static StarAnimation.Utils.MathUtil;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Configurable parameter range for random TwistInstance generation.
    /// </summary>
    public class TwistParameterRange
    {
        public float TriggerChance { get; set; } = 0.9f;
        public float EffectAppliedChance { get; set; } = 0.9f;
        public FloatRange DurationRange { get; set; } = new FloatRange(300f, 800f);
        public FloatRange RadiusRange { get; set; } = new FloatRange(30f, 80f);
        public FloatRange StrengthRange { get; set; } = new FloatRange(0.5f, 1.5f);
        public float ClockwiseChance { get; set; } = 0.5f;

        public TwistInstance CreateRandomInstance(PointF center, IAreaShape area, Random rand)
        {
            float duration = DurationRange.GetRandom(rand);
            float radius = RadiusRange.GetRandom(rand);
            float strength = StrengthRange.GetRandom(rand);
            float direction = rand.NextDouble() < ClockwiseChance ? 1f : -1f;

            return new TwistInstance(center, area, duration, rand, strength, radius, direction, EffectAppliedChance);
        }
    }
}