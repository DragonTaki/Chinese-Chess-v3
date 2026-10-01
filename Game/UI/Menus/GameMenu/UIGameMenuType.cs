/* ----- ----- ----- ----- */
// UIGameMenuType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    public enum UIGameMenuType
    {
        Default,
        Restart,      // 重新開始 (Restart)
        Undo,         // 撤銷上步 (Undo the last round)
        SaveGame,     // 儲存遊戲 (Save game)
        LoadLayout,   // 載入佈局 (Load layout: the saved-game list)
        Surrender,    // 放棄對局 (Resign)
        ReturnToMain  // 回到主畫面 (Return to the main menu)
    }
}
