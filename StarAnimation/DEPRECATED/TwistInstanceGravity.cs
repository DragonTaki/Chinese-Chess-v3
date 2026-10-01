/* ----- ----- ----- ----- */
// TwistInstanceGravity.cs
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

using Engine.Mathematics;
using SharedLib.RandomTable;

namespace StarAnimation.Core.Effect
{
    /// <summary>
    /// Represents a twist-plus-gravity effect applied to stars: affected stars receive a radial
    /// pull toward the effect center plus a tangential (twisting) acceleration.
    /// </summary>
    public class GravityInstance : EffectInstance
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
        private const float MaxSpeedBoost = 1.5f;

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
/*
            do
            {
                float x = GlobalRandom.Instance.NextFloat(bounds.Left, bounds.Right);
                float y = GlobalRandom.Instance.NextFloat(bounds.Top, bounds.Bottom);
                center = new Vector2F(x, y);
            } while (!area.Contains(center) && --maxTries > 0);

            // Set effect center to area center if tries all failed
            if (maxTries <= 0)*/
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
            // Speed boost ramps up toward the middle of the effect duration (peak at normalizedTime = 0.5) and then ramps down again
            float speedBoost = 1.0f + MaxSpeedBoost * (1.0f - Math.Abs(2 * normalizedTime - 1));

            foreach (var star in affectedStars)
            {
                // === Twist core: rotate around the effect center ===
                Vector2F toCenter = star.Position.Current - Center;
                float radius = toCenter.Length();

                if (radius < 0.01f)
                    continue; // Too close to the center, skip rotation

                Vector2F radialDir = toCenter.Normalize();

                // === Tangent direction (perpendicular to the radial direction) ===
                Vector2F tangentDir = new Vector2F(-radialDir.Y, radialDir.X); // Clockwise in screen coordinates (y axis points down)
                if (Direction < 0)
                    tangentDir = new Vector2F(radialDir.Y, -radialDir.X); // Counter-clockwise in screen coordinates

                // === Twist angle contribution ===
                float twistAngle = Direction * MaxAngle * normalizedTime;
                Vector2F twistedTangent = tangentDir.Rotate(twistAngle);

                // === Radius falloff factor ===
                float falloff = 1.0f / (1.0f + radius);

                // === Gravity acceleration (radial) ===
                float gravityConstant = 50f;
                float safeRadius = Math.Max(radius, 0.01f); // Guard against division by zero
                Vector2F gravityAccel = (-1f) * radialDir * (gravityConstant / (safeRadius * safeRadius));

                // === Dynamic tangential force adjustment (option A) ===
                Vector2F velocity = star.Physics.Velocity.Current;
                if (Math.Sqrt(velocity.Length()) > 0.0001f)
                {
                    Vector2F velocityDir = velocity.Normalize();
                    // Use the utility method to compute the dot product
                    float alignment = Vector2F.DotProduct(velocityDir, (-1f) * radialDir);

                    float twistFactor = 0.5f + 0.5f * MathF.Max(0, alignment); // the smaller the alignment, the weaker the twist

                    // === Twist acceleration (source of angular momentum) ===
                    float twistStrength = 0.15f * falloff * speedBoost * twistFactor;
                    Vector2F twistAccel = twistedTangent * twistStrength;

                    // === Total acceleration ===
                    Vector2F finalAccel = gravityAccel + twistAccel;
                    star.Physics.AccelerationContributions[InstanceId] = finalAccel;
                }
                else
                {
                    // With no velocity direction, only the radial gravity is used
                    star.Physics.AccelerationContributions[InstanceId] = gravityAccel;
                }
            }
        }
        
        protected override void OnReset()
        {
            affectedStars.Clear();
        }
    }
}
