/* ----- ----- ----- ----- */
// TwistParameter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using StarAnimation.Utils.Area;

using StarAnimation.Core.Effect.Instance;

using SharedLib.MathUtils;
using SharedLib.RandomTable;

namespace StarAnimation.Core.Effect.Parameter
{
    /// <summary>
    /// Declare configurable parameter for Twist effect instance.
    /// </summary>
    public class TwistParameter
    {
        public RangeF CountdownRange { get; set; }
        public float TriggerChance { get; set; }
        public float EffectAppliedChance { get; set; }
        public RangeF DurationRange { get; set; }
        public RangeF RadiusRange { get; set; }
        public RangeF StrengthRange { get; set; }
        public float ClockwiseChance { get; set; }
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        public TwistInstance CreateRandomInstance(Vector2F center, IAreaShape area)
        {
            float duration = DurationRange.GetRandom();
            float radius = RadiusRange.GetRandom();
            float strength = StrengthRange.GetRandom();
            float direction = Rand.NextDouble() < ClockwiseChance ? 1f : -1f;

            return new TwistInstance(center, area, duration, EffectAppliedChance, strength, radius, direction);
        }
    }
}