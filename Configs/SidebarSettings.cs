/* ----- ----- ----- ----- */
// SidebarSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Chinese_Chess_v3.Configs
{
    public static class SidebarSettings
    {
        // Sidebar size
        public const int Width = 360;

        public const int Height = BoardConstants.TotalHeight;

        // Space between the edge of the form and the sidebar object
        public const int Margin = 20;

        // Logger size
        public const int LoggerWidth = Width - Margin * 2;
        public const int LoggerHeight = 200;


        // Origin point for the board
        public const int StartX = BoardConstants.TotalWidth;
        public const int StartY = 0;        
    }
}