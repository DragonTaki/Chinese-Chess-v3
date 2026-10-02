/* ----- ----- ----- ----- */
// UILayoutConstants.Board.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/02
// Version: v2.3
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Boards;

using Engine.Geometry;
using Engine.Mathematics;
using Engine.UI.Constants.Core;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static partial class UILayoutConstants
    {
        // ----- ----- ----- -----
        // Notice: Position is relative to its parent, not abs position
        // ----- ----- ----- -----

        /// <summary>
        /// Encapsulates Board related setting values.
        /// </summary>
        public static class Board
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(MainMenu.Size.X, MainMenu.Position.Y),
                new Vector2F(780.0f, MainMenu.Size.Y));

            // Margin around the board (currently unused: the layout rules place the board)
            public const float Margin = 60.0f;

            // Board details (line width, piece radius and font, marks, frame) scale with
            // the board by its grid scale, rounded to this step: a board a fraction of a
            // pixel off its authored size (pixel snapping) keeps its authored details
            // exactly, and scaled fonts/pens are only rebuilt when the step changes.
            public const float DetailScaleStep = 0.01f;

            /// <summary>
            /// Encapsulates Board:Grid related setting values.
            /// </summary>
            public static class Grid
            {
                // Location start point
                /// <summary>
                /// Returns the top-left position of the grid, centered inside the board.
                /// </summary>
                public static Vector2F Position
                {
                    get
                    {
                        float offsetX = (Layout.Size.X - GridAreaSize.X) / 2.0f;
                        float offsetY = (Layout.Size.Y - GridAreaSize.Y) / 2.0f;
                        return Layout.Position + new Vector2F(offsetX, offsetY);
                    }
                }

                // Distance between pieces
                public const float CellSize = 80.0f;

                // Board line width
                public const float LineWidth = 2.0f;

                // Cannon/soldier position marks ("L" shapes): arm length, and the gap
                // between each mark and its grid intersection
                public const float MarkLength = 6.0f;
                public const float MarkGap = 4.0f;

                /// <summary>
                /// The pixel size of the grid area calculated from board constants.
                /// </summary>
                public static readonly Vector2F GridAreaSize = new Vector2F(
                    (BoardConstants.Full.Columns - 1) * CellSize,
                    (BoardConstants.Full.Rows - 1) * CellSize
                );
            }

            /// <summary>
            /// Encapsulates Board:ClickArea related setting values: the Full board's clickable
            /// area (<c>UIBoard.TryPixelToGrid</c>, <c>BoardHitTest</c>) is the drawn extent -
            /// the outermost piece centres (grid crossings) out by the drawn piece radius - moved
            /// per edge by these, in design-space units at any board size (not scaled with the
            /// board). Positive extends the area outward, negative pulls it in. 0 = exactly the
            /// drawn extent; for the author to tune. The edges are screen edges: they stay put
            /// when the board is drawn rotated (<c>UIBoard.IsFlipped</c>).
            /// </summary>
            public static class ClickArea
            {
                public const float Left = 0.0f;
                public const float Top = 0.0f;
                public const float Right = 0.0f;
                public const float Bottom = 0.0f;

                /// <summary>The four edges together.</summary>
                public static readonly PaddingF EdgeAdjust = new PaddingF(Left, Top, Right, Bottom);
            }

            /// <summary>
            /// The HalfCenter board (台灣暗棋半盤, 8×4): pieces stand in the cells, not on the line
            /// crossings, so the grid area is Columns × Rows whole cells. Placeholder values for
            /// the author to tune. The board element takes this size's aspect ratio
            /// (<c>UILayoutSheet.GameScreen.HalfCenterBoard</c>); pieces and their rings use the
            /// <see cref="Piece"/> sizes, so <see cref="Grid.CellSize"/> must leave room for a
            /// ring (2 × (Radius + GlowMargin)).
            /// </summary>
            public static class HalfCenter
            {
                // Authored size of the board element (its aspect ratio): the grid plus a margin.
                public static readonly Vector2F Size = new Vector2F(780.0f, 420.0f);

                /// <summary>Encapsulates Board:HalfCenter:Grid related setting values.</summary>
                public static class Grid
                {
                    // Width (and height) of one cell
                    public const float CellSize = 90.0f;

                    /// <summary>The pixel size of the grid area: every cell, at the authored size.</summary>
                    public static readonly Vector2F GridAreaSize = new Vector2F(
                        BoardConstants.HalfCenter.Columns * CellSize,
                        BoardConstants.HalfCenter.Rows * CellSize
                    );
                }

                /// <summary>
                /// Encapsulates Board:HalfCenter:ClickArea related setting values: as
                /// <see cref="Board.ClickArea"/>, for the HalfCenter board (the outermost piece
                /// centres are the outer cells' centres). 0 = exactly the drawn extent; for the
                /// author to tune.
                /// </summary>
                public static class ClickArea
                {
                    public const float Left = 0.0f;
                    public const float Top = 0.0f;
                    public const float Right = 0.0f;
                    public const float Bottom = 0.0f;

                    /// <summary>The four edges together.</summary>
                    public static readonly PaddingF EdgeAdjust = new PaddingF(Left, Top, Right, Bottom);
                }
            }

            /// <summary>
            /// Encapsulates Board:Piece related setting values: the sizes a piece is drawn
            /// at, at the board's authored size (scaled by <c>UIBoard.DetailScale</c>).
            /// Colors and the font face are in <see cref="PieceSettings"/>.
            /// </summary>
            public static class Piece
            {
                // Radius of the filled circle
                public const float Radius = 35.0f;

                // Outline circle: inset from the edge by OuterMargin, drawn this wide
                public const float OuterMargin = 6.0f;
                public const float RedOutlineWidth = 3.0f;
                public const float BlackOutlineWidth = 2.0f;
                // Face-down piece's outline (placeholder, for the author to tune)
                public const float FaceDownOutlineWidth = 2.0f;

                // Selection glow ring: extends this far past the radius. The board hint
                // rings (legal moves, hanging pieces) use the same ring.
                public const float GlowMargin = 6.0f;

                // Label font size
                public const float FontSize = 30.0f;
            }
        }
    }
}
