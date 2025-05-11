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

using StarAnimation.Utils.Area;

using SharedLib.MathUtils;
using SharedLib.RandomTable;

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
        private List<Star> affectedStars;

        /// <summary>
        /// Constructs the Pulse effect with optional configuration parameters.
        /// </summary>
        public PulseInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance, float amplitude, float midOpacity)
            : base(center, area, duration, effectAppliedChance)
        {
            Center = center;
            Area = area;
            Duration = duration;
            EffectAppliedChance = effectAppliedChance;
            Amplitude = amplitude;
            MidOpacity = midOpacity;
            affectedStars = new List<Star>();
        }

        public static PulseInstance CreateRandom(IAreaShape area, PulseParameter config)
        {
            RectangleF bounds = area.BoundingBox;
            Vector2F center;
            int maxTries = 100;

            do
            {
                float x = GlobalRandom.Instance.NextFloat(bounds.Left, bounds.Right);
                float y = GlobalRandom.Instance.NextFloat(bounds.Top, bounds.Bottom);
                center = new Vector2F(x, y);
            } while (!area.Contains(center) && --maxTries > 0);

            // Set effect center to area center if tries all failed
            if (maxTries <= 0)
                center = new Vector2F(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            float duration = config.DurationRange.GetRandom();
            float amplitude = config.AmplitudeRange.GetRandom();
            float midOpacity = config.MidOpacityRange.GetRandom();

            return new PulseInstance(center, area, duration, config.EffectAppliedChance, amplitude, midOpacity);
        }

        /// <summary>
        /// Initializes the pulse phase for each star (only once per star).
        /// </summary>
        protected override void OnApplyTo(List<Star> stars)
        {
            affectedStars.Clear();

            foreach (var star in stars)
            {
                if (Area.Contains(star.Position.Current) && !star.Pulse.HasPhase)
                {
                    if (Rand.NextFloat() < EffectAppliedChance)
                    {
                        star.Pulse.Delay = (float)(Rand.NextDouble() * 2.0);
                        star.Pulse.HasPhase = true;
                        star.Pulse.ShiningTimes = Rand.NextInt(1, 4);
                        affectedStars.Add(star);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the opacity of stars based on sinusoidal wave with start delay.
        /// Should be called every frame.
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

                if (!star.Pulse.HasPhase) continue;
                if (elapsedTime - star.Pulse.Delay > Duration)
                {
                    star.Pulse.HasPhase = false;
                    star.Opacity = 1f;
                    affectedStars.RemoveAt(i);
                    continue;
                }

                float timeSinceStart = elapsedTime - star.Pulse.Delay;
                if (timeSinceStart < 0f) continue;

                float wave = (float)Math.Sin(2 * Math.PI * star.Pulse.ShiningTimes * timeSinceStart / Duration);
                float opacity = MidOpacity + Amplitude * wave;
                star.Opacity = Math.Clamp(opacity, 0f, 1f);
            }
        }
        protected override void Reset()
        {
            foreach (var star in affectedStars)
            {
                star.Pulse.HasPhase = false;
                star.Opacity = 1f;
            }

            affectedStars.Clear();
        }
    }
}
