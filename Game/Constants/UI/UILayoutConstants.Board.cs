/* ----- ----- ----- ----- */
// UILayoutConstants.MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/25
// Version: v2.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Constants.Game;

using Engine.Geometry;
using Engine.Mathematics;

namespace Chinese_Chess_v3.Game.Constants.UI
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

                /// <summary>
                /// The pixel size of the grid area calculated from board constants.
                /// </summary>
                public static readonly Vector2F GridAreaSize = new Vector2F(
                    (BoardConstants.Full.Columns - 1) * CellSize,
                    (BoardConstants.Full.Rows - 1) * CellSize
                );
            }
        }
    }
}
