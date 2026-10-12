/* ----- ----- ----- ----- */
// MenuSpacing.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    /// <summary>
    /// The gap between the main menu buttons: as large as allowed while every button still fits
    /// the viewport, so the player sees the last one (離開遊戲) without scrolling.
    /// </summary>
    public static class MenuSpacing
    {
        /// <summary>The gap that makes <paramref name="count"/> buttons exactly fill <paramref name="viewportHeight"/>, within the limits.</summary>
        /// <param name="viewportHeight">Height of the scroll container.</param>
        /// <param name="count">Number of buttons.</param>
        /// <param name="buttonHeight">Height of one button.</param>
        /// <param name="maxGap">Largest gap (used when there is room to spare).</param>
        /// <param name="minGap">Smallest gap (the menu then scrolls if the buttons still do not fit).</param>
        /// <returns>The gap, rounded down to 0.01 so the buttons never overshoot the viewport.</returns>
        public static float Gap(float viewportHeight, int count, float buttonHeight, float maxGap, float minGap)
        {
            if (count <= 1)
                return maxGap;

            float fitting = MathF.Floor((viewportHeight - count * buttonHeight) / (count - 1) * 100f) / 100f;
            return Math.Clamp(fitting, Math.Min(minGap, maxGap), maxGap);
        }
    }
}
