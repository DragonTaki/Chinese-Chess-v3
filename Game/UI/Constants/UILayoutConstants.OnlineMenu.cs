/* ----- ----- ----- ----- */
// UILayoutConstants.OnlineMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
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
        /// Shared online block sizes and the room screen layout: a screen-sized menu
        /// whose panel is the left column the main menu and
        /// an open submenu take together, holding one scrolling column of headers, text lines,
        /// setting-like rows (name left, control right) and rows of buttons.
        /// All values are placeholders for the author to tune where the app can be seen.
        /// </summary>
        public static class OnlineMenu
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            // The main menu's panel and a submenu side by side.
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(0.0f, 0.0f),
                new Vector2F(MainMenu.Size.X + Submenu.Size.X, MainMenu.Size.Y));

            // Inset of the scroll container from the panel's edges (as a submenu's)
            public const float MarginX = Submenu.MarginX;
            public const float MarginY = Submenu.MarginY;

            /// <summary>
            /// Encapsulates OnlineMenu:ScrollContainer related setting values.
            /// </summary>
            public static class ScrollContainer
            {
                public static Vector2F Position => Layout.Position;
                public static Vector2F Size => Layout.Size;
                public static readonly LayoutF Layout = new LayoutF(
                    new Vector2F(MarginX, MarginY),
                    new Vector2F(OnlineMenu.Size.X - MarginX * 2, OnlineMenu.Size.Y - MarginY * 2));
            }

            /// <summary>Vertical space between two blocks of the column (as the settings rows').</summary>
            public const float RowGap = SettingsMenu.RowGap;

            /// <summary>Space between two text lines inside a block (a room's lines, the settings summary).</summary>
            public const float LineGap = 4.0f;

            /// <summary>Size of a button in a row of buttons (登入, 返回, a game kind, 準備…).</summary>
            public const float ButtonWidth = 300.0f, ButtonHeight = SettingsMenu.FooterButtonHeight;

            /// <summary>Horizontal and vertical space between the buttons of a row (it wraps).</summary>
            public const float ButtonGap = SettingsMenu.FooterColumnGap;

            /// <summary>Width of the account, password and room number fields.</summary>
            public const float FieldWidth = 420.0f;

            /// <summary>Size of a row's own button (a lobby room's 加入, the room number's 加入).</summary>
            public const float RowButtonWidth = 140.0f, RowButtonHeight = SettingsMenu.ValueHeight;

            /// <summary>Space between a row's control and its button.</summary>
            public const float RowButtonGap = 16.0f;
        }
    }
}
