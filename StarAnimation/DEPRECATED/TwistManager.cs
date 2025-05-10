/* ----- ----- ----- ----- */
// TwistManager.cs
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
    /// Manages multiple TwistInstance objects for concurrent spiral effects.
    /// </summary>
    public class TwistManager
    {
        private readonly List<TwistInstance> activeTwists = new();
        public float EffectAppliedChance { get; set; } = 0.8f;

        public TwistParameterRange ParameterRange { get; set; } = new();

        /// <summary>
        /// Applies a new twist effect in a specified area.
        /// </summary>
        public void Apply(List<Star> stars, IAreaShape area, Random rand)
        {
            if (rand.NextDouble() > EffectAppliedChance)
                return;

            RectangleF bounds = area.BoundingBox;
            PointF center = new();
            int attempts = 100;
            do
            {
                float x = MathUtil.GetRandomFloat(bounds.Left, bounds.Right, rand);
                float y = MathUtil.GetRandomFloat(bounds.Top, bounds.Bottom, rand);
                center = new PointF(x, y);
            } while (!area.Contains(center) && --attempts > 0);

            TwistInstance instance = ParameterRange.CreateRandomInstance(center, area, rand);
            activeTwists.Add(instance);
        }

        /// <summary>
        /// Updates all active twist effects, removing completed ones.
        /// </summary>
        public void Update(List<Star> stars, float deltaTimeInSeconds)
        {
            for (int i = activeTwists.Count - 1; i >= 0; i--)
            {
                var twist = activeTwists[i];
                twist.Update(deltaTimeInSeconds);
                if (!twist.IsActive)
                    activeTwists.RemoveAt(i);
            }
        }
    }
}
