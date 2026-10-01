/* ----- ----- ----- ----- */
// UILayoutConstants.LoadSavedGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// The main menu's saved-game list (<c>UILoadSavedGameMenu</c>, 讀取存檔): a submenu
        /// panel (same place and size as <see cref="Submenu"/>) whose scroll container stacks,
        /// per category, a header and one full-width button per save. All values are
        /// placeholders for the author to tune where the app can be seen.
        /// </summary>
        public static class LoadSavedGameMenu
        {
            /// <summary>Height of a save's button (one line).</summary>
            public const float ItemHeight = 56.0f;

            /// <summary>Height of a category header.</summary>
            public const float HeaderHeight = 44.0f;

            /// <summary>Vertical space between two rows of the menu.</summary>
            public const float RowGap = 14.0f;
        }
    }
}
