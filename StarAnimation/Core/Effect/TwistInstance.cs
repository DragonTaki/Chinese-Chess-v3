/* ----- ----- ----- ----- */
// TwistInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
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
    /// Represents a twist effect applied to stars.
    /// </summary>
    public class TwistInstance : EffectInstance
    {
        private class StarInfo
        {
            public Star Star;
            public float InitialAngle;
            public float Distance;
        }
        private readonly List<StarInfo> starInfos = new();
        public float Strength { get; private set; }
        public float Radius { get; private set; }
        public float Direction { get; private set; }
        private List<Star> affectedStars = new();
        private const float MaxAngle = 2f * (float)Math.PI;

        public TwistInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance, float strength, float radius, float direction)
            : base(center, area, duration, effectAppliedChance)
        {
            Center = center;
            Area = area;
            Duration = duration;
            EffectAppliedChance = effectAppliedChance;
            Strength = strength;
            Radius = radius;
            Direction = direction;
        }

        public static TwistInstance CreateRandom(IAreaShape area, TwistParameter config)
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
            float radius = config.RadiusRange.GetRandom();
            float strength = config.StrengthRange.GetRandom();
            float direction = GlobalRandom.Instance.NextFloat() < config.ClockwiseChance ? 1f : -1f;

            return new TwistInstance(center, area, duration, config.EffectAppliedChance, strength, radius, direction);
        }
        private void InitializeStarInfo(Star star)
        {
            float dx = star.Position.Current.X - Center.X;
            float dy = star.Position.Current.Y - Center.Y;

            starInfos.Add(new StarInfo
            {
                Star = star,
                InitialAngle = (float)Math.Atan2(dy, dx),
                Distance = (float)Math.Sqrt(dx * dx + dy * dy)
            });
        }
        protected override void OnApplyTo(List<Star> stars)
        {
            affectedStars.Clear();
            starInfos.Clear();
            
            foreach (var star in stars)
            {
                if (Area.Contains(star.Position.Current))
                {
                    if (Rand.NextDouble() < EffectAppliedChance)
                    {
                        InitializeStarInfo(star);
                        affectedStars.Add(star);
                    }
                }
            }

            // If no stars are affected, at least the nearest one will be selected.
            if (affectedStars.Count == 0)
            {
#nullable enable
                Star? closest = null;
#nullable disable
                float closestDistSq = float.MaxValue;

                foreach (var star in stars)
                {
                    if (Area.Contains(star.Position.Current))
                    {
                        float dx = star.Position.Current.X - Center.X;
                        float dy = star.Position.Current.Y - Center.Y;
                        float distSq = dx * dx + dy * dy;

                        if (distSq < closestDistSq)
                        {
                            closest = star;
                            closestDistSq = distSq;
                        }
                    }
                }

                if (closest != null)
                {
                    InitializeStarInfo(closest);
                    affectedStars.Add(closest);
                }
            }
        }

        /// <summary>
        /// Updates the positions of affected stars based on a twisting effect using normalized time.
        /// Should be called every frame.
        /// </summary>
        /// <param name="normalizedTime">
        /// A float value between 0 and 1 representing the progression of the effect's lifecycle.
        /// </param>
        protected override void OnUpdate(float normalizedTime)
        {
            float elapsedTime = normalizedTime * Duration;

            float speedFactor = 1.0f - Math.Abs(2 * normalizedTime - 1); // Speed ramps down as we reach the middle and back

            foreach (var star in affectedStars)
            {
                // Update position based on velocity, considering current angle (info.InitialAngle)
                float angle = star.Twist.InitialAngle + Direction * (float)Math.Sin(normalizedTime * Math.PI) * MaxAngle;

                star.Velocity.Target = star.Velocity.Base * speedFactor;
                star.Velocity.Current = MathUtil.Lerp(star.Velocity.Current, star.Velocity.Target, 0.1f);

                star.Position.Current.X += star.Velocity.Current.X * (float)Math.Cos(angle);
                star.Position.Current.Y += star.Velocity.Current.Y * (float)Math.Sin(angle);
            }
        }
        
        protected override void Reset()
        {
            affectedStars.Clear();
        }
    }
}
