/* ----- ----- ----- ----- */
// GameMenuType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Menu
{
    public enum GameMenuType
    {
        Default,
        Restart,      // 重新開始
        Undo,         // 撤銷上步
        SaveGame,     // 儲存遊戲
        LoadLayout,   // 載入佈局
        Surrender,    // 放棄對局
        ReturnToMain  // 回到主畫面
    }
}
