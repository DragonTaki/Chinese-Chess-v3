/* ----- ----- ----- ----- */
// UIPalette.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// The colours of the submenus' labels and buttons (author 2026-10-12): softer than the main menu
    /// buttons' blue-yellow-red gradient but not washed out, all opaque, with light text. Every colour
    /// is defined here, once, by role; <see cref="UILayoutStyles"/> builds the styles from it. Text
    /// colours keep at least <see cref="ColorContrast.BodyTextMinimum"/> against the colour they sit on.
    /// </summary>
    public static class UIPalette
    {
        /// <summary>
        /// What labels are read against: the dark panel behind the submenus (an assumed colour, only used
        /// to check the label colours' contrast; nothing is drawn with it).
        /// </summary>
        public static readonly Color PanelBase = Color.FromArgb(255, 0x1E, 0x22, 0x30);

        #region Submenu button

        public static readonly Color ButtonFill = Color.FromArgb(255, 0x3F, 0x5F, 0x94);
        public static readonly Color ButtonHoverFill = Color.FromArgb(255, 0x4F, 0x70, 0xA8);
        public static readonly Color ButtonPressedFill = Color.FromArgb(255, 0x32, 0x49, 0x6F);
        public static readonly Color ButtonBorder = Color.FromArgb(255, 0x9D, 0xB4, 0xDC);
        public static readonly Color ButtonText = Color.FromArgb(255, 0xF8, 0xF9, 0xFD);
        public static readonly Color ButtonHoverText = Color.FromArgb(255, 0xFF, 0xFF, 0xFF);

        /// <summary>A button that does nothing now (a disabled button, an empty row, a category that is off).</summary>
        public static readonly Color ButtonDisabledFill = Color.FromArgb(255, 0x36, 0x3C, 0x4B);
        public static readonly Color ButtonDisabledBorder = Color.FromArgb(255, 0x6B, 0x73, 0x88);
        public static readonly Color ButtonDisabledText = Color.FromArgb(255, 0xB4, 0xB9, 0xC6);

        #endregion

        /// <summary>A muted slate role for the smaller account logout action.</summary>
        public static readonly Color LogoutFill = Color.FromArgb(255, 0x50, 0x57, 0x65);
        public static readonly Color LogoutBorder = Color.FromArgb(255, 0xA3, 0xA9, 0xB8);

        #region Submenu header and tabs

        /// <summary>A section header box (the screen's title, 房間列表, 座位…).</summary>
        public static readonly Color HeaderFill = Color.FromArgb(255, 0x6A, 0x5A, 0x2E);
        public static readonly Color HeaderBorder = Color.FromArgb(255, 0xE3, 0xC7, 0x77);
        public static readonly Color HeaderText = Color.FromArgb(255, 0xFF, 0xF4, 0xD6);

        /// <summary>A tab that is not selected (the selected one looks like a button).</summary>
        public static readonly Color TabFill = Color.FromArgb(255, 0x34, 0x3B, 0x4D);
        public static readonly Color TabBorder = Color.FromArgb(255, 0x6B, 0x73, 0x88);
        public static readonly Color TabText = Color.FromArgb(255, 0xD0, 0xD5, 0xE2);

        /// <summary>The bar under the selected tab.</summary>
        public static readonly Color TabIndicator = HeaderBorder;

        #endregion

        #region Labels (text drawn straight on the panel)

        /// <summary>A section heading such as <c>── 大盤 ──</c>, and a status line.</summary>
        public static readonly Color SectionHeadingText = Color.FromArgb(255, 0xF2, 0xD5, 0x8A);

        public static readonly Color LabelText = Color.FromArgb(255, 0xF3, 0xF1, 0xEA);

        /// <summary>Hints and a free seat.</summary>
        public static readonly Color MutedText = Color.FromArgb(255, 0xA3, 0xA9, 0xB8);

        /// <summary>An error: red text, never a red background.</summary>
        public static readonly Color ErrorText = Color.FromArgb(255, 0xFF, 0x8F, 0x85);

        public static readonly Color SuccessText = Color.FromArgb(255, 0x8F, 0xD9, 0xA0);

        #endregion
    }
}
