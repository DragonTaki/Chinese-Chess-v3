/* ----- ----- ----- ----- */
// UIMainMenuType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public enum UIMainMenuType
    {
        Default,
        NewGame,           // 開新一局 (New game)
        LoadGame,          // 讀取存檔 (Load saved game)
        EndgameChallenge,  // 殘局闖關 (Endgame challenge)
        OpeningPractice,   // 開局練習 (Opening practice)
        RuleSettings,      // 規則設定 (Rule settings)
        Multiplayer,       // 多人連線 (Multiplayer)
        Help,              // 教學／幫助 (Tutorial / help)
        Settings,          // 遊戲設定 (Game settings)
        Exit               // 離開遊戲 (Exit game)
    }
}
