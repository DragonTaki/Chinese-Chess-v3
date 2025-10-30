/* ----- ----- ----- ----- */
// UILayoutConstants.MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/25
// Version: v2.0
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
        public class MainMenu
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(0.0f, 0.0f),
                new Vector2F(360.0f, 840.0f));

            // Space between the edge of the form and the MainMenu object
            public const float Margin = 40.0f;

            /// <summary>
            /// Encapsulates MainMenu:ScrollContainer related setting values.
            /// </summary>
            public class ScrollContainer
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
            public class Button
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(0.0f, Margin),
                    new Vector2F(ScrollContainer.Size.X, 60.0f));
                public const float Spacing = 40.0f;
            }
        }
    }
}
