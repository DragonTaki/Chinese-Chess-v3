/* ----- ----- ----- ----- */
// OverscrollCalculator.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace SharedLib.PhysicsUtils
{
    /// <summary>
    /// Calculates overscroll rebound acceleration using spring-damping physics.
    /// </summary>
    public class OverscrollCalculator
    {
        /// <summary>
        /// The stiffness of the spring (higher value means stronger rebound).
        /// </summary>
        public float SpringConstant { get; set; } = 0.5f;

        /// <summary>
        /// The damping constant to reduce oscillation (like friction).
        /// </summary>
        public float Damping { get; set; } = 0.7f;

        /// <summary>
        /// Computes rebound acceleration based on current scroll position and velocity.
        /// </summary>
        /// <param name="scrollY">The current scroll offset.</param>
        /// <param name="scrollVelocity">The current scroll velocity.</param>
        /// <param name="contentHeight">Total content height.</param>
        /// <param name="viewportHeight">Visible viewport height.</param>
        /// <returns>Overscroll rebound acceleration (positive or negative).</returns>
        public float GetOverscrollAcceleration(
            float scrollY, float scrollVelocity,
            float contentHeight, float viewportHeight)
        {
            float lowerBound = 0.0f;
            float upperBound = Math.Max(contentHeight - viewportHeight, 0);
            float overscroll = 0.0f;

            if (scrollY < lowerBound)
            {
                overscroll = scrollY - lowerBound;
            }
            else if (scrollY > upperBound)
            {
                overscroll = scrollY - upperBound;
            }
            else
            {
                return 0;
            }

            return -SpringConstant * overscroll - Damping * scrollVelocity;
        }

        /// <summary>
        /// Calculates soft overscroll offset for visual effect or debug use.
        /// </summary>
        /// <param name="scrollY">Current scroll offset.</param>
        /// <param name="contentHeight">Content total height.</param>
        /// <param name="viewportHeight">Visible viewport height.</param>
        /// <param name="overscrollLimit">The max overscroll before hard clamp (from settings).</param>
        /// <returns>Soft overscroll offset for UI feedback.</returns>
        public float GetSoftOverscroll(
            float scrollY, float contentHeight, float viewportHeight,
            float overscrollLimit)
        {
            float maxScroll = Math.Max(contentHeight - viewportHeight, 0);
            float softMargin = overscrollLimit / 2.0f;

            if (scrollY < -overscrollLimit - softMargin)
            {
                return scrollY + overscrollLimit;
            }
            else if (scrollY < -overscrollLimit)
            {
                float t = (-scrollY - overscrollLimit) / softMargin;
                return (scrollY + overscrollLimit) * t;
            }

            if (scrollY > maxScroll + overscrollLimit + softMargin)
            {
                return scrollY - (maxScroll + overscrollLimit);
            }
            else if (scrollY > maxScroll + overscrollLimit)
            {
                float t = (scrollY - maxScroll - overscrollLimit) / softMargin;
                return (scrollY - maxScroll - overscrollLimit) * t;
            }

            return 0;
        }
    }
}
