/* ----- ----- ----- ----- */
// Pulse.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using StarAnimation.Utils;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Applies a time-based sinusoidal opacity (pulse) effect to each star individually.
    /// Each star pulses with a unique phase to avoid synchronous blinking.
    /// </summary>
    public class Pulse
    {
        #region Settings (Adjustable Parameters)

        /// <summary>
        /// The base opacity value (0.0 ~ 1.0).
        /// </summary>
        public float BaseOpacity { get; set; } = 0.5f;

        /// <summary>
        /// The amplitude of the pulse (0.0 ~ 1.0).
        /// </summary>
        public float Amplitude { get; set; } = 0.5f;

        /// <summary>
        /// The speed multiplier for pulse oscillation (larger = faster pulses).
        /// </summary>
        public float PulseSpeed { get; set; } = 0.01f;

        #endregion

        /// <summary>
        /// Constructs the Pulse effect with optional configuration parameters.
        /// </summary>
        public Pulse(
            float? baseOpacity = null,
            float? amplitude = null,
            float? pulseSpeed = null)
        {
            if (baseOpacity.HasValue)
                BaseOpacity = MathUtil.Sigmoid01(baseOpacity.Value);
            if (amplitude.HasValue)
                Amplitude = MathUtil.Sigmoid01(amplitude.Value);
            if (pulseSpeed.HasValue)
                PulseSpeed = MathUtil.ClampMinFloat(pulseSpeed.Value);
        }

        /// <summary>
        /// Applies the pulse effect to each star with a randomized phase.
        /// </summary>
        /// <param name="stars">The list of stars to apply the effect to.</param>
        /// <param name="rand">Random generator used to assign initial phases.</param>
        public void Apply(List<Star> stars, Random rand)
        {
            float time = Environment.TickCount * PulseSpeed;

            foreach (var star in stars)
            {
                if (!star.HasPulsePhase)
                {
                    // Assign random phase offset once
                    star.PulsePhase = (float)(rand.NextDouble() * 2 * Math.PI);
                    star.HasPulsePhase = true;
                }

                float strength = BaseOpacity + Amplitude * (float)Math.Sin(time + star.PulsePhase);
                star.Opacity = strength;
            }
        }
    }
}