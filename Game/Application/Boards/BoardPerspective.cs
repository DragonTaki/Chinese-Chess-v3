/* ----- ----- ----- ----- */
// BoardPerspective.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;

namespace Chinese_Chess_v3.Game.Application.Boards
{
    /// <summary>
    /// Which way up the board is drawn: the player's own side (己方,
    /// <see cref="GameManager.LocalSide"/>) is always at the bottom. Core coordinates are
    /// absolute FEN coordinates (red at the bottom, high y), so on the Full board the view is
    /// rotated 180 degrees when 己方 plays black. Half boards are never rotated. Pure (reads
    /// Core only, no UI state), so it can be checked in isolation. Works in squares (grid
    /// coordinates); the board view (<c>UIBoard</c>) converts to and from pixels and uses it
    /// for both drawing (<c>UIBoard.GridToPixel</c>) and clicking (<c>UIBoard.TryPixelToGrid</c>).
    /// </summary>
    public static class BoardPerspective
    {
        /// <summary>
        /// Whether a <paramref name="boardType"/> board is drawn rotated 180 degrees when 己方
        /// plays <paramref name="localColor"/>: the Full board with 己方 black.
        /// </summary>
        public static bool IsFlipped(BoardType boardType, PieceColor localColor) =>
            boardType == BoardType.Full && localColor == PieceColor.Black;

        /// <summary>
        /// Whether the game set up in <paramref name="gameManager"/> is drawn rotated
        /// (<see cref="IsFlipped(BoardType, PieceColor)"/> for its board and the colour of its
        /// <see cref="GameManager.LocalSide"/>). Read live, so it follows every new game.
        /// </summary>
        public static bool IsFlipped(GameManager gameManager) =>
            gameManager != null
            && IsFlipped(gameManager.Board.Type, gameManager.ColorOf(gameManager.LocalSide));

        /// <summary>
        /// Maps a square between board (Core) and view (screen) coordinates on a
        /// <paramref name="columns"/> x <paramref name="rows"/> board: unchanged, or rotated
        /// 180 degrees (x to columns-1-x, y to rows-1-y) when <paramref name="flipped"/>. The
        /// rotation is its own inverse, so the same call maps either way.
        /// </summary>
        public static (float x, float y) Map(float x, float y, int columns, int rows, bool flipped) =>
            flipped ? (columns - 1 - x, rows - 1 - y) : (x, y);

        /// <inheritdoc cref="Map(float, float, int, int, bool)"/>
        public static (int x, int y) Map(int x, int y, int columns, int rows, bool flipped) =>
            flipped ? (columns - 1 - x, rows - 1 - y) : (x, y);
    }
}
