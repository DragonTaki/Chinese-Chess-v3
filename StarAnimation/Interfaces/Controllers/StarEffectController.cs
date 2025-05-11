/* ----- ----- ----- ----- */
// StarEffectController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Core;
using StarAnimation.Core.Effect;
using StarAnimation.Renderers;
using StarAnimation.Utils;
using StarAnimation.Utils.Area;

using SharedLib.RandomTable;
using SharedLib.Timing;

namespace StarAnimation.Controllers
{
    /// <summary>
    /// Controls dynamic starfield effects and visual debugging.
    /// Manages multiple concurrently active visual effects.
    /// </summary>
    public class StarEffectController
    {
        private readonly int canvasWidth, canvasHeight;
        private readonly FrameRenderer frameRenderer;

        private readonly List<IEffectInstance> activeEffects = new();
        private readonly Dictionary<EffectType, object> effectConfigs = new(EffectConfigRegistry.Configs);
        private readonly List<EffectEntry> effectEntries = new();
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        private bool EnableDebugFrame { get; set; } = true;

        /// <summary>
        /// Control which effects will be enabled.
        /// </summary>
        private readonly Dictionary<EffectType, bool> enableEffect = new()
        {
            [EffectType.ColorShift] = false,
            [EffectType.Pulse] = false,
            [EffectType.Twist] = true
        };

        /// <summary>
        /// Internal structure to manage each effect's creation, countdown, and metadata.
        /// </summary>
        private class EffectEntry
        {
            public EffectType Name;
            public IAreaSelector AreaSelector;
            public float Countdown;
            public Func<IAreaShape, object, IEffectInstance> CreateInstance;
            public Color DebugColor;
        }

        public StarEffectController(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
            frameRenderer = new FrameRenderer();

            // Register effect entries
            foreach (EffectType type in Enum.GetValues(typeof(EffectType)))
            {
                if (!enableEffect.TryGetValue(type, out var enabled) || !enabled)
                    continue;
                effectEntries.Add(new EffectEntry
                {
                    Name = type,
                    AreaSelector = type switch
                    {
                        EffectType.ColorShift => new RectangleAreaSelector(1920f, 1080f, 1920f, 1080f),
                        EffectType.Pulse => new RectangleAreaSelector(1920f, 1080f, 1920f, 1080f),
                        EffectType.Twist => new CircleAreaSelector(1920f, 1920f),
                        _ => throw new NotSupportedException($"No area selector for effect type: {type}")
                    },
                    
                    Countdown = 0.0f,
                    DebugColor = EffectConfigRegistry.DebugColors[type],
                    CreateInstance = EffectConfigRegistry.Factories[type]
                });
            }
        }
        // For outside manual add effect
        public void AddEffect(EffectInstance effect, List<Star> stars)
        {
            effect.ApplyTo(stars);
            activeEffects.Add(effect);
        }
        private void AutoAddEffect(List<Star> stars)
        {
            // Attempt to trigger new effects
            foreach (var entry in effectEntries)
            {
                if (!effectConfigs.TryGetValue(entry.Name, out var config))
                    continue;

                entry.Countdown -= GlobalTime.Timer.DeltaTimeInSeconds;
                if (entry.Countdown > 0) continue;

                (float triggerChance, float countdown) = config switch
                {
                    ColorShiftParameter shift => (shift.TriggerChance, Rand.NextFloat(shift.CountdownRange.Min, shift.CountdownRange.Max)),
                    PulseParameter pulse => (pulse.TriggerChance, Rand.NextFloat(pulse.CountdownRange.Min, pulse.CountdownRange.Max)),
                    TwistParameter twist => (twist.TriggerChance, Rand.NextFloat(twist.CountdownRange.Min, twist.CountdownRange.Max)),
                    _ => (1.0f, 10.0f)  // Default values for unknown effect types
                };

                if (Rand.NextDouble() < triggerChance)
                {
                    var area = entry.AreaSelector.GetArea(canvasWidth, canvasHeight);
                    var instance = entry.CreateInstance(area, config);

                    instance.ApplyTo(stars);
                    activeEffects.Add(instance);

                    if (EnableDebugFrame)
                        frameRenderer.ShowFrame(area, entry.DebugColor, 4);
                }

                entry.Countdown = countdown;
            }
        }
        public void Update(List<Star> stars, Graphics g)
        {
            // Update all active effects
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.Update();

                if (!effect.IsActive)
                    activeEffects.RemoveAt(i);
            }

            AutoAddEffect(stars);

            if (EnableDebugFrame)
                frameRenderer.Update();
        }

        public void Draw(Graphics g)
        {
            if (EnableDebugFrame)
                frameRenderer.Draw(g);
        }
    }
}
