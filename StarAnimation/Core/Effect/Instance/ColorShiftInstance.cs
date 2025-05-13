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

using StarAnimation.Core.Effect.Parameter;
using StarAnimation.Models;
using StarAnimation.Utils.Area;

using SharedLib.MathUtils;

namespace StarAnimation.Core.Effect.Instance
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
            Vector2F center = new Vector2F(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            float duration = config.DurationRange.GetRandom();

            return new ColorShiftInstance(center, area, duration, config.EffectAppliedChance);
        }

        /// <summary>
        /// Initializes color shift effect (applies to stars based on probability).
        /// </summary>
        protected override void OnApplyTo(IReadOnlyList<Star> stars)
        {
            float currentTime = Environment.TickCount;

            foreach (var star in stars)
            {
                bool inArea = Area.Contains(star.Position.Current);

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
                        star.ColorShift.Delay = Rand.NextFloat() * 2.0f;
                        star.ColorShift.BiasDirection = Rand.NextFloat() < 0.5 ? -1f : 1f;
                        star.Color.Base = star.ColorShift.BiasDirection < 0
                            ? Color.FromArgb(255, 0, 0) // Red
                            : Color.FromArgb(0, 0, 255); // Blue
                        affectedStars.Add(star);
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
            float elapsedTime = normalizedTime * Duration;

            if (affectedStars.Count == 0)
            {
                return;
            }

            for (int i = affectedStars.Count - 1; i >= 0; i--)
            {
                var star = affectedStars[i];

                if (!star.ColorShift.HasPhase) continue;
                if (elapsedTime - star.ColorShift.Delay > Duration)
                {
                    star.ColorShift.HasPhase = false;
                    star.Color.Current = Color.White;
                    affectedStars.RemoveAt(i);
                    continue;
                }

                float timeSinceStart = elapsedTime - star.ColorShift.Delay;
                if (timeSinceStart < 0.0f) continue;

                float wave = (float)Math.Sin(2.0f * Math.PI * timeSinceStart / Duration);
                Color targetColor = MathUtil.LerpColor(Color.White, star.Color.Base, wave);
                star.Color.Current = targetColor;
            }
        }
        protected override void OnReset()
        {
            foreach (var star in affectedStars)
            {
                star.ColorShift.HasPhase = false;
                star.Opacity = 1.0f;
            }

            affectedStars.Clear();
        }
    }
}
