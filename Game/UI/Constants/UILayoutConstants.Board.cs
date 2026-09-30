/* ----- ----- ----- ----- */
// UILayoutConstants.MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Boards;

using Engine.Geometry;
using Engine.Mathematics;

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
        public class Board
        {
            public static Vector2F Position => Layout.Position;
            public static Vector2F Size => Layout.Size;
            public static readonly LayoutF Layout = new LayoutF(
                new Vector2F(MainMenu.Size.X, MainMenu.Position.Y),
                new Vector2F(780.0f, MainMenu.Size.Y));

            // Space between the edge of the form and the Board
            public const float Margin = 60.0f;

            // Board details (line width, piece radius and font, marks, frame) scale with
            // the board by its grid scale, rounded to this step: a board a fraction of a
            // pixel off its authored size (pixel snapping) keeps its authored details
            // exactly, and scaled fonts/pens are only rebuilt when the step changes.
            public const float DetailScaleStep = 0.01f;

            /// <summary>
            /// Encapsulates Board:Grid related setting values.
            /// </summary>
            public class Grid
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
            /// Encapsulates Board:Piece related setting values: the sizes a piece is drawn
            /// at, at the board's authored size (scaled by <c>UIBoard.DetailScale</c>).
            /// Colors and the font face are in <see cref="PieceSettings"/>.
            /// </summary>
            public class Piece
            {
                // Radius of the filled circle
                public const float Radius = 35.0f;

                // Outline circle: inset from the edge by OuterMargin, drawn this wide
                public const float OuterMargin = 6.0f;
                public const float RedOutlineWidth = 3.0f;
                public const float BlackOutlineWidth = 2.0f;

                // Selection glow: extends this far past the radius
                public const float GlowMargin = 6.0f;

                // Label font size
                public const float FontSize = 30.0f;
            }
        }
    }
}
