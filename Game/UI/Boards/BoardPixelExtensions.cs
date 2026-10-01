/* ----- ----- ----- ----- */
// BoardPixelExtensions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/10/02
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Mathematics;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// Pixel/grid conversion for a <see cref="Board"/> at the authored layout
    /// (<c>UILayoutConstants.Board.Grid</c>). Moved here from <c>Board</c> so Game/Core
    /// does not depend on UI layout constants. The live hit test is
    /// <see cref="UIBoard.TryPixelToGrid"/>, which uses the resolved board rectangle
    /// instead; these keep the authored-layout variant available. Both delegate to
    /// <see cref="BoardHitTest"/> (the drawing geometry), so they follow the same rules.
    /// Full board only: the HalfCenter board's geometry is only resolved by
    /// <see cref="UIBoard"/>.
    /// </summary>
    public static class BoardPixelExtensions
    {
        /// <summary>Where the piece on square (0, 0) is drawn at the authored Full layout: grid point (0, 0).</summary>
        private static Vector2F FirstCenter => UILayoutConstants.Board.Grid.Position;

        /// <summary>
        /// Determine whether the pixel coordinates are within the board's clickable area: the
        /// drawn extent (the outermost grid points out by the authored piece radius on every
        /// side), adjusted per edge by <c>UILayoutConstants.Board.ClickArea</c>
        /// (<see cref="BoardHitTest.IsWithin"/>).
        /// </summary>
        public static bool IsWithinBoard(this Board board, float x, float y) =>
            BoardHitTest.IsWithin(FirstCenter, UILayoutConstants.Board.Grid.CellSize, board.Columns, board.Rows,
                UILayoutConstants.Board.Piece.Radius, UILayoutConstants.Board.ClickArea.EdgeAdjust, x, y);

        /// <summary>
        /// Convert pixel coordinates within the board area to board coordinates: the nearest
        /// grid point, clamped onto the board (<see cref="BoardHitTest.NearestSquare"/>).
        /// </summary>
        /// <param name="board">The board whose Columns/Rows bound the result</param>
        /// <param name="pixelX">Mouse X coordinate</param>
        /// <param name="pixelY">Mouse Y coordinate</param>
        /// <param name="gridX">Grid coordinate X</param>
        /// <param name="gridY">Grid coordinate Y</param>
        public static void PixelToGrid(this Board board, float pixelX, float pixelY, out int gridX, out int gridY) =>
            BoardHitTest.NearestSquare(FirstCenter, UILayoutConstants.Board.Grid.CellSize, board.Columns, board.Rows,
                pixelX, pixelY, out gridX, out gridY);
    }
}
