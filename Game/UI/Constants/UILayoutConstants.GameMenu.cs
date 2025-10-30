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
        /// Encapsulates GameMenu related setting values.
        /// </summary>
        public class GameMenu
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = MainMenu.Layout;

            // Space between the edge of the form and the GameMenu object
            public const float Margin = MainMenu.Margin;

            /// <summary>
            /// Encapsulates GameMenu:ScrollContainer related setting values.
            /// </summary>
            public class ScrollContainer
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = MainMenu.ScrollContainer.Layout;
            }

            /// <summary>
            /// Encapsulates GameMenu:ScrollContainer:Button related setting values.
            /// </summary>
            public class Button
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = MainMenu.Button.Layout;
                public const float Spacing = 40.0f;
            }
        }
    }
}
