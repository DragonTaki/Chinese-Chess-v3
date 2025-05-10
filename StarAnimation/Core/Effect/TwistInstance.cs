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

using StarAnimation.Utils;
using StarAnimation.Utils.Area;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Represents a twist effect applied to stars.
    /// </summary>
    public class TwistInstance : EffectInstance
    {
        public PointF Center { get; private set; }
        public IAreaShape Area { get; private set; }
        public float Duration { get; private set; }
        private Random rand;
        public float Strength { get; private set; }
        public float Radius { get; private set; }
        public float Direction { get; private set; }
        public float EffectAppliedChance { get; private set; }
        private List<Star> affectedStars = new();

        public TwistInstance(PointF center, IAreaShape area, float duration, Random rand, float strength, float radius, float direction, float effectAppliedChance)
            : base(center, area, duration, rand)
        {
            Center = center;
            Area = area;
            Duration = duration;
            this.rand = rand;
            Strength = strength;
            Radius = radius;
            Direction = direction;
            EffectAppliedChance = effectAppliedChance;
        }

        public static TwistInstance CreateRandom(IAreaShape area, TwistParameterRange config, Random rand)
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
            float radius = config.RadiusRange.GetRandom(rand);
            float strength = config.StrengthRange.GetRandom(rand);
            float direction = rand.NextDouble() < config.ClockwiseChance ? 1f : -1f;

            return new TwistInstance(center, area, duration, rand, strength, radius, direction, config.EffectAppliedChance);
        }

        protected override void OnApplyTo(List<Star> stars)
        {
            affectedStars.Clear();
            
            foreach (var star in stars)
            {
                if (Area.Contains(star.Position))
                {
                    if (rand.NextDouble() < EffectAppliedChance)
                    {
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
                    if (Area.Contains(star.Position))
                    {
                        float dx = star.X - Center.X;
                        float dy = star.Y - Center.Y;
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
                    affectedStars.Add(closest);
                }
            }
        }

        protected override void OnUpdate(float deltaTimeInSeconds)
        {
            float angleOffset = Direction * (float)Math.Sin(deltaTimeInSeconds * Math.PI) * Strength * 2f * (float)Math.PI;

            foreach (var star in affectedStars)
            {
                float dx = star.X - Center.X;
                float dy = star.Y - Center.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance < Radius)
                {
                    float angle = (float)Math.Atan2(dy, dx);
                    float newAngle = angle + angleOffset;

                    star.X = Center.X + distance * (float)Math.Cos(newAngle);
                    star.Y = Center.Y + distance * (float)Math.Sin(newAngle);
                }
            }
        }
        
        protected override void Reset()
        {
            affectedStars.Clear();
        }
    }
}
