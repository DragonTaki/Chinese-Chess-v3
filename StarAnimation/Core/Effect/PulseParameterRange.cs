/* ----- ----- ----- ----- */
// PulseParameterRange.cs
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
    /// Configurable parameter range for random PulseInstance generation.
    /// </summary>
    public class PulseParameterRange
    {
        public float TriggerChance { get; set; } = 0.9f;
        public float EffectAppliedChance { get; set; } = 0.9f;
        public FloatRange DurationRange { get; set; } = new FloatRange(5.0f, 10.0f);
        public FloatRange AmplitudeRange { get; set; } = new FloatRange(0.5f, 0.5f);
        public FloatRange MidOpacityRange { get; set; } = new FloatRange(0.5f, 0.5f);

        public PulseInstance CreateRandomInstance(PointF center, IAreaShape area, Random rand)
        {
            float duration = DurationRange.GetRandom(rand);
            float amplitude = AmplitudeRange.GetRandom(rand);
            float midOpacity = MidOpacityRange.GetRandom(rand);

            return new PulseInstance(center, area, duration, rand, amplitude, midOpacity, EffectAppliedChance);
        }
    }
}