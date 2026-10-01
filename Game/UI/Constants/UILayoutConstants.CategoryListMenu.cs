/* ----- ----- ----- ----- */
// UILayoutConstants.CategoryListMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
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
        /// The category list submenus (<c>UICategoryListMenu</c>: 殘局闖關 <c>UIEndgameMenu</c>,
        /// 開局練習 <c>UIOpeningMenu</c>): a submenu panel (same place and size as
        /// <see cref="Submenu"/>) whose scroll container holds the category filter row and
        /// the item grid, both flex rows that wrap after <see cref="Columns"/> buttons.
        /// </summary>
        public class CategoryListMenu
        {
            /// <summary>Buttons per row, in both the category row and the item grid.</summary>
            public const int Columns = 4;

            /// <summary>Horizontal space between two buttons of a row.</summary>
            public const float ColumnGap = 20.0f;

            /// <summary>Vertical space between two rows of buttons.</summary>
            public const float RowGap = 20.0f;

            /// <summary>Vertical space between the category row and the item grid.</summary>
            public const float SectionGap = 40.0f;

            /// <summary>
            /// Width of every button: the scroll container's width split into
            /// <see cref="Columns"/> equal columns with <see cref="ColumnGap"/> between them
            /// ((680 - 3 * 20) / 4 = 155), minus one unit of slack. Pixel snapping can make the
            /// container a fraction of a unit narrower than designed; without the slack the
            /// fourth button would no longer fit and wrap onto the next line.
            /// </summary>
            public static readonly float ButtonWidth =
                MathF.Floor((Submenu.ScrollContainer.Size.X - ColumnGap * (Columns - 1)) / Columns) - 1.0f;

            /// <summary>An item button (puzzle, opening): up to three lines (name, possibly wrapped, and difficulty stars).</summary>
            public const float ItemButtonHeight = 100.0f;

            /// <summary>A category toggle button: one line.</summary>
            public const float CategoryButtonHeight = 56.0f;
        }
    }
}
