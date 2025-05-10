/* ----- ----- ----- ----- */
// PulseInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/09
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
    /// Applies a time-based sinusoidal opacity (pulse) effect to each star individually.
    /// Each star pulses with a unique phase to avoid synchronous blinking.
    /// Only stars within the given area are affected.
    /// </summary>
    public class PulseInstance : EffectInstance
    {
        public float Amplitude { get; private set; } = 0.5f;
        public float MidOpacity { get; private set; } = 0.5f;
        public float EffectAppliedChance { get; private set; }
        private List<Star> affectedStars;

        /// <summary>
        /// Constructs the Pulse effect with optional configuration parameters.
        /// </summary>
        public PulseInstance(PointF center, IAreaShape area, float duration, Random rand, float amplitude, float midOpacity, float effectAppliedChance)
            : base(center, area, duration, rand)
        {
            Center = center;
            Area = area;
            Duration = duration;
            this.rand = rand;
            Amplitude = amplitude;
            MidOpacity = midOpacity;
            EffectAppliedChance = effectAppliedChance;
            affectedStars = new List<Star>();
        }

        public static PulseInstance CreateRandom(IAreaShape area, PulseParameterRange config, Random rand)
        {
            RectangleF bounds = area.BoundingBox;
            PointF center;
            int maxTries = 100;

            do
            {
                float x = MathUtil.GetRandomFloat(bounds.Left, bounds.Right, rand);
                float y = MathUtil.GetRandomFloat(bounds.Top, bounds.Bottom, rand);
                center = new PointF(x, y);
            } while (!area.Contains(center) && --maxTries > 0);

            // Set effect center to area center if tries all failed
            if (maxTries <= 0)
                center = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            float duration = config.DurationRange.GetRandom(rand);
            float amplitude = config.AmplitudeRange.GetRandom(rand);
            float midOpacity = config.MidOpacityRange.GetRandom(rand);

            return new PulseInstance(center, area, duration, rand, amplitude, midOpacity, config.EffectAppliedChance);
        }

        /// <summary>
        /// Initializes the pulse phase for each star (only once per star).
        /// </summary>
        protected override void OnApplyTo(List<Star> stars)
        {
            affectedStars.Clear();

            foreach (var star in stars)
            {
                if (Area.Contains(star.Position) && !star.HasPulsePhase)
                {
                    if (rand.NextDouble() < EffectAppliedChance)
                    {
                        star.PulseDelay = (float)(rand.NextDouble() * 2.0);
                        star.HasPulsePhase = true;
                        star.ShiningTimes = rand.Next(1, 4);
                        affectedStars.Add(star);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the opacity of stars based on sinusoidal wave with start delay.
        /// Should be called every frame.
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

                if (!star.HasPulsePhase) continue;
                if (elapsedTime - star.PulseDelay > Duration)
                {
                    star.HasPulsePhase = false;
                    star.Opacity = 1f;
                    affectedStars.RemoveAt(i);
                    continue;
                }

                float timeSinceStart = elapsedTime - star.PulseDelay;
                if (timeSinceStart < 0f) continue;

                float wave = (float)Math.Sin(2 * Math.PI * star.ShiningTimes * timeSinceStart / Duration);
                float opacity = MidOpacity + Amplitude * wave;
                star.Opacity = Math.Clamp(opacity, 0f, 1f);
            }
        }
        protected override void Reset()
        {
            foreach (var star in affectedStars)
            {
                star.HasPulsePhase = false;
                star.Opacity = 1f;
            }

            affectedStars.Clear();
        }
    }
}
