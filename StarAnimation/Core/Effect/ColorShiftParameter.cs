/* ----- ----- ----- ----- */
// ColorShiftParameter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using StarAnimation.Utils.Area;

using StarAnimation.Core.Effect.Instance;

using SharedLib.MathUtils;

namespace StarAnimation.Core.Effect.Parameter
{
    /// <summary>
    /// Declare configurable parameter for ColorShift effect instance.
    /// </summary>
    public class ColorShiftParameter
    {
        public RangeF CountdownRange { get; set; }
        public float TriggerChance { get; set; }
        public float EffectAppliedChance { get; set; }
        public RangeF DurationRange { get; set; }

        public ColorShiftInstance CreateRandomInstance(Vector2F center, IAreaShape area)
        {
            float duration = DurationRange.GetRandom();

            return new ColorShiftInstance(center, area, duration, EffectAppliedChance);
        }
    }
}