/* ----- ----- ----- ----- */
// EffectInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Linq;

using StarAnimation.Utils.Area;

using SharedLib.RandomTable;
using SharedLib.Timing;

namespace StarAnimation.Core.Effect
{
    public abstract class EffectInstance : IEffectInstance
    {
        public Vector2F Center { get; protected set; }
        public IAreaShape Area { get; protected set; }
        public float Duration { get; protected set; }
        public float EffectAppliedChance { get; protected set; }
        public float TimeProgress { get; protected set; }
        protected IRandomProvider Rand = GlobalRandom.Instance;

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

        public virtual void ApplyTo(List<Star> stars)
        {
           CreateNewAffectedStars.Clear();
            foreach (var star in stars)
            {
                if (Area.Contains(star.Position))
                    CreateNewAffectedStars.Add(star);
            }
            OnApplyTo(CreateNewAffectedStars);
        }
        protected abstract void OnApplyTo(List<Star> stars);

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
        protected abstract void Reset();
    }
}
