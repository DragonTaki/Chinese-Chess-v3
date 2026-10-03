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
using System.Linq;

using StarAnimation.Core.Effect.Parameter;
using StarAnimation.Models;
using StarAnimation.Utils.Area;

using Engine.Mathematics;

namespace StarAnimation.Core.Effect.Instance
{
    /// <summary>
    /// Applies a color shift to a list of stars: each affected star goes from its own color
    /// (white) to its target color (red or blue) and back over the effect's duration, each with
    /// its own random start delay (author decision 2026-10-04: original → target → original).
    /// </summary>
    public class ColorShiftInstance : EffectInstance
    {
        private readonly List<Star> _affectedStars;

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
            _affectedStars = new List<Star>();
        }

        /// <summary>
        /// Latest ColorShift.Delay among the stars still shifting, so the instance
        /// is not reset before its last delayed star has finished.
        /// </summary>
        protected override float MaxStartDelay => _affectedStars.Count == 0
            ? 0f
            : _affectedStars.Max(star => star.ColorShift.Delay);

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
                    if (_rand.NextFloat() < EffectAppliedChance)
                    {
                        star.ColorShift.HasPhase = true;
                        star.ColorShift.Delay = _rand.NextFloat() * 2.0f;
                        star.ColorShift.BiasDirection = _rand.NextFloat() < 0.5 ? -1f : 1f;
                        star.Color.Base = star.ColorShift.BiasDirection < 0
                            ? Color.FromArgb(255, 0, 0) // Red
                            : Color.FromArgb(0, 0, 255); // Blue
                        _affectedStars.Add(star);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the color shift effect, transitioning each star's color smoothly.
        /// Should be called every frame.
        /// </summary>
        /// <param name="normalizedTime">
        /// Elapsed time divided by Duration: 0 to 1 over the effect's lifecycle, and past 1
        /// while stars with a start delay are still finishing.
        /// </param>
        protected override void OnUpdate(float normalizedTime)
        {
            float elapsedTime = normalizedTime * Duration;

            if (_affectedStars.Count == 0)
            {
                return;
            }

            for (int i = _affectedStars.Count - 1; i >= 0; i--)
            {
                var star = _affectedStars[i];

                if (!star.ColorShift.HasPhase) continue;
                if (elapsedTime - star.ColorShift.Delay > Duration)
                {
                    star.ColorShift.HasPhase = false;
                    star.Color.Current = Color.White;
                    _affectedStars.RemoveAt(i);
                    continue;
                }

                float timeSinceStart = elapsedTime - star.ColorShift.Delay;
                if (timeSinceStart < 0.0f) continue;

                // Half a sine over the whole duration: 0 → 1 → 0, so the star is white → target →
                // white (a full sine left the negative half clamped to white, half the time unchanged).
                float wave = (float)Math.Sin(Math.PI * timeSinceStart / Duration);
                Color targetColor = MathUtil.LerpColor(Color.White, star.Color.Base, wave);
                star.Color.Current = targetColor;
            }
        }
        protected override void OnReset()
        {
            foreach (var star in _affectedStars)
            {
                star.ColorShift.HasPhase = false;
                star.Color.Current = Color.White;
            }

            _affectedStars.Clear();
        }
    }
}
