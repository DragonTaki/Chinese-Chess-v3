/* ----- ----- ----- ----- */
// BoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/08
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.Utils.GraphicsUtils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Game.Board
{
    public class BoardRenderer
    {
        private Pen boardPen;
        public BoardRenderer()
        {
            // Customized pen
            boardPen = new Pen(Color.Black, UILayoutConstants.Board.Grid.LineWidth);
        }

        // Draw whole board
        public void DrawBoard(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            // Step 1: Draw the background
            RectangleF fullArea = Vector2F.ToRectangleF(UILayoutConstants.Board.Position, UILayoutConstants.Board.Size);

            using (Brush backgroundBrush = BoardStyles.CreateBoardBackgroundBrush(fullArea))
            {
                g.FillRectangle(backgroundBrush, fullArea);
            }

            // Calculated from the origin point
            // Step 2: Draw vertical lines for the grid
            for (int i = 1; i < Configs.Constants.Board.Columns - 1; i++)
            {
                float x = UILayoutConstants.Board.Position.X + i * UILayoutConstants.Board.Grid.Size;
                // Black side vertical lines
                g.DrawLine(
                    boardPen,
                    x,
                    UILayoutConstants.Board.Position.Y,
                    x,
                    UILayoutConstants.Board.Position.Y + Configs.Constants.Board.BlackYSideRiverLine * UILayoutConstants.Board.Grid.Size
                );
                // Red side vertical lines
                g.DrawLine(
                    boardPen,
                    x,
                    UILayoutConstants.Board.Position.Y + Configs.Constants.Board.RedYSideRiverLine * UILayoutConstants.Board.Grid.Size,
                    x,
                    UILayoutConstants.Board.Position.Y + (Configs.Constants.Board.Rows - 1) * UILayoutConstants.Board.Grid.Size
                );
            }

            // Step 3: Draw the river (empty space between the 5th and 6th row)
            // ----- None -----

            // Step 4: Draw horizontal lines for the grid
            for (int i = 1; i < Configs.Constants.Board.Rows - 1; i++)
            {
                float y = UILayoutConstants.Board.Position.Y + i * UILayoutConstants.Board.Grid.Size;
                // Horizontal lines
                g.DrawLine(
                    boardPen,
                    UILayoutConstants.Board.Position.X,
                    y,
                    UILayoutConstants.Board.Position.X + (Configs.Constants.Board.Columns - 1) * UILayoutConstants.Board.Grid.Size,
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
            float x1 = UILayoutConstants.Board.Position.X + Configs.Constants.Board.PalaceXRange.MinX * UILayoutConstants.Board.Grid.Size;
            float y1 = UILayoutConstants.Board.Position.Y + Configs.Constants.Board.BlackPalaceYRange.MinY * UILayoutConstants.Board.Grid.Size;
            float x2 = UILayoutConstants.Board.Position.X + Configs.Constants.Board.PalaceXRange.MaxX * UILayoutConstants.Board.Grid.Size;
            float y2 = UILayoutConstants.Board.Position.Y + Configs.Constants.Board.BlackPalaceYRange.MaxY * UILayoutConstants.Board.Grid.Size;

            g.DrawLine(pen, x1, y1, x2, y2);  // Left-top to right-bottom
            g.DrawLine(pen, x2, y1, x1, y2);  // Right-top to left-bottom

            // Red side palace (bottom)
            float x3 = UILayoutConstants.Board.Position.X + Configs.Constants.Board.PalaceXRange.MinX * UILayoutConstants.Board.Grid.Size;
            float y3 = UILayoutConstants.Board.Position.Y + Configs.Constants.Board.RedPalaceYRange.MinY * UILayoutConstants.Board.Grid.Size;
            float x4 = UILayoutConstants.Board.Position.X + Configs.Constants.Board.PalaceXRange.MaxX * UILayoutConstants.Board.Grid.Size;
            float y4 = UILayoutConstants.Board.Position.Y + Configs.Constants.Board.RedPalaceYRange.MaxY * UILayoutConstants.Board.Grid.Size;

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
            float cx = UILayoutConstants.Board.Position.X + x * UILayoutConstants.Board.Grid.Size;
            float cy = UILayoutConstants.Board.Position.Y + y * UILayoutConstants.Board.Grid.Size;

            float cornerLength = 6.0f;
            float gap = 4.0f;

            bool leftEdge = x == 0;
            bool rightEdge = x == Configs.Constants.Board.Columns - 1;

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
            float gap1 = 0.0f;
            float gap2 = UILayoutConstants.Board.Grid.LineWidth * 2;
            float boardWidthPx = (Configs.Constants.Board.Columns - 1) * UILayoutConstants.Board.Grid.Size;
            float boardHeightPx = (Configs.Constants.Board.Rows - 1) * UILayoutConstants.Board.Grid.Size;

            // Padding is calculated from the origin point, subtracting gap to move outward
            RectangleF outerRect1 = new RectangleF(
                UILayoutConstants.Board.Position.X - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                UILayoutConstants.Board.Position.Y - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                boardWidthPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth,
                boardHeightPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth
            );

            RectangleF outerRect2 = new RectangleF(
                UILayoutConstants.Board.Position.X - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                UILayoutConstants.Board.Position.Y - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                boardWidthPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth,
                boardHeightPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth
            );

            g.DrawRectangle(pen, outerRect1);
            g.DrawRectangle(pen, outerRect2);
        }
    }
}