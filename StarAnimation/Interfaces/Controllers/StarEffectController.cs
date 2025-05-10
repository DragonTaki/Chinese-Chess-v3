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

namespace StarAnimation.Controllers
{
    /// <summary>
    /// Controls dynamic starfield effects and visual debugging.
    /// Manages multiple concurrently active visual effects.
    /// </summary>
    public class StarEffectController
    {
        private readonly int canvasWidth, canvasHeight;
        private readonly Random rand;
        private readonly FrameRenderer frameRenderer;

        private readonly List<IEffectInstance> activeEffects = new();
        private readonly Dictionary<string, object> effectConfigs = new();
        private readonly List<EffectEntry> effectEntries = new();

        public bool EnableDebugFrame { get; set; } = true;

        /// <summary>
        /// Internal structure to manage each effect's creation, countdown, and metadata.
        /// </summary>
        private class EffectEntry
        {
            public string Name;
            public IAreaSelector AreaSelector;
            public int Countdown;
            public Func<IAreaShape, object, Random, IEffectInstance> CreateInstance;
            public Color DebugColor;
        }

        public StarEffectController(int width, int height, Random rand)
        {
            canvasWidth = width;
            canvasHeight = height;
            this.rand = rand;
            frameRenderer = new FrameRenderer();

            // Register configurable effect parameters
            effectConfigs["Twist"] = new TwistParameterRange
            {
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.9f,
                DurationRange = new FloatRange(3f, 5f),
                RadiusRange = new FloatRange(20f, 60f),
                StrengthRange = new FloatRange(0.6f, 1.2f),
                ClockwiseChance = 0.7f
            };

            effectConfigs["Pulse"] = new PulseParameterRange
            {
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.7f,
                DurationRange = new FloatRange(3f, 5f),
                AmplitudeRange = new FloatRange(0.0f, 1.0f),
                MidOpacityRange = new FloatRange(0.0f, 1.0f)
            };
/*
            effectConfigs["ColorShift"] = new ColorShiftParameterRange
            {
                TriggerChance = 0.9f,
                EffectAppliedChance = 0.5f,
                HueShiftRange = new FloatRange(0f, 180f),
                DurationRange = new FloatRange(300f, 800f)
            };
*/
            // Register effect entries
            effectEntries.Add(new EffectEntry
            {
                Name = "Twist",
                AreaSelector = new RectangleAreaSelector(1920f, 1080f, 1920f, 1080f),
                Countdown = rand.Next(100, 200),
                DebugColor = Color.Blue,
                CreateInstance = (area, config, r) => TwistInstance.CreateRandom(area, (TwistParameterRange)config, r)
            });

            effectEntries.Add(new EffectEntry
            {
                Name = "Pulse",
                AreaSelector = new CircleAreaSelector(1920f, 1920f),
                Countdown = rand.Next(100, 200),
                DebugColor = Color.Magenta,
                CreateInstance = (area, config, r) => PulseInstance.CreateRandom(area, (PulseParameterRange)config, r)
            });
/*
            effectEntries.Add(new EffectEntry
            {
                Name = "ColorShift",
                AreaSelector = new RectangleAreaSelector(300, 300, 600, 600),
                Countdown = rand.Next(100, 200),
                DebugColor = Color.Orange,
                CreateInstance = (area, config, r) => ColorShiftInstance.CreateRandom(area, (ColorShiftParameterRange)config, r)
            });*/
        }
        // For outside add effect
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
                entry.Countdown--;
                if (entry.Countdown > 0) continue;

                var config = effectConfigs[entry.Name];
                float triggerChance = config switch
                {
                    TwistParameterRange twist => twist.TriggerChance,
                    PulseParameterRange pulse => pulse.TriggerChance,
                    //ColorShiftParameterRange shift => shift.TriggerChance,
                    _ => 1f
                };

                if (rand.NextDouble() < triggerChance)
                {
                    var area = entry.AreaSelector.GetArea(canvasWidth, canvasHeight, rand);
                    var instance = entry.CreateInstance(area, config, rand);

                    instance.ApplyTo(stars);
                    activeEffects.Add(instance);

                    if (EnableDebugFrame)
                        frameRenderer.ShowFrame(area, entry.DebugColor, 4);
                }

                entry.Countdown = rand.Next(100, 200);
            }
        }
        public void Update(List<Star> stars, Graphics g, float deltaTimeInSeconds)
        {
            // Update all active effects
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.Update(deltaTimeInSeconds);

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
