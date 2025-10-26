/* ----- ----- ----- ----- */
// UILayoutConstants.MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/25
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.Constants.UI
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// Encapsulates Sidebar related setting values.
        /// </summary>
        public static class Sidebar
        {
            // Location start point
            public static Vector2F Position => Layout.Position;
            // Size
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(Board.Position.X + Board.Size.X, MainMenu.Position.Y),
                new Vector2F(360.0f, Board.Size.Y));

            // Space between the edge of the form and the sidebar object
            public const float Margin = 20.0f;

            /// <summary>
            /// Encapsulates Sidebar:Infoboard related setting values.
            /// </summary>
            public class Infoboard
            {
                // Location start point
                public static Vector2F Position => Layout.Position;
                // Size
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    Sidebar.Position + Margin,
                    new Vector2F(Sidebar.Size.X - Margin * 2.0f, 200.0f));
            }

            /// <summary>
            /// Encapsulates Sidebar:Logger related setting values.
            /// </summary>
            public class LoggerBox
            {
                // Location start point
                public static Vector2F Position => Layout.Position;
                // Size
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(Sidebar.Margin, Sidebar.Size.Y - 200.0f - Sidebar.Margin),
                    new Vector2F(Sidebar.Size.X - Sidebar.Margin * 2.0f, 200.0f));

                // Space between the edge of the form and the MainMenu object
                public const float Margin = 8.0f;

                /// <summary>
                /// Encapsulates LoggerBox:ScrollContainer related setting values.
                /// </summary>
                public class ScrollContainer
                {
                    public static Vector2F Position => Layout.Position;
                    public static Vector2F Size => Layout.Size;
                    public static readonly LayoutF Layout = new LayoutF(
                        new Vector2F(Margin, Margin),
                        new Vector2F(LoggerBox.Size.X - Margin * 2, LoggerBox.Size.Y - Margin * 2));
                }
            }

            // Color
            public static readonly Color BackgroundColor = StyleHelper.GetColor("#716c6cff");  // #0A0A0A
            //public static readonly Color BackgroundColor = StyleHelper.GetColor("#0A0A0A");  // #0A0A0A
        }
    }
}
