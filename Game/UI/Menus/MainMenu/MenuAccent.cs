/* ----- ----- ----- ----- */
// MenuAccent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    /// <summary>
    /// The main menu buttons' colours (author §8.40): a gradient from blue through yellow to red,
    /// evenly spaced over the buttons (hue interpolated, so it goes blue, cyan, green, yellow,
    /// orange, red). A submenu uses the colour of the button that opened it.
    /// </summary>
    public static class MenuAccent
    {
        /// <summary>Hue (degrees) of the first button (blue).</summary>
        public const float BlueHue = 220f;

        /// <summary>Hue (degrees) of the middle of the gradient (yellow).</summary>
        public const float YellowHue = 55f;

        /// <summary>Hue (degrees) of the last button (red).</summary>
        public const float RedHue = 0f;

        private const float Saturation = 0.78f;
        private const float Value = 0.9f;

        /// <summary>The colours of <paramref name="count"/> buttons, first blue and last red.</summary>
        /// <param name="count">Number of buttons; 0 or less gives an empty array, 1 gives blue.</param>
        /// <returns>One colour per button, in button order.</returns>
        public static Color[] Gradient(int count)
        {
            if (count <= 0)
                return Array.Empty<Color>();

            var colors = new Color[count];
            for (int i = 0; i < count; i++)
                colors[i] = At(count == 1 ? 0f : i / (float)(count - 1));
            return colors;
        }

        /// <summary>The colour at <paramref name="t"/> along the gradient.</summary>
        /// <param name="t">0 is blue, 0.5 yellow, 1 red (clamped to that range).</param>
        /// <returns>The colour there.</returns>
        public static Color At(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            float hue = t <= 0.5f
                ? BlueHue + (YellowHue - BlueHue) * (t / 0.5f)
                : YellowHue + (RedHue - YellowHue) * ((t - 0.5f) / 0.5f);
            return FromHsv(hue, Saturation, Value);
        }

        /// <summary>An opaque colour from hue (degrees), saturation and value (0 to 1).</summary>
        private static Color FromHsv(float hue, float saturation, float value)
        {
            float c = value * saturation;
            float h = hue / 60f;
            float x = c * (1f - Math.Abs(h % 2f - 1f));
            float m = value - c;

            (float r, float g, float b) = (int)h switch
            {
                0 => (c, x, 0f),
                1 => (x, c, 0f),
                2 => (0f, c, x),
                3 => (0f, x, c),
                4 => (x, 0f, c),
                _ => (c, 0f, x),
            };

            return Color.FromArgb(255, ToByte(r + m), ToByte(g + m), ToByte(b + m));
        }

        private static int ToByte(float channel) => Math.Clamp((int)MathF.Round(channel * 255f), 0, 255);
    }
}
