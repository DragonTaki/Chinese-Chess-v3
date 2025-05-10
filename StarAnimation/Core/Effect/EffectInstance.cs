/* ----- ----- ----- ----- */
// EffectInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StarAnimation.Utils.Area;

namespace StarAnimation.Core.Effect
{
    public abstract class EffectInstance : IEffectInstance
    {
        public PointF Center { get; protected set; }
        public IAreaShape Area { get; protected set; }
        public float Duration { get; protected set; }
        protected Random rand;
        public float TimeProgress { get; protected set; }

        protected List<Star> affectedStars = new();
        protected IReadOnlyList<Star> CreateNewAffectedStars => affectedStars;
        private float MaxEndTime => CreateNewAffectedStars.Count == 0
            ? Duration
            : CreateNewAffectedStars.Max(s => s.PulseDelay + Duration);
        public bool IsActive => TimeProgress < MaxEndTime;

        public EffectInstance(PointF center, IAreaShape area, float duration, Random rand)
        {
            Center = center;
            Area = area;
            Duration = duration;
            this.rand = rand;
            TimeProgress = 0f;
        }

        public virtual void ApplyTo(List<Star> stars)
        {
           affectedStars.Clear();
            foreach (var star in stars)
            {
                if (Area.Contains(star.Position))
                    affectedStars.Add(star);
            }
            OnApplyTo(affectedStars);
        }
        protected abstract void OnApplyTo(List<Star> stars);

        public void Update(float deltaTimeInSeconds)
        {
            if (!IsActive) return;
            TimeProgress += deltaTimeInSeconds;
            OnUpdate(TimeProgress / Duration);  // normalized time (0~1)
            
            if (TimeProgress >= MaxEndTime)
            {
                Reset(); // Reset affectedStars
            }
        }

        protected abstract void OnUpdate(float normalizedTime);
        protected abstract void Reset();
    }
}
