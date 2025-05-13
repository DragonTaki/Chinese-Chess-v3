/* ----- ----- ----- ----- */
// Effect.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace StarAnimation.Models
{
    /// <summary>
    /// Represents a local visual effect applied to a specific area of the field.
    /// </summary>
    public class Effect
    {
        public RectangleF Area { get; set; }
        public float TimeLeft { get; set; }
        public string Type { get; set; }
        public float Strength { get; set; }
        public Star AffectedStar { get; set; }
        public Color? TargetColor { get; set; }

        /// <summary>
        /// Initializes a new local effect with a specified area, duration, type, strength, and optional target color.
        /// </summary>
        /// <param name="area">The area within which the effect will be applied.</param>
        /// <param name="duration">The duration for which the effect will last, in seconds.</param>
        /// <param name="type">The type of effect ("twist", "pulse", or "colorShift").</param>
        /// <param name="strength">The strength of the effect (affects intensity).</param>
        /// <param name="color">Optional target color for the effect (only used for "colorShift").</param>
        public Effect(RectangleF area, float duration, string type, float strength, Star star, Color? color = null)
        {
            Area = area;
            TimeLeft = duration;
            Type = type;
            Strength = strength;
            AffectedStar = star;
            TargetColor = color;
        }
    }
}