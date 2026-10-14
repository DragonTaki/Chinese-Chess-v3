/* ----- ----- ----- ----- */
// UILayoutConstants.MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/12
// Version: v2.2
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.Mathematics;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// Encapsulates MainMenu related setting values.
        /// </summary>
        public static class MainMenu
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(0.0f, 0.0f),
                new Vector2F(360.0f, 840.0f));

            // Inset of the scroll container from the menu panel's edges
            public const float Margin = 40.0f;

            /// <summary>
            /// Encapsulates MainMenu:ScrollContainer related setting values.
            /// </summary>
            public static class ScrollContainer
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(Margin, Margin),
                    new Vector2F(MainMenu.Size.X - Margin * 2, MainMenu.Size.Y - Margin * 2));
            }

            /// <summary>
            /// Encapsulates MainMenu:ScrollContainer:Button related setting values.
            /// </summary>
            public static class Button
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(0.0f, 0.0f),  // The scroll container is already inset by the margin
                    new Vector2F(ScrollContainer.Size.X, 60.0f));

                /// <summary>
                /// Largest gap between two buttons (the actual gap is computed from the button count,
                /// <c>MenuSpacing.Gap</c>). 40 so the eight buttons fill the scroll container exactly
                /// without scrolling: 8 * 60 + 7 * 40 = 760 (840 - 2 * 40).
                /// </summary>
                public const float Spacing = 40.0f;

                /// <summary>
                /// Smallest gap between two buttons. The actual gap is computed from the number of
                /// buttons (<c>MenuSpacing.Gap</c>, at most <see cref="Spacing"/>) so that all of them
                /// fit the scroll container: ten buttons need (760 - 10 * 60) / 9 = 17.77.
                /// </summary>
                public const float MinSpacing = 8.0f;
            }
        }
    }
}
