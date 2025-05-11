/* ----- ----- ----- ----- */
// ColorShiftInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Utils;
using StarAnimation.Utils.Area;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Applies a sinusoidal red-blue color shift effect to a list of stars.
    /// Each star shifts color independently using a randomized phase.
    /// </summary>
    public class ColorShiftInstance : EffectInstance
    {
        private List<Star> affectedStars;

        /// <summary>
        /// Constructs the ColorShift effect with optional parameters.
        /// </summary>
        public ColorShiftInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance)
            : base(center, area, duration, effectAppliedChance)
        {
            Center = center;
            Area = area;
            Duration = duration;
            EffectAppliedChance = effectAppliedChance;
            affectedStars = new List<Star>();
        }

        public static ColorShiftInstance CreateRandom(IAreaShape area, ColorShiftParameter config)
        {
            RectangleF bounds = area.BoundingBox;
            Vector2F center;
            int maxTries = 100;

            do
            {
                float x = MathUtil.GetRandomFloat(bounds.Left, bounds.Right);
                float y = MathUtil.GetRandomFloat(bounds.Top, bounds.Bottom);
                center = new Vector2F(x, y);
            } while (!area.Contains(center) && --maxTries > 0);

            // Set effect center to area center if tries all failed
            if (maxTries <= 0)
                center = new Vector2F(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            float duration = config.DurationRange.GetRandom();

            return new ColorShiftInstance(center, area, duration, config.EffectAppliedChance);
        }

        /// <summary>
        /// Initializes color shift effect (applies to stars based on probability).
        /// </summary>
        protected override void OnApplyTo(List<Star> stars)
        {
            float currentTime = Environment.TickCount;

            foreach (var star in stars)
            {
                bool inArea = Area.Contains(star.Position);

                if (!inArea)
                {
                    star.ColorShift.HasPhase = false;
                    star.Color.Current = Color.White;
                    continue;
                }

                // Start transition
                if (!star.ColorShift.HasPhase)
                {
                    if (Rand.NextFloat() < EffectAppliedChance)
                    {
                        star.ColorShift.HasPhase = true;
                        star.ColorShift.StartTime = currentTime;
                        star.ColorShift.BiasDirection = Rand.NextFloat() < 0.5 ? -1f : 1f;
                        star.Color.Base = star.ColorShift.BiasDirection < 0
                            ? Color.FromArgb(255, 0, 0) // Red
                            : Color.FromArgb(0, 0, 255); // Blue
                    }
                    else
                    {
                        star.Color.Current = Color.White;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the color shift effect, transitioning each star's color smoothly.
        /// Should be called every frame.
        /// </summary>
        /// <param name="normalizedTime">
        /// A float value between 0 and 1 representing the progression of the effect's lifecycle.
        /// </summary>
        protected override void OnUpdate(float normalizedTime)
        {
            float currentTime = Environment.TickCount;
            float transitionMs = Duration * 1000f;

            foreach (var star in affectedStars)
            {
                if (!star.ColorShift.HasPhase) continue;

                float elapsed = currentTime - star.ColorShift.StartTime;
                if (elapsed >= transitionMs)
                {
                    star.ColorShift.HasPhase = false;
                    star.Color.Current = Color.White;
                    continue;
                }

                float normalized = elapsed / transitionMs;
                float wave = (float)Math.Sin(normalized * Math.PI);  // smooth wave: 0 → 1 → 0

                Color targetColor = MathUtil.LerpColor(Color.White, star.Color.Base, wave);
                star.Color.Current = targetColor;
            }
        }
        protected override void Reset()
        {
            affectedStars.Clear();
        }
    }
}
