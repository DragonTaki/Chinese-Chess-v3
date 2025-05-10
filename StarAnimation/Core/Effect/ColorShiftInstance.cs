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
    public class ColorShiftInstance
    {
        #region Settings (Adjustable Parameters)

        /// <summary>
        /// The probability that a star receives the effect (range: 0.0 ~ 1.0).
        /// </summary>
        public float EffectAppliedChance { get; set; } = 0.8f;

        /// <summary>
        /// The total duration (in seconds) for the color shift to complete.
        /// </summary>
        public float TransitionDuration { get; set; } = 3.0f;

        /// <summary>
        /// The baseline color offset (0.0 = full red, 1.0 = full blue).
        /// </summary>
        /// [DEPRECATED]
        public float ColorBias { get; set; } = 0.5f;

        /// <summary>
        /// The amplitude of the sine wave color oscillation.
        /// </summary>
        /// [DEPRECATED]
        public float ColorAmplitude { get; set; } = 0.5f;

        /// <summary>
        /// Speed factor for sine wave (larger = faster color changes).
        /// </summary>
        /// [DEPRECATED]
        public float TimeScale { get; set; } = 10.0f;

        #endregion

        /// <summary>
        /// Constructs the ColorShift effect with optional parameters.
        /// </summary>
        public ColorShiftInstance(
            float? effectAppliedChance = null,
            float? transitionDuration = null)
        {
            if (effectAppliedChance.HasValue)
                EffectAppliedChance = MathUtil.Sigmoid01(effectAppliedChance.Value);
            if (transitionDuration.HasValue)
                TransitionDuration = MathUtil.ClampMinFloat(0.0001f, transitionDuration.Value);
        }

        /// <summary>
        /// Initializes color shift effect (applies to stars based on probability).
        /// </summary>
        public void Apply(List<Star> stars, IAreaShape area, Random rand)
        {
            float currentTime = Environment.TickCount;

            foreach (var star in stars)
            {
                bool inArea = area.Contains(star.Position);

                if (!inArea)
                {
                    star.HasColorShiftPhase = false;
                    star.CurrentColor = Color.White;
                    continue;
                }

                // Start transition
                if (!star.HasColorShiftPhase)
                {
                    if (rand.NextDouble() < EffectAppliedChance)
                    {
                        star.HasColorShiftPhase = true;
                        star.ColorShiftStartTime = currentTime;
                        star.ColorShiftBiasDirection = rand.NextDouble() < 0.5 ? -1f : 1f;
                        star.BaseColor = star.ColorShiftBiasDirection < 0
                            ? Color.FromArgb(255, 0, 0) // Red
                            : Color.FromArgb(0, 0, 255); // Blue
                    }
                    else
                    {
                        star.CurrentColor = Color.White;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the color shift effect, transitioning each star's color smoothly.
        /// </summary>
        public void Update(List<Star> stars)
        {
            float currentTime = Environment.TickCount;
            float transitionMs = TransitionDuration * 1000f;

            foreach (var star in stars)
            {
                if (!star.HasColorShiftPhase) continue;

                float elapsed = currentTime - star.ColorShiftStartTime;
                if (elapsed >= transitionMs)
                {
                    star.HasColorShiftPhase = false;
                    star.CurrentColor = Color.White;
                    continue;
                }

                float normalized = elapsed / transitionMs;
                float wave = (float)Math.Sin(normalized * Math.PI); // smooth wave: 0 → 1 → 0

                Color targetColor = MathUtil.LerpColor(Color.White, star.BaseColor, wave);
                star.CurrentColor = targetColor;
            }
        }
    }
}
