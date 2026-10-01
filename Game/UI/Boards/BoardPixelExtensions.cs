/* ----- ----- ----- ----- */
// BoardPixelExtensions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.UI.Constants;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// Pixel/grid conversion for a <see cref="Board"/> at the authored layout
    /// (<c>UILayoutConstants.Board.Grid</c>). Moved here from <c>Board</c> so Game/Core
    /// does not depend on UI layout constants. The live hit test is
    /// <see cref="UIBoard.TryPixelToGrid"/>, which uses the resolved board rectangle
    /// instead; these keep the authored-layout variant available.
    /// </summary>
    public static class BoardPixelExtensions
    {
        /// <summary>
        /// Determine whether the pixel coordinates are within the board range
        /// </summary>
        public static bool IsWithinBoard(this Board board, float x, float y)
        {
            float boardX = UILayoutConstants.Board.Grid.Position.X;
            float boardY = UILayoutConstants.Board.Grid.Position.Y;
            float gridSize = UILayoutConstants.Board.Grid.CellSize;
            return x >= boardX && x <= boardX + board.Columns * gridSize &&
                   y >= boardY && y <= boardY + board.Rows * gridSize;
        }

        /// <summary>
        /// Convert pixel coordinates within the board area to board coordinates
        /// </summary>
        /// <param name="board">The board whose Columns/Rows bound the result</param>
        /// <param name="pixelX">Mouse X coordinate</param>
        /// <param name="pixelY">Mouse Y coordinate</param>
        /// <param name="gridX">Grid coordinate X</param>
        /// <param name="gridY">Grid coordinate Y</param>
        public static void PixelToGrid(this Board board, float pixelX, float pixelY, out int gridX, out int gridY)
        {
            gridX = (int)((pixelX - UILayoutConstants.Board.Grid.Position.X) / UILayoutConstants.Board.Grid.CellSize + 0.5f);
            gridY = (int)((pixelY - UILayoutConstants.Board.Grid.Position.Y) / UILayoutConstants.Board.Grid.CellSize + 0.5f);

            gridX = Math.Clamp(gridX, 0, board.Columns - 1);
            gridY = Math.Clamp(gridY, 0, board.Rows - 1);
        }
    }
}
