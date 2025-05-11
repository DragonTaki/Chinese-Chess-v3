/* ----- ----- ----- ----- */
// PulseParameter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using StarAnimation.Utils;
using StarAnimation.Utils.Area;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Declare configurable parameter for Pulse effect instance.
    /// </summary>
    public class PulseParameter
    {
        public FloatRange CountdownRange { get; set; }
        public float TriggerChance { get; set; }
        public float EffectAppliedChance { get; set; }
        public FloatRange DurationRange { get; set; }
        public FloatRange AmplitudeRange { get; set; }
        public FloatRange MidOpacityRange { get; set; }

        public PulseInstance CreateRandomInstance(Vector2F center, IAreaShape area)
        {
            float duration = DurationRange.GetRandom();
            float amplitude = AmplitudeRange.GetRandom();
            float midOpacity = MidOpacityRange.GetRandom();

            return new PulseInstance(center, area, duration, EffectAppliedChance, amplitude, midOpacity);
        }
    }
}