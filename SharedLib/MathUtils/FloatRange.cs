/* ----- ----- ----- ----- */
// FloatRange.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using SharedLib.RandomTable;

namespace SharedLib.MathUtils
{
    /// <summary>
    /// Represents a float range with inclusive minimum and maximum values.
    /// </summary>
    public class FloatRange
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns a random float within the range [Min, Max], using the provided Random.
        /// </summary>
        public float GetRandom()
        {
            return GlobalRandom.Instance.NextFloat(Min, Max);
        }
    }
}