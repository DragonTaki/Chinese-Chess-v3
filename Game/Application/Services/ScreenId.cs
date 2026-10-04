/* ----- ----- ----- ----- */
// ScreenId.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// The app's top-level screens that <see cref="INavigator"/> switches between. The main
    /// menu's submenus (開始新局, 讀取存檔, 殘局挑戰...) are not screens: they open inside the
    /// main menu.
    /// </summary>
    public enum ScreenId
    {
        /// <summary>The main menu (主畫面).</summary>
        MainMenu,
        /// <summary>The game screen: the board, its sidebar and the game menu on the left.</summary>
        Game
    }
}
