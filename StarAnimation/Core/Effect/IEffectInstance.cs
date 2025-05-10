/* ----- ----- ----- ----- */
// IEffectInstance.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace StarAnimation.Core.Effect
{
    public interface IEffectInstance
    {
        bool IsActive { get; }
        void Update(float deltaTimeInSeconds);
        void ApplyTo(List<Star> stars);
    }
}