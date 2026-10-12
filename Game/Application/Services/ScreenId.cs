/* ----- ----- ----- ----- */
// ScreenId.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/12
// Version: v1.1
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
        Game,
        /// <summary>多人連線's login screen (account, password, 記住帳密).</summary>
        Login,
        /// <summary>多人連線's menu: the game kinds, the modes, and the custom rooms' lobby as its pages.</summary>
        OnlineMenu,
        /// <summary>An online room: its seats, ready marks and settings (its game is shown from there).</summary>
        Room
    }
}
