/* ----- ----- ----- ----- */
// SidebarSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using Chinese_Chess_v3.Configs.Board;
using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Configs.Sidebar
{
    public static class SidebarSettings
    {
        // Sidebar origin point
        public const int SidebarStartX = BoardConstants.BoardTotalWidth;
        public const int SidebarStartY = BoardSettings.BoardFrameStartY;

        // Sidebar size
        public const int SidebarWidth = 360;
        public const int SidebarHeight = BoardConstants.BoardTotalHeight;

        // Space between the edge of the form and the sidebar object
        public const int Margin = 20;

        // Info board origin point
        public const int InfoboardStartX = SidebarStartX + Margin;
        public const int InfoboardStartY = SidebarStartY + Margin;

        // Info board size
        public const int InfoboardWidth = SidebarWidth - Margin * 2;
        public const int InfoboardHeight = 160;

        // Logger origin point
        public const int LoggerStartX = SidebarStartX + Margin;
        public const int LoggerStartY = InfoboardStartY + InfoboardHeight + Margin * 2;

        // Logger size
        public const int LoggerWidth = SidebarWidth - Margin * 2;
        public const int LoggerHeight = 200;

        // Color
        public static readonly Color BackgroundColor = StyleHelper.GetColor("#0A0A0A");  // #0A0A0A
    }
}