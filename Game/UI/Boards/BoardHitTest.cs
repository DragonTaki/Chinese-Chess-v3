/* ----- ----- ----- ----- */
// BoardHitTest.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// Board hit testing derived from where the pieces are drawn, shared by
    /// <see cref="UIBoard.TryPixelToGrid"/> (the resolved board rectangle) and
    /// <see cref="BoardPixelExtensions"/> (the authored Full layout), so the two can never
    /// drift apart again. Pure math (no UI state), so it can be checked in isolation.
    /// <para>
    /// The geometry is what drawing uses: <c>firstCenter</c> is where the piece on square
    /// (0, 0) is drawn (<see cref="UIBoard.GridToPixel"/>: a line crossing on the Full board,
    /// a cell's centre on HalfCenter), <c>cellSize</c> the distance between neighbouring
    /// piece centres, <c>pieceRadius</c> the drawn piece radius.
    /// </para>
    /// <list type="bullet">
    /// <item>Clickable area: the drawn extent - from the outermost piece centres out by the
    /// piece radius on every side - moved outward per edge by <c>edgeAdjust</c> (positive
    /// extends the area, negative pulls it in).</item>
    /// <item>Target: the square whose piece centre is nearest (the inverse of
    /// <see cref="UIBoard.GridToPixel"/>, rounded), clamped onto the board for clicks in
    /// the margin outside the outermost centres.</item>
    /// </list>
    /// </summary>
    public static class BoardHitTest
    {
        /// <summary>
        /// Whether (<paramref name="pixelX"/>, <paramref name="pixelY"/>) is inside the
        /// clickable area of a <paramref name="columns"/> x <paramref name="rows"/> board.
        /// </summary>
        /// <param name="firstCenter">Where the piece on square (0, 0) is drawn.</param>
        /// <param name="cellSize">Distance between neighbouring piece centres.</param>
        /// <param name="columns">Number of columns (squares across).</param>
        /// <param name="rows">Number of rows (squares down).</param>
        /// <param name="pieceRadius">The drawn piece radius.</param>
        /// <param name="edgeAdjust">Per-edge adjustment, same units as the pixels; positive extends the area outward.</param>
        /// <param name="pixelX">Point X, same space as <paramref name="firstCenter"/>.</param>
        /// <param name="pixelY">Point Y, same space as <paramref name="firstCenter"/>.</param>
        /// <returns>False also for an empty board or a non-positive <paramref name="cellSize"/>.</returns>
        public static bool IsWithin(Vector2F firstCenter, float cellSize, int columns, int rows, float pieceRadius,
            PaddingF edgeAdjust, float pixelX, float pixelY)
        {
            if (cellSize <= 0f || columns <= 0 || rows <= 0)
                return false;

            float left = firstCenter.X - pieceRadius - edgeAdjust.Left;
            float top = firstCenter.Y - pieceRadius - edgeAdjust.Top;
            float right = firstCenter.X + (columns - 1) * cellSize + pieceRadius + edgeAdjust.Right;
            float bottom = firstCenter.Y + (rows - 1) * cellSize + pieceRadius + edgeAdjust.Bottom;
            return pixelX >= left && pixelX <= right && pixelY >= top && pixelY <= bottom;
        }

        /// <summary>
        /// The square whose drawn piece centre is nearest to (<paramref name="pixelX"/>,
        /// <paramref name="pixelY"/>), clamped onto the board; (0, 0) for an empty board or a
        /// non-positive <paramref name="cellSize"/>. Does not check the clickable area
        /// (<see cref="IsWithin"/>).
        /// </summary>
        /// <param name="firstCenter">Where the piece on square (0, 0) is drawn.</param>
        /// <param name="cellSize">Distance between neighbouring piece centres.</param>
        /// <param name="columns">Number of columns (squares across).</param>
        /// <param name="rows">Number of rows (squares down).</param>
        /// <param name="pixelX">Point X, same space as <paramref name="firstCenter"/>.</param>
        /// <param name="pixelY">Point Y, same space as <paramref name="firstCenter"/>.</param>
        /// <param name="gridX">The square's column.</param>
        /// <param name="gridY">The square's row.</param>
        public static void NearestSquare(Vector2F firstCenter, float cellSize, int columns, int rows,
            float pixelX, float pixelY, out int gridX, out int gridY)
        {
            gridX = gridY = 0;
            if (cellSize <= 0f || columns <= 0 || rows <= 0)
                return;

            // Floor(v + 0.5) rounds half up on both sides of 0 (a cast truncates toward 0).
            gridX = Math.Clamp((int)MathF.Floor((pixelX - firstCenter.X) / cellSize + 0.5f), 0, columns - 1);
            gridY = Math.Clamp((int)MathF.Floor((pixelY - firstCenter.Y) / cellSize + 0.5f), 0, rows - 1);
        }

        /// <summary>
        /// <see cref="IsWithin"/>, then <see cref="NearestSquare"/>: the square a click at
        /// (<paramref name="pixelX"/>, <paramref name="pixelY"/>) targets.
        /// </summary>
        /// <returns>False (and square (0, 0)) when the point is outside the clickable area.</returns>
        public static bool TryPixelToGrid(Vector2F firstCenter, float cellSize, int columns, int rows, float pieceRadius,
            PaddingF edgeAdjust, float pixelX, float pixelY, out int gridX, out int gridY)
        {
            gridX = gridY = 0;
            if (!IsWithin(firstCenter, cellSize, columns, rows, pieceRadius, edgeAdjust, pixelX, pixelY))
                return false;

            NearestSquare(firstCenter, cellSize, columns, rows, pixelX, pixelY, out gridX, out gridY);
            return true;
        }
    }
}
