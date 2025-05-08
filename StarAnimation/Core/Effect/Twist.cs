/* ----- ----- ----- ----- */
// Twist.cs
// Smooth spiral distortion effect for stars
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using StarAnimation.Utils;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Represents a spiral twist effect with smooth fade-in/out.
    /// Stars rotate around a center point using a sinusoidal animation curve.
    /// </summary>
    public class Twist
    {
        #region Settings (Adjustable Parameters)

        /// <summary>
        /// Maximum outward spiral radius.
        /// </summary>
        public float MaxRadius { get; set; } = 40f;

        /// <summary>
        /// Strength factor for angular distortion.
        /// </summary>
        public float Strength { get; set; } = 1.0f;

        /// <summary>
        /// Total animation duration in frames.
        /// </summary>
        public float Duration { get; set; } = 60f;


        /// <summary>
        /// Direction of rotation: 1 = clockwise, -1 = counter-clockwise.
        /// </summary>
        public float RotationDirection { get; set; } = 1f;

        #endregion

        /// <summary>
        /// Current frame progress of the effect.
        /// </summary>
        public float TimeProgress = 0f;

        /// <summary>
        /// Current twist center (randomized at start).
        /// </summary>
        private PointF center;

        /// <summary>
        /// Whether the effect is active (non-zero progress).
        /// </summary>
        public bool IsActive => TimeProgress > 0;

        public Twist(
            float? maxRadius = null,
            float? strength = null,
            float? duration = null,
            float? rotationDirection = null)
        {
            if (maxRadius.HasValue)
                MaxRadius = Math.Max(0.0f, maxRadius.Value);
            if (strength.HasValue)
                Strength = MathUtil.Sigmoid01(strength.Value);
            if (duration.HasValue)
                Duration = MathUtil.Sigmoid01(duration.Value);
            if (rotationDirection.HasValue)
                RotationDirection = MathUtil.Sigmoid01(rotationDirection.Value);
        }

        /// <summary>
        /// Applies the spiral twist effect to stars based on current time progress.
        /// Animation progresses using an ease-in-out curve (sinusoidal).
        /// </summary>
        /// <param name="stars">The list of stars to apply the effect to.</param>
        /// <param name="rand">Random generator for initializing twist position and direction.</param>
        public void Apply(List<Star> stars, Random rand)
        {
            // Start a new twist if inactive
            if (TimeProgress == 0)
            {
                center = new PointF(rand.Next(0, 1920), rand.Next(0, 1080));
                RotationDirection = rand.NextDouble() < 0.5 ? 1f : -1f;
            }

            // Normalize time progress (0 ~ 1)
            float t = TimeProgress / Duration;

            // Use a sine wave for smooth ease-in and ease-out (0 → 1 → 0)
            float ease = (float)Math.Sin(t * Math.PI);

            // Calculate angular offset and radial strength
            float angleOffset = RotationDirection * ease * Strength * 2f * (float)Math.PI;
            float radiusLimit = MaxRadius;

            foreach (var star in stars)
            {
                float dx = star.X - center.X;
                float dy = star.Y - center.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                // Only twist stars within MaxRadius
                if (distance < radiusLimit)
                {
                    float angle = (float)Math.Atan2(dy, dx);
                    float newAngle = angle + angleOffset;

                    // Maintain original radius, only rotate
                    star.X = center.X + distance * (float)Math.Cos(newAngle);
                    star.Y = center.Y + distance * (float)Math.Sin(newAngle);
                }
            }

            // Advance time progress
            TimeProgress++;

            // End effect when duration completes
            if (TimeProgress >= Duration)
            {
                TimeProgress = 0f;
            }
        }
    }
}
