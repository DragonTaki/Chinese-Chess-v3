/* ----- ----- ----- ----- */
// IGameNavigationCommands.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game screen's navigation and file commands (儲存, 載入, 回到主畫面). Kept apart from
    /// <see cref="IGameControlCommands"/> because the author wants the two groups separated on
    /// screen (LAYER-SPLIT §6); the screen does not show them apart yet.
    /// </summary>
    public interface IGameNavigationCommands
    {
        /// <summary>
        /// 儲存: saves the game to the saves folder (the Core logs the file name); a game-log
        /// line with the reason when it cannot.
        /// </summary>
        void SaveGame();

        /// <summary>
        /// 載入: opens the saved-game list (asking first when the current game has unsaved
        /// changes); while it is open, closes it again.
        /// </summary>
        void LoadGame();

        /// <summary>
        /// 回到主畫面: while the game is in progress, asks whether to give it up first; an ended
        /// game, or one in which nobody has moved yet, goes back directly.
        /// </summary>
        void ReturnToMain();
    }
}
