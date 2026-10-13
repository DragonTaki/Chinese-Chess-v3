/* ----- ----- ----- ----- */
// SetupLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    /// <summary>
    /// Vertical split of the pre-game settings panel (author decision 22, 2026-10-13): the tab bar at the top and
    /// the button row at the bottom are fixed, and the scrolling settings area takes the height between them.
    /// All values are design units; the constants are placeholders for the author to tune.
    /// </summary>
    public static class SetupLayout
    {
        /// <summary>Height of the bottom row (the buttons' height).</summary>
        public const float RowHeight = 60.0f;

        /// <summary>Width of each bottom-row button.</summary>
        public const float ButtonWidth = 200.0f;

        /// <summary>Space between the scrolling area and the bottom row.</summary>
        public const float RowGap = 16.0f;

        /// <summary>Distance from the panel's top edge to the scrolling area's top edge.</summary>
        public static float ScrollTop(float marginY, float tabBarHeight, float tabBarGap) => marginY + tabBarHeight + tabBarGap;

        /// <summary>Distance from the panel's bottom edge to the scrolling area's bottom edge.</summary>
        public static float ScrollBottom(float marginY) => marginY + RowHeight + RowGap;

        /// <summary>Height left for the scrolling area (never negative).</summary>
        public static float ScrollHeight(float panelHeight, float marginY, float tabBarHeight, float tabBarGap) =>
            System.Math.Max(0f, panelHeight - ScrollTop(marginY, tabBarHeight, tabBarGap) - ScrollBottom(marginY));

        /// <summary>Distance from the panel's top edge to the bottom row's top edge.</summary>
        public static float RowTop(float panelHeight, float marginY) => panelHeight - marginY - RowHeight;
    }
}
