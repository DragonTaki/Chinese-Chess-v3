/* ----- ----- ----- ----- */
// UILayoutConstants.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

using SharedLib.Geometry;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Constants
{
    public static class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----
        
        /// <summary>
        /// Encapsulates menu related setting values.
        /// </summary>
        public class MainMenu
        {
            public static Vector2F Position => layout.Position;
            public static Vector2F Size => layout.Size;
            private static readonly LayoutF layout = new LayoutF(
                new Vector2F(0.0f, 0.0f),
                new Vector2F(360.0f, 840.0f));

            // Space between the edge of the form and the MainMenu object
            public const float Margin = 40.0f;

            /// <summary>
            /// Encapsulates MainMenu:ScrollContainer related setting values.
            /// </summary>
            public class ScrollContainer
            {
                public static Vector2F Position => layout.Position;
                public static Vector2F Size => layout.Size;
                private static readonly LayoutF layout = new LayoutF(
                    new Vector2F(Margin, Margin),
                    new Vector2F(MainMenu.Size.X - Margin * 2, MainMenu.Size.Y - Margin * 2));
            }

            /// <summary>
            /// Encapsulates MainMenu:ScrollContainer:Button related setting values.
            /// </summary>
            public class Button
            {
                public static Vector2F Position => layout.Position;
                public static Vector2F Size => layout.Size;
                private static readonly LayoutF layout = new LayoutF(
                    new Vector2F(0.0f, Margin),
                    new Vector2F(ScrollContainer.Size.X, 60.0f));
            }
        }

        /// <summary>
        /// Encapsulates SecondMenu related setting values.
        /// </summary>
        public class SecondMenu
        {
            public static Vector2F Position => layout.Position;
            public static Vector2F Size => layout.Size;
            private static readonly LayoutF layout = new LayoutF(
                new Vector2F(MainMenu.Size.X, MainMenu.Position.Y),
                new Vector2F(840.0f, MainMenu.Size.Y));

            // Space between the edge of the form and the MainMenu object
            public const float MarginX = 80.0f;
            public const float MarginY = 40.0f;

            /// <summary>
            /// Encapsulates SecondMenu:ScrollContainer related setting values.
            /// </summary>
            public class ScrollContainer
            {
                public static Vector2F Position => layout.Position;
                public static Vector2F Size => layout.Size;
                private static readonly LayoutF layout = new LayoutF(
                    new Vector2F(MarginX, MarginY),
                    new Vector2F(SecondMenu.Size.X - MarginX * 2, SecondMenu.Size.Y - MarginY * 2));
            }

            /// <summary>
            /// Encapsulates SecondMenu:ScrollContainer:Button related setting values.
            /// </summary>
            public class Button
            {
                public static Vector2F Position => layout.Position;
                public static Vector2F Size => layout.Size;
                private static readonly LayoutF layout = new LayoutF(
                    new Vector2F(0.0f, MarginY),
                    new Vector2F(ScrollContainer.Size.X, 60.0f));
            }
        }

        /// <summary>
            /// Encapsulates Board related setting values.
            /// </summary>
            public class Board
            {
                public static Vector2F Position => layout.Position;
                public static Vector2F Size => layout.Size;
                private static readonly LayoutF layout = new LayoutF(
                    new Vector2F(MainMenu.Size.X, MainMenu.Position.Y),
                    new Vector2F(840.0f, MainMenu.Size.Y));

                // Space between the edge of the form and the Board
                public const float Margin = 60.0f;

                /// <summary>
                /// Encapsulates Board:Grid related setting values.
                /// </summary>
                public class Grid
                {
                    // Location start point
                    public static readonly Vector2F Position = Board.Position + Margin;

                    // Distance between pieces
                    public const float Size = 80.0f;

                    // Board line width
                    public const float LineWidth = 2.0f;
                }
            }

        /// <summary>
        /// Encapsulates Sidebar related setting values.
        /// </summary>
        public static class Sidebar
        {
            // Location start point
            public static readonly Vector2F Position = new Vector2F(Board.Size.X, MainMenu.Position.Y);
            // Size
            public static readonly Vector2F Size = new Vector2F(360.0f, Board.Size.Y);

            // Space between the edge of the form and the sidebar object
            public const float Margin = 20.0f;

            /// <summary>
            /// Encapsulates Sidebar:Infoboard related setting values.
            /// </summary>
            public class Infoboard
            {
                // Location start point
                public static readonly Vector2F Position = Sidebar.Position + Margin;
                // Size
                public static readonly Vector2F Size = new Vector2F(Sidebar.Size.X - Margin * 2.0f, 160.0f);
            }

            /// <summary>
            /// Encapsulates Sidebar:Logger related setting values.
            /// </summary>
            public class Logger
            {
                // Location start point
                public static readonly Vector2F Position = new Vector2F(
                    Sidebar.Position.X + Margin,
                    Infoboard.Position.Y + Infoboard.Size.Y + Margin * 2.0f);
                // Size
                public static readonly Vector2F Size = new Vector2F(Sidebar.Size.X - Margin * 2.0f, 200.0f);
            }

            // Color
            public static readonly Color BackgroundColor = StyleHelper.GetColor("#0A0A0A");  // #0A0A0A
        }
    }
}