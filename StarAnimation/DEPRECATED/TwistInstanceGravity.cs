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
            // Speed ramps down toward the middle of the effect duration and then ramps up again
            float speedBoost = 1.0f + MaxSpeedBoost * (1.0f - Math.Abs(2 * normalizedTime - 1));

            foreach (var star in affectedStars)
            {
                // === 扭曲重點：以 center 為旋轉中心 ===
                Vector2F toCenter = star.Position.Current - Center;
                float radius = toCenter.Length();

                if (radius < 0.01f)
                    continue; // 太靠近中心不旋轉

        Vector2F radialDir = toCenter.Normalize();

        // === 切線方向（正交方向）===
        Vector2F tangentDir = new Vector2F(-radialDir.Y, radialDir.X); // 順時針方向
        if (Direction < 0)
            tangentDir = new Vector2F(radialDir.Y, -radialDir.X); // 逆時針方向

        // === 扭曲角度影響 ===
        float twistAngle = Direction * MaxAngle * normalizedTime;
        Vector2F twistedTangent = tangentDir.Rotate(twistAngle);

        // === 計算半徑衰減因子 ===
        float falloff = 1.0f / (1.0f + radius);

        // === 引力加速度（徑向）===
        float gravityConstant = 50f;
        float safeRadius = Math.Max(radius, 0.01f); // 防止除以零
        Vector2F gravityAccel = (-1f) * radialDir * (gravityConstant / (safeRadius * safeRadius));

        // === 切向力動態調整（方案 A）===
        Vector2F velocity = star.Physics.Velocity.Current;
        if (Math.Sqrt(velocity.Length()) > 0.0001f)
        {
            Vector2F velocityDir = velocity.Normalize();
            // 改成使用工具方法計算內積
            float alignment = Vector2F.DotProduct(velocityDir, (-1f) * radialDir);

            float twistFactor = 0.5f + 0.5f * MathF.Max(0, alignment); // alignment 越小 twist 越弱

            // === 扭曲角加速度（角動量來源） ===
            float twistStrength = 0.15f * falloff * speedBoost * twistFactor;
            Vector2F twistAccel = twistedTangent * twistStrength;

            // === 總合加速度 ===
            Vector2F finalAccel = gravityAccel + twistAccel;
            star.Physics.AccelerationContributions[InstanceId] = finalAccel;
        }
        else
        {
            // 沒有速度方向時，只使用徑向引力
            star.Physics.AccelerationContributions[InstanceId] = gravityAccel;
        }
                star.Physics.SmoothUpdate();
            }
        }
        
        protected override void OnReset()
        {
            affectedStars.Clear();
        }
    }
}
