/* ----- ----- ----- ----- */
// UILayoutConstants.Sidebar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.UI.Constants
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

            // Inset of the info board and logger box from the sidebar's edges (also the gap between them)
            public const float Margin = 20.0f;

            /// <summary>
            /// Encapsulates Sidebar:InfoBoard related setting values.
            /// </summary>
            public static class InfoBoard
            {
                // Location start point
                public static Vector2F Position => Layout.Position;
                // Size
                public static Vector2F Size => Layout.Size;
                // Relative to the sidebar, like every other position here (this used to be
                // Sidebar.Position + Margin, an absolute position the info board renderer
                // happened to draw at).
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(Margin, Margin),
                    new Vector2F(Sidebar.Size.X - Margin * 2.0f, 200.0f));
            }

            /// <summary>
            /// Encapsulates Sidebar:GameControls related setting values: the game controls
            /// (重新開始, 撤銷上步, 放棄對局...) between the info board and the logger box, two buttons
            /// per row. Placeholder sizes for the author to tune.
            /// </summary>
            public static class GameControls
            {
                // Buttons per row
                public const int Columns = 2;
                // Gap between buttons, both ways
                public const float Gap = 10.0f;
                // Button height
                public const float ButtonHeight = 50.0f;
                // Button width: the row's width shared by the columns
                public static readonly float ButtonWidth = (Sidebar.Size.X - Margin * 2.0f - Gap * (Columns - 1)) / Columns;
            }

            /// <summary>
            /// Encapsulates Sidebar:Logger related setting values.
            /// </summary>
            public static class LoggerBox
            {
                // Location start point
                public static Vector2F Position => Layout.Position;
                // Size
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(Sidebar.Margin, Sidebar.Size.Y - 200.0f - Sidebar.Margin),
                    new Vector2F(Sidebar.Size.X - Sidebar.Margin * 2.0f, 200.0f));

                // Inset of the scroll container from the logger box's edges
                public const float Margin = 8.0f;

                /// <summary>
                /// Encapsulates LoggerBox:ScrollContainer related setting values.
                /// </summary>
                public static class ScrollContainer
                {
                    public static Vector2F Position => Layout.Position;
                    public static Vector2F Size => Layout.Size;
                    public static readonly LayoutF Layout = new LayoutF(
                        new Vector2F(Margin, Margin),
                        new Vector2F(LoggerBox.Size.X - Margin * 2, LoggerBox.Size.Y - Margin * 2));
                }
            }

            // Color
            public static readonly Color BackgroundColor = StyleHelper.GetColor("#716c6cff");  // #716c6cff (the previous #0A0A0A is in the line below)
            //public static readonly Color BackgroundColor = StyleHelper.GetColor("#0A0A0A");  // #0A0A0A
        }
    }
}
