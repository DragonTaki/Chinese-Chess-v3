/* ----- ----- ----- ----- */
// MenuFrameLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    /// <summary>
    /// Where the open submenu's frame goes next to the main menu's frame (author 2026-10-12): the
    /// horizontal distance between the two outlines is one number for every frame style, and the tail
    /// reaches that far over the main menu's outline. All values are design units (both frames are drawn
    /// under the same scale).
    /// </summary>
    public static class MenuFrameLayout
    {
        /// <summary>
        /// Distance between the main menu frame's right outline and the submenu frame's left outline.
        /// Both frames are inset by the same amount from their panels, which touch at the main panel's
        /// right edge, so it used to be twice the style's inset (20 to 34); 16 is two thirds of the
        /// default style's 24.5.
        /// </summary>
        public const float Gap = 16f;

        /// <summary>How far the tail's point goes past the main menu frame's right outline, over its glow and fill.</summary>
        public const float TailReach = 8f;

        /// <summary>The tail's length, from the submenu frame's left outline to its point.</summary>
        public const float TailDepth = Gap + TailReach;

        /// <summary>
        /// The left edge of the submenu's frame bounds: the frame's outline (that bounds moved in by
        /// <paramref name="inset"/>) ends up <see cref="Gap"/> right of the main menu frame's outline.
        /// </summary>
        /// <param name="mainPanelRight">The main menu panel's right edge.</param>
        /// <param name="inset">The frame style's inset (the same for both frames).</param>
        public static float FrameLeft(float mainPanelRight, float inset) => mainPanelRight - 2f * inset + Gap;
    }
}
