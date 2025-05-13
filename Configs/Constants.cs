/* ----- ----- ----- ----- */
// Constants.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/13
// Update Date: 2025/05/13
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Configs
{
    public static class Constants
    {
        public static class Board
        {
            /* ----- Defining the board's grid size ----- */
            public const int Columns = 9;
            public const int Rows = 10;

            /* ----- Defining the board's X-axis and Y-axis range ----- */
            public static readonly (int MinX, int MaxX) XRange = (0, Columns - 1);
            public static readonly (int MinY, int MaxY) YRange = (0, Rows - 1);
            public static bool IsInBounds(int x, int y)
            {
                return x >= XRange.MinX && x <= XRange.MaxX &&
                    y >= YRange.MinY && y <= YRange.MaxY;
            }

            /* ----- In palace: General, Advisor ----- */
            // Both side palace area 3 <= X <= 5
            public static readonly (int MinX, int MaxX) PalaceXRange = (3, 5);
            // Red palace area (3, 7) to (5, 9)
            public static readonly (int MinY, int MaxY) RedPalaceYRange = (7, 9);

            // Black palace area (3, 0) to (5, 2)
            public static readonly (int MinY, int MaxY) BlackPalaceYRange = (0, 2);

            /* ----- In own side OR crossed river ----- */
            public const int RedYSideRiverLine = 5;    // Red side (Y >= 5)
            public const int BlackYSideRiverLine = 4;  // Black side (Y <= 4)
        }
    }
}