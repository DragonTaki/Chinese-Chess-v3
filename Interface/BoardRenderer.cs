/* ----- ----- ----- ----- */
// BoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Interface
{
    public class BoardRenderer
    {
        private Pen boardPen;
        public BoardRenderer()
        {
            // Customized pen
            boardPen = new Pen(Color.Black, BoardSettings.LineWidth);
        }

        // Draw whole board
        public void DrawBoard(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            // Step 1: Draw the background
            int startX = BoardSettings.BoardFrameStartX;
            int startY = BoardSettings.BoardFrameStartY;
            int fullWidth = BoardConstants.BoardTotalWidth;
            int fullHeight = BoardConstants.BoardTotalHeight;
            Rectangle fullArea = new Rectangle(startX, startY, fullWidth, fullHeight);

            using (Brush backgroundBrush = BoardStyles.CreateBoardBackgroundBrush(fullArea))
            {
                g.FillRectangle(backgroundBrush, fullArea);
            }

            // Calculated from the origin point
            // Step 2: Draw vertical lines for the grid
            for (int i = 1; i < BoardConstants.Columns - 1; i++)
            {
                int x = BoardSettings.BoardStartX + i * BoardSettings.GridSize;
                // Black side vertical lines
                g.DrawLine(
                    boardPen,
                    x,
                    BoardSettings.BoardStartY,
                    x,
                    BoardSettings.BoardStartY + BoardConstants.BlackYSideRiverLine * BoardSettings.GridSize
                );
                // Red side vertical lines
                g.DrawLine(
                    boardPen,
                    x,
                    BoardSettings.BoardStartY + BoardConstants.RedYSideRiverLine * BoardSettings.GridSize,
                    x,
                    BoardSettings.BoardStartY + (BoardConstants.Rows - 1) * BoardSettings.GridSize
                );
            }

            // Step 3: Draw the river (empty space between the 5th and 6th row)
            // ----- None -----

            // Step 4: Draw horizontal lines for the grid
            for (int i = 1; i < BoardConstants.Rows - 1; i++)
            {
                int y = BoardSettings.BoardStartY + i * BoardSettings.GridSize;
                // Horizontal lines
                g.DrawLine(
                    boardPen,
                    BoardSettings.BoardStartX,
                    y,
                    BoardSettings.BoardStartX + (BoardConstants.Columns - 1) * BoardSettings.GridSize,
                    y
                );
            }

            // Step 5: Drow palace's diagonal line ("X" shape)
            DrawPalaces(g, boardPen);

            // Step 6: Draw cannon's and soldier's anchor point ("L" shape)
            DrawPositioningPoints(g, boardPen);

            // Step 7: Draw board border ("=" line)
            DrawOuterFrame(g, boardPen);
        }

        // Drow palace's diagonal line ("X" shape)
        private void DrawPalaces(Graphics g, Pen pen)
        {
            // Calculated from the origin point
            // Black side palace (top)
            int x1 = BoardSettings.BoardStartX + BoardConstants.PalaceXRange.MinX * BoardSettings.GridSize;
            int y1 = BoardSettings.BoardStartY + BoardConstants.BlackPalaceYRange.MinY * BoardSettings.GridSize;
            int x2 = BoardSettings.BoardStartX + BoardConstants.PalaceXRange.MaxX * BoardSettings.GridSize;
            int y2 = BoardSettings.BoardStartY + BoardConstants.BlackPalaceYRange.MaxY * BoardSettings.GridSize;

            g.DrawLine(pen, x1, y1, x2, y2);  // Left-top to right-bottom
            g.DrawLine(pen, x2, y1, x1, y2);  // Right-top to left-bottom

            // Red side palace (bottom)
            int x3 = BoardSettings.BoardStartX + BoardConstants.PalaceXRange.MinX * BoardSettings.GridSize;
            int y3 = BoardSettings.BoardStartY + BoardConstants.RedPalaceYRange.MinY * BoardSettings.GridSize;
            int x4 = BoardSettings.BoardStartX + BoardConstants.PalaceXRange.MaxX * BoardSettings.GridSize;
            int y4 = BoardSettings.BoardStartY + BoardConstants.RedPalaceYRange.MaxY * BoardSettings.GridSize;

            g.DrawLine(pen, x3, y3, x4, y4);  // Left-bottom to right-top
            g.DrawLine(pen, x4, y3, x3, y4);  // Right-bottom to left-top
        }

        // Draw cannon's and soldier's anchor point ("L" shape)
        private void DrawPositioningPoints(Graphics g, Pen pen)
        {
            // Solider's anchor coordinate
            int[] soldierCols = { 0, 2, 4, 6, 8 };
            foreach (int col in soldierCols)
            {
                DrawCorner(g, col, 3, pen);  // Black side
                DrawCorner(g, col, 6, pen);  // Red side
            }

            // Cannon's anchor coordinate
            int[] cannonCols = { 1, 7 };
            foreach (int col in cannonCols)
            {
                DrawCorner(g, col, 2, pen);  // Black side
                DrawCorner(g, col, 7, pen);  // Red side
            }
        }

        // Draw a small "L" shape near each point
        void DrawCorner(Graphics g, int x, int y, Pen pen)
        {
            // Calculated from the origin point
            int cx = BoardSettings.BoardStartX + x * BoardSettings.GridSize;
            int cy = BoardSettings.BoardStartY + y * BoardSettings.GridSize;

            int cornerLength = 6;
            int gap = 4;

            bool leftEdge = x == 0;
            bool rightEdge = x == BoardConstants.Columns - 1;

            // Top-left
            if (!leftEdge)
            {
                g.DrawLine(pen, cx - gap - cornerLength, cy - gap, cx - gap, cy - gap);  // horizontal
                g.DrawLine(pen, cx - gap, cy - gap - cornerLength, cx - gap, cy - gap);  // vertical
            }

            // Top-right
            if (!rightEdge)
            {
                g.DrawLine(pen, cx + gap, cy - gap, cx + gap + cornerLength, cy - gap);  // horizontal
                g.DrawLine(pen, cx + gap, cy - gap - cornerLength, cx + gap, cy - gap);  // vertical
            }

            // Bottom-left
            if (!leftEdge)
            {
                g.DrawLine(pen, cx - gap - cornerLength, cy + gap, cx - gap, cy + gap);  // horizontal
                g.DrawLine(pen, cx - gap, cy + gap, cx - gap, cy + gap + cornerLength);  // vertical
            }

            // Bottom-right
            if (!rightEdge)
            {
                g.DrawLine(pen, cx + gap, cy + gap, cx + gap + cornerLength, cy + gap);  // horizontal
                g.DrawLine(pen, cx + gap, cy + gap, cx + gap, cy + gap + cornerLength);  // vertical
            }
        }

        // Draw board border ("=" line)
        private void DrawOuterFrame(Graphics g, Pen pen)
        {
            // Gap between grid line and frame line
            int gap1 = 0;
            int gap2 = BoardSettings.LineWidth * 2;
            int boardWidthPx = (BoardConstants.Columns - 1) * BoardSettings.GridSize;
            int boardHeightPx = (BoardConstants.Rows - 1) * BoardSettings.GridSize;

            // Padding is calculated from the origin point, subtracting gap to move outward
            Rectangle outerRect1 = new Rectangle(
                BoardSettings.BoardStartX - gap1 - BoardSettings.LineWidth / 2,
                BoardSettings.BoardStartY - gap1 - BoardSettings.LineWidth / 2,
                boardWidthPx + 2 * gap1 + BoardSettings.LineWidth,
                boardHeightPx + 2 * gap1 + BoardSettings.LineWidth
            );

            Rectangle outerRect2 = new Rectangle(
                BoardSettings.BoardStartX - gap2 - BoardSettings.LineWidth / 2,
                BoardSettings.BoardStartY - gap2 - BoardSettings.LineWidth / 2,
                boardWidthPx + 2 * gap2 + BoardSettings.LineWidth,
                boardHeightPx + 2 * gap2 + BoardSettings.LineWidth
            );

            g.DrawRectangle(pen, outerRect1);
            g.DrawRectangle(pen, outerRect2);
        }
    }
}