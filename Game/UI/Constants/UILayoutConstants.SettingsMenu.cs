/* ----- ----- ----- ----- */
// UILayoutConstants.SettingsMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/02
// Version: v1.1
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
        /// The settings screens (<c>UISettingsMenu</c>: 遊戲設定, 單機規則設定): a submenu panel
        /// (same place and size as <see cref="Submenu"/>) with a tab bar at the top and, below it,
        /// a scroll container that stacks the save / back row, then each section's header and
        /// one row per setting (name left, control right).
        /// All values are placeholders for the author to tune where the app can be seen.
        /// </summary>
        public static class SettingsMenu
        {
            /// <summary>Height of the tab bar.</summary>
            public const float TabBarHeight = 52.0f;

            /// <summary>Horizontal space between two tabs.</summary>
            public const float TabGap = 8.0f;

            /// <summary>Vertical space between the tab bar and the scroll container.</summary>
            public const float TabBarGap = 16.0f;

            /// <summary>Height of the selected tab's indicator bar.</summary>
            public const float TabIndicatorHeight = 4.0f;

            /// <summary>Horizontal inset of the indicator bar from the tab's sides.</summary>
            public const float TabIndicatorInset = 12.0f;

            /// <summary>Height of a setting's row.</summary>
            public const float ItemHeight = 52.0f;

            /// <summary>Left and right padding inside a setting's row (the name's and the control's distance from the row's edges).</summary>
            public const float ItemPaddingX = 12.0f;

            /// <summary>Height of a section header.</summary>
            public const float HeaderHeight = 44.0f;

            /// <summary>Vertical space between two rows of the menu.</summary>
            public const float RowGap = 10.0f;

            /// <summary>Size of a row's switch.</summary>
            public const float ToggleWidth = 64.0f, ToggleHeight = 32.0f;

            /// <summary>Gap between a switch's knob and its track's edge.</summary>
            public const float ToggleKnobInset = 4.0f;

            /// <summary>The narrowest a choice's dropdown gets (the width the value buttons had).</summary>
            public const float ValueWidth = 260.0f;

            /// <summary>The widest a choice's dropdown gets (it widens to fit its longest choice, see <see cref="ValueTextPaddingX"/>).</summary>
            public const float ValueMaxWidth = 480.0f;

            /// <summary>Space left of a choice's text inside its dropdown (and between the longest text and the arrow).</summary>
            public const float ValueTextPaddingX = 24.0f;

            /// <summary>Width of a dropdown's ▼ arrow (its height is half of it).</summary>
            public const float DropdownArrowWidth = 14.0f;

            /// <summary>Space between a dropdown's arrow and its box's right edge.</summary>
            public const float DropdownArrowInset = 16.0f;

            /// <summary>Height of one option in an open dropdown list.</summary>
            public const float DropdownItemHeight = 40.0f;

            /// <summary>Most options an open dropdown list shows at once (more scroll).</summary>
            public const int DropdownMaxVisibleItems = 6;

            /// <summary>Space between a dropdown's box and its open list.</summary>
            public const float DropdownListGap = 4.0f;

            /// <summary>Space above the first and below the last visible option inside an open dropdown list.</summary>
            public const float DropdownListPadding = 6.0f;

            /// <summary>Width of an open dropdown list's scroll bar.</summary>
            public const float DropdownScrollBarWidth = 4.0f;

            /// <summary>Width of a number's slider (track, knob and the value's column).</summary>
            public const float SliderWidth = 380.0f;

            /// <summary>Height of a slider's track.</summary>
            public const float SliderTrackHeight = 6.0f;

            /// <summary>Diameter of a slider's knob.</summary>
            public const float SliderKnobDiameter = 24.0f;

            /// <summary>Width of the value's column at a slider's right (fits e.g. 「180 分鐘」).</summary>
            public const float SliderLabelWidth = 110.0f;

            /// <summary>Space between a slider's track and its value's column.</summary>
            public const float SliderLabelGap = 14.0f;

            /// <summary>Height of a row's dropdown, slider and text field.</summary>
            public const float ValueHeight = 40.0f;

            /// <summary>Width of a row's text field.</summary>
            public const float TextFieldWidth = 260.0f;

            /// <summary>Horizontal inset of the text inside a text field.</summary>
            public const float TextFieldInset = 10.0f;

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
