/* ----- ----- ----- ----- */
// MainMenuOption.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/13
// Version: v1.4
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>
    /// The options of the main menu (<see cref="MainMenuOptions"/>); most open a submenu inside
    /// the main menu (<see cref="MainMenuPresenter.IsSubmenu"/>).
    /// </summary>
    public enum MainMenuOption
    {
        Default,
        SinglePlayer,      // Single-player game
        VsComputer,        // Game against the computer
        LoadGame,          // 讀取存檔 (Load saved game)
        EndgameChallenge,  // 殘局闖關 (Endgame challenge)
        OpeningPractice,   // 開局練習 (Opening practice)
        Multiplayer,       // 多人連線 (Multiplayer)
        Help,              // 教學／幫助 (Tutorial / help)
        Settings,          // 遊戲設定 (Game settings)
        Exit               // 離開遊戲 (Exit game)
    }
}
