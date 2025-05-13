/* ----- ----- ----- ----- */
// BoardSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Configs.Board
{
    public static class BoardSettings
    {
        // Board frame origin point
        public const int BoardFrameStartX = 0;
        public const int BoardFrameStartY = 0;
        // Board origin point
        public const int BoardStartX = BoardFrameStartX + Margin;
        public const int BoardStartY = BoardFrameStartY + Margin;

        // Distance between pieces
        public const int GridSize = 80;

        // Space between the edge of the form and the board
        public const int Margin = 60;
        
        // Board line width
        public const int LineWidth = 2;
    }
}