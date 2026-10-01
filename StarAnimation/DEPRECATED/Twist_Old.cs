/* ----- ----- ----- ----- */
// Twist_Old.cs
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
    /// Applies a twist distortion effect to stars, by offsetting stars along a rotating vector.
    /// Each affected star is shifted by a vector whose angle follows the system tick count
    /// and whose length scales with a random share of Strength (which decays on every Apply call).
    /// </summary>
    public class Twist_Old
    {
        #region Settings (Adjustable Parameters)

        /// <summary>
        /// The probability that the twist effect is applied to each star (0.0 ~ 1.0).
        /// </summary>
        public float EffectAppliedChance { get; set; } = 0.4f;

        /// <summary>
        /// The maximum distortion radius applied to a star.
        /// </summary>
        public float MaxRadius { get; set; } = 100f;

        /// <summary>
        /// The strength factor of the twist effect per star (0.0 ~ 1.0).
        /// </summary>
        public float Strength { get; set; } = 1.0f;

        /// <summary>
        /// How fast the strength decays over time (0.0 ~ 1.0).
        /// </summary>
        public float DecayRate { get; set; } = 0.97f;

        #endregion

        /// <summary>
        /// Constructs a Twist effect with optional parameter overrides.
        /// Chance, strength and decay values are passed through MathUtil.Sigmoid01; maxRadius is clamped to be non-negative.
        /// </summary>
        public Twist_Old(
            float? effectAppliedChance = null,
            float? maxRadius = null,
            float? strength = null,
            float? decayRate = null)
        {
            if (effectAppliedChance.HasValue)
                EffectAppliedChance = MathUtil.Sigmoid01(effectAppliedChance.Value);
            if (maxRadius.HasValue)
                MaxRadius = Math.Max(0.0f, maxRadius.Value);
            if (strength.HasValue)
                Strength = MathUtil.Sigmoid01(strength.Value);
            if (decayRate.HasValue)
                DecayRate = MathUtil.Sigmoid01(decayRate.Value);
        }

        /// <summary>
        /// Applies the twist effect to a list of stars.
        /// Each affected star's position is shifted by a time-dependent rotating offset; Strength
        /// is then multiplied by DecayRate once per call.
        /// </summary>
        /// <param name="stars">The list of stars to apply the effect to.</param>
        /// <param name="rand">Random generator for probabilistic application.</param>
        public void Apply(List<Star> stars, Random rand)
        {
            float angle = Environment.TickCount / 1000.0f * 2f * (float)Math.PI;

            foreach (var star in stars)
            {
                if (rand.NextDouble() < EffectAppliedChance)
                {
                    float dynamicStrength = Strength * MathUtil.Sigmoid01((float)rand.NextDouble());
                    float radius = MaxRadius * dynamicStrength;

                    float offsetX = radius * (float)Math.Cos(angle);
                    float offsetY = radius * (float)Math.Sin(angle);

                    star.X += offsetX;
                    star.Y += offsetY;

                    // Optionally, you could store star-specific strength and apply decay over time
                    // if per-frame continuity is needed (e.g. if Star class tracks strength)
                }
            }

            // Simulate global decay (if needed externally or per-frame)
            Strength *= DecayRate;
        }
    }
}
