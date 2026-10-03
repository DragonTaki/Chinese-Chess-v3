/* ----- ----- ----- ----- */
// ConcentrateParameter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Mathematics;

namespace StarAnimation.Core.Effect.Parameter
{
    /// <summary>
    /// Declare configurable parameter for Concentrate effect instance (stars fly to a center, then scatter).
    /// </summary>
    public class ConcentrateParameter
    {
        public RangeF CountdownRange { get; set; }
        public float TriggerChance { get; set; }
        public float EffectAppliedChance { get; set; }
        public RangeF DurationRange { get; set; }

        /// <summary>Radius around the center within which stars are pulled in.</summary>
        public RangeF RadiusRange { get; set; }

        /// <summary>Acceleration toward (then away from) the center, at the center's distance falloff 1.</summary>
        public RangeF StrengthRange { get; set; }
    }
}
