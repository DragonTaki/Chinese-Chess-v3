/* ----- ----- ----- ----- */
// GameControlOption.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game controls (<see cref="GameControlOptions"/>): the buttons that act on the game being
    /// played, shown in the sidebar between the clock board and the log box (author 2026-10-05).
    /// </summary>
    public enum GameControlOption
    {
        Restart,  // 重新開始 (Restart)
        Undo,     // 撤銷上步 (Undo the last round)
        Resign,   // 放棄對局 (Resign; 三國: 棄權)
    }
}
