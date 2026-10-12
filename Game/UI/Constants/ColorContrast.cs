/* ----- ----- ----- ----- */
// ColorContrast.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// WCAG 2 contrast between two colours, to check that a text colour is readable on its background.
    /// </summary>
    public static class ColorContrast
    {
        /// <summary>The smallest ratio WCAG AA accepts for body text.</summary>
        public const double BodyTextMinimum = 4.5;

        /// <summary>The relative luminance of <paramref name="color"/> (0 black to 1 white; alpha is ignored).</summary>
        public static double Luminance(Color color) =>
            0.2126 * Linear(color.R) + 0.7152 * Linear(color.G) + 0.0722 * Linear(color.B);

        /// <summary>The contrast ratio of two colours, 1 (identical) to 21 (black on white); the order does not matter.</summary>
        public static double Ratio(Color a, Color b)
        {
            double la = Luminance(a), lb = Luminance(b);
            return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
        }

        private static double Linear(byte channel)
        {
            double c = channel / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }
    }
}
