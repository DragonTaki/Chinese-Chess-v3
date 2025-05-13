/* ----- ----- ----- ----- */
// EffectInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using StarAnimation.Models;
using StarAnimation.Utils.Area;

using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;
using SharedLib.RandomTable;
using SharedLib.Timing;

namespace StarAnimation.Core.Effect
{
    public abstract class EffectInstance : IEffectInstance
    {
        public Guid InstanceId { get; } = Guid.NewGuid();
        public Vector2F Center { get; protected set; }
        public IAreaShape Area { get; protected set; }
        public float Duration { get; protected set; }
        public float EffectAppliedChance { get; protected set; }
        public float TimeProgress { get; protected set; }
        protected IRandomProvider Rand = GlobalRandom.Instance;


        private static readonly ConcurrentDictionary<Guid, byte> _activeIds = new();
        public static void Register(Guid id) => _activeIds.TryAdd(id, 0);
        public static void Unregister(Guid id) => _activeIds.TryRemove(id, out _);
        public static HashSet<Guid> GetAllActiveEffectIds() => _activeIds.Keys.ToHashSet();
        private readonly List<Physics2D> _affectedPhysics = new();
        
        protected List<Star> CreateNewAffectedStars = new();
        private float MaxEndTime => CreateNewAffectedStars.Count == 0
            ? Duration
            : CreateNewAffectedStars.Max(star => star.Pulse.Delay + Duration);
        public bool IsActive => TimeProgress < MaxEndTime;

        public EffectInstance(Vector2F center, IAreaShape area, float duration, float effectAppliedChance)
        {
            Center = center;
            Area = area;
            Duration = duration;
            EffectAppliedChance = effectAppliedChance;
            TimeProgress = 0f;
        }

        public virtual void ApplyTo(IReadOnlyList<Star> stars)
        {
            Register(InstanceId);
            CreateNewAffectedStars.Clear();
            _affectedPhysics.Clear();
            foreach (var star in stars)
            {
                if (Area.Contains(star.Position.Current))
                    CreateNewAffectedStars.Add(star);
                    _affectedPhysics.Add(star.Physics);
            }
            OnApplyTo(CreateNewAffectedStars);
        }
        protected abstract void OnApplyTo(IReadOnlyList<Star> stars);

        public void Update()
        {
            if (!IsActive) return;
            TimeProgress += GlobalTime.Timer.DeltaTimeInSeconds;
            OnUpdate(TimeProgress / Duration);  // normalized time (0~1)

            // If over animation time, reset affectedStars
            if (TimeProgress >= MaxEndTime)
            {
                Reset();
            }
        }

        protected abstract void OnUpdate(float normalizedTime);
        public void Reset()
        {
            Unregister(InstanceId);
            foreach (var physics in _affectedPhysics)
            {
                physics.AccelerationContributions.Remove(InstanceId);
            }
            _affectedPhysics.Clear();
            OnReset();
        }
        protected abstract void OnReset();
    }
}
