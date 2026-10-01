/* ----- ----- ----- ----- */
// UILayoutConstants.SettingsMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// The settings submenu (<c>UISettingsMenu</c>, 遊戲設定 / 規則設定): a submenu panel
        /// (same place and size as <see cref="Submenu"/>) whose scroll container stacks the
        /// save / back row, then each section's header and one full-width button per setting.
        /// All values are placeholders for the author to tune where the app can be seen.
        /// </summary>
        public class SettingsMenu
        {
            /// <summary>Height of a setting's button (one line).</summary>
            public const float ItemHeight = 56.0f;

            /// <summary>Height of a section header.</summary>
            public const float HeaderHeight = 44.0f;

            /// <summary>Vertical space between two rows of the menu.</summary>
            public const float RowGap = 14.0f;

            /// <summary>Horizontal space between the save and back buttons.</summary>
            public const float FooterColumnGap = 20.0f;

            /// <summary>Height of the save / back buttons.</summary>
            public const float FooterButtonHeight = 64.0f;

            /// <summary>
            /// Width of the save and back buttons: the scroll container's width split in two
            /// with <see cref="FooterColumnGap"/> between, minus one unit of slack (see
            /// <see cref="CategoryListMenu.ButtonWidth"/> for why).
            /// </summary>
            public static readonly float FooterButtonWidth =
                MathF.Floor((Submenu.ScrollContainer.Size.X - FooterColumnGap) / 2.0f) - 1.0f;
        }
    }
}
