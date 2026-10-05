/* ----- ----- ----- ----- */
// IGameControlCommands.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game screen's game controls: commands that act on the game being played (撤銷,
    /// 放棄, 重新開始; later 暫停 / 求和). Kept apart from <see cref="IGameNavigationCommands"/>
    /// because the author wants the two groups separated on screen (LAYER-SPLIT §6); the
    /// screen does not show them apart yet.
    /// </summary>
    public interface IGameControlCommands
    {
        /// <summary>
        /// 撤銷: takes back one round (both sides' last move); a game-log line when there is none.
        /// </summary>
        void UndoRound();

        /// <summary>
        /// 放棄: the side to move resigns now (clocks stop); a game-log line either way.
        /// </summary>
        void ResignSideToMove();

        /// <summary>
        /// 重新開始: restarts the current game in its current mode; asks first while a move has
        /// been made and the game is not over.
        /// </summary>
        void Restart();
    }
}
