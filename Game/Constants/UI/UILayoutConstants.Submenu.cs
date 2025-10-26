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

namespace Chinese_Chess_v3.Game.Constants.UI
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// Encapsulates Submenu related setting values.
        /// </summary>
        public class Submenu
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(MainMenu.Size.X, MainMenu.Position.Y),
                new Vector2F(840.0f, MainMenu.Size.Y));

            // Space between the edge of the form and the MainMenu object
            public const float MarginX = 80.0f;
            public const float MarginY = 40.0f;

            /// <summary>
            /// Encapsulates Submenu:ScrollContainer related setting values.
            /// </summary>
            public class ScrollContainer
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(MarginX, MarginY),
                    new Vector2F(Submenu.Size.X - MarginX * 2, Submenu.Size.Y - MarginY * 2));
            }

            /// <summary>
            /// Encapsulates Submenu:ScrollContainer:Button related setting values.
            /// </summary>
            public class Button
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(0.0f, MarginY),
                    new Vector2F(ScrollContainer.Size.X, 60.0f));
                public const float Spacing = 40.0f;
            }
        }
    }
}
