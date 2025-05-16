/* ----- ----- ----- ----- */
// EffectConfigRegistry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Core.Effect;
using StarAnimation.Core.Effect.Instance;
using StarAnimation.Core.Effect.Parameter;
using StarAnimation.Models;
using StarAnimation.Utils.Area;

using SharedLib.MathUtils;

namespace StarAnimation.Core
{
    /// <summary>
    /// Central registry for effect parameter presets and factory methods.
    /// </summary>
    public static class EffectConfigRegistry
    {
        /// <summary>
        /// Maps each EffectType to its configuration object.
        /// </summary>
        public static readonly Dictionary<EffectType, object> Configs = new()
        {
            [EffectType.ColorShift] = new ColorShiftParameter
            {
                CountdownRange = new RangeF(3.0f, 5.0f),
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.1f,
                DurationRange = new RangeF(3f, 6f),
                //HueShiftRange = new RangeF(-45f, 45f)
            },
            [EffectType.Pulse] = new PulseParameter
            {
                CountdownRange = new RangeF(3.0f, 5.0f),
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.3f,
                DurationRange = new RangeF(3.0f, 5.0f),
                AmplitudeRange = new RangeF(0.0f, 1.0f),
                MidOpacityRange = new RangeF(0.0f, 1.0f)
            },
            [EffectType.Twist] = new TwistParameter
            {
                CountdownRange = new RangeF(15.0f, 15.0f),
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.6f,
                DurationRange = new RangeF(10.0f, 10.0f),
                RadiusRange = new RangeF(200.0f, 400.0f),
                StrengthRange = new RangeF(0.5f, 1.5f),
                ClockwiseChance = 0.45f
            }
        };

        /// <summary>
        /// Maps each EffectType to its factory method for creating instances.
        /// </summary>
        public static readonly Dictionary<EffectType, Func<IAreaShape, object, IEffectInstance>> Factories = new()
        {
            [EffectType.Pulse] = (area, config) => PulseInstance.CreateRandom(area, (PulseParameter)config),
            [EffectType.Twist] = (area, config) => TwistInstance.CreateRandom(area, (TwistParameter)config),
            [EffectType.ColorShift] = (area, config) => ColorShiftInstance.CreateRandom(area, (ColorShiftParameter)config)
        };

        /// <summary>
        /// Optionally assign each effect type a debug color.
        /// </summary>
        public static readonly Dictionary<EffectType, Color> DebugColors = new()
        {
            [EffectType.Pulse] = Color.Magenta,
            [EffectType.Twist] = Color.Cyan,
            [EffectType.ColorShift] = Color.Orange
        };
    }
}
