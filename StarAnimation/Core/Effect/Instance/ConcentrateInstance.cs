/* ----- ----- ----- ----- */
// ConcentrateInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Core.Effect.Parameter;
using StarAnimation.Models;
using StarAnimation.Utils.Area;

using Engine.Mathematics;
using Engine.Randomization;

namespace StarAnimation.Core.Effect.Instance
{
    /// <summary>
    /// Concentrate: stars within <see cref="Radius"/> of a random center are pulled toward it
    /// during the first half of the effect and pushed away (scattered) during the second half
    /// (author decision 2026-10-04), through the physics acceleration like Twist. The pull
    /// weakens toward the edge of the radius.
    /// </summary>
    public class ConcentrateInstance : EffectInstance
    {
        // Within this distance of the center a star gets no pull (avoids jitter on the center).
        private const float DeadZone = 5f;

        private readonly List<Star> _affectedStars = new();

        public float Radius { get; private set; }
        public float Strength { get; private set; }

        public ConcentrateInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance, float radius, float strength)
            : base(center, area, duration, effectAppliedChance)
        {
            Radius = radius;
            Strength = strength;
        }

        public static ConcentrateInstance CreateRandom(IAreaShape area, ConcentrateParameter config)
        {
            RectangleF bounds = area.BoundingBox;
            Vector2F center;
            int maxTries = 100;
            do
            {
                center = new Vector2F(GlobalRandom.Instance.NextFloat(bounds.Left, bounds.Right),
                    GlobalRandom.Instance.NextFloat(bounds.Top, bounds.Bottom));
            } while (!area.Contains(center) && --maxTries > 0);
            if (maxTries <= 0)
                center = new Vector2F(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            return new ConcentrateInstance(center, area, config.DurationRange.GetRandom(), config.EffectAppliedChance,
                config.RadiusRange.GetRandom(), config.StrengthRange.GetRandom());
        }

        protected override void OnApplyTo(IReadOnlyList<Star> stars)
        {
            _affectedStars.Clear();
            foreach (var star in stars)
            {
                if ((star.Position.Current - Center).Length() < Radius && _rand.NextDouble() < EffectAppliedChance)
                    _affectedStars.Add(star);
            }
        }

        protected override void OnUpdate(float normalizedTime)
        {
            // First half: toward the center; second half: away from it.
            float sign = normalizedTime < 0.5f ? 1f : -1f;
            foreach (var star in _affectedStars)
            {
                Vector2F toCenter = Center - star.Position.Current;
                float distance = toCenter.Length();
                if (distance < DeadZone || distance >= Radius * 2f)
                {
                    star.Physics.AccelerationContributions.Remove(InstanceId);
                    continue;
                }
                float falloff = MathF.Max(0f, 1f - distance / (Radius * 2f));
                star.Physics.AccelerationContributions[InstanceId] = toCenter / distance * (Strength * falloff * sign);
            }
        }

        protected override void OnReset()
        {
            _affectedStars.Clear();
        }
    }
}
