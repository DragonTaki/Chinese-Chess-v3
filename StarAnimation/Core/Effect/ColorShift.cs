/* ----- ----- ----- ----- */
// ColorShift.cs
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

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Applies a sinusoidal red-blue color shift effect to a list of stars.
    /// Each star shifts color independently using a randomized phase.
    /// </summary>
    public class ColorShift
    {
        #region Settings (Adjustable Parameters)

        /// <summary>
        /// The probability that a star receives the effect (range: 0.0 ~ 1.0).
        /// </summary>
        public float EffectAppliedChance { get; set; } = 0.5f;

        /// <summary>
        /// The baseline color offset (0.0 = full red, 1.0 = full blue).
        /// </summary>
        public float ColorBias { get; set; } = 0.5f;

        /// <summary>
        /// The amplitude of the sine wave color oscillation.
        /// </summary>
        public float ColorAmplitude { get; set; } = 0.5f;

        /// <summary>
        /// Speed factor for sine wave (larger = faster color changes).
        /// </summary>
        public float TimeScale { get; set; } = 0.002f;

        #endregion

        /// <summary>
        /// Constructs the ColorShift effect with optional parameters.
        /// </summary>
        public ColorShift(
            float? effectAppliedChance = null,
            float? colorBias = null,
            float? colorAmplitude = null,
            float? timeScale = null)
        {
            if (effectAppliedChance.HasValue)
                EffectAppliedChance = MathUtil.Sigmoid01(effectAppliedChance.Value);
            if (colorBias.HasValue)
                ColorBias = MathUtil.Sigmoid01(colorBias.Value);
            if (colorAmplitude.HasValue)
                ColorAmplitude = MathUtil.Sigmoid01(colorAmplitude.Value);
            if (timeScale.HasValue)
                TimeScale = MathUtil.ClampMinFloat(0.0001f, timeScale.Value);
        }

        /// <summary>
        /// Applies a sinusoidal red-blue color shift to stars based on a time-varying function.
        /// Each star has an independent phase for desynchronized animation.
        /// </summary>
        /// <param name="stars">The list of stars to apply the color effect to.</param>
        /// <param name="rand">A random number generator for probabilistic application and phase assignment.</param>
        public void Apply(List<Star> stars, Random rand)
        {
            float time = Environment.TickCount * TimeScale;

            foreach (var star in stars)
            {
                if (!star.HasColorShiftPhase)
                {
                    star.ColorShiftPhase = (float)(rand.NextDouble() * 2 * Math.PI);
                    star.HasColorShiftPhase = true;
                }

                float t = (float)Math.Sin(time + star.ColorShiftPhase);
                int red = MathUtil.ClampToByte(255 * (ColorBias - ColorAmplitude * t));
                int blue = MathUtil.ClampToByte(255 * (ColorBias + ColorAmplitude * t));
                Color shiftColor = Color.FromArgb(red, 0, blue);

                if (rand.NextDouble() < EffectAppliedChance)
                {
                    star.Color = shiftColor;
                }
                else
                {
                    star.Color = Color.White;
                }
            }
        }
    }
}
