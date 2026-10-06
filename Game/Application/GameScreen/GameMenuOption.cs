/* ----- ----- ----- ----- */
// GameMenuOption.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>The options of the game screen's menu (<see cref="GameMenuOptions"/>; the game controls are <see cref="GameControlOption"/>).</summary>
    public enum GameMenuOption
    {
        Default,
        SaveGame,     // 儲存遊戲 (Save game)
        LoadLayout,   // 載入佈局 (Load layout: the saved-game list)
        ReturnToMain  // 回到主畫面 (Return to the main menu)
    }
}
