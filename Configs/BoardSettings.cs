/* ----- ----- ----- ----- */
// BoardSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Configs
{
    public static class BoardSettings
    {
        // Distance between pieces
        public const int GridSize = 80;

        // Space between the edge of the form and the board
        public const int Margin = 60;

        // Origin point for the board
        public const int StartX = Margin;
        public const int StartY = Margin;
        
        // Board line width
        public const int LineWidth = 2;
    }
}