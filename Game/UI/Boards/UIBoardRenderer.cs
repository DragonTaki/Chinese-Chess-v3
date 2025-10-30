/* ----- ----- ----- ----- */
// UIBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/31
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.UI.Boards.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.GraphicsUtils;
using Engine.Mathematics;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    public class UIBoardRenderer : UIContainerRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
    {
        protected CompositeRenderer<UIBoard, UIBoardHandler, UIBoardRenderer> _composite = new();

        public UIBoardRenderer() { }

        protected override void AfterInit()
        {
            SetupRendererChildren();
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new ClassicBoard())
                    .Add(new UIPieceRenderer());
            }
        }

        public override void OnRender(Graphics g, UIBoard element)
        {
            _composite.Render(g, element);
        }

        private class ClassicBoard : UIRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
        {
            private Pen _boardPen = new Pen(Color.Black, UILayoutConstants.Board.Grid.LineWidth);

            // Draw whole board
            public override void OnRender(Graphics g, UIBoard element)
            {
                GraphicsHelper.ApplyHighQualitySettings(g);

                // Step 1: Draw the background
                RectangleF fullArea = Vector2F.ToRectangleF(
                    new Vector2F(element.LocalPosition.Base.X, element.LocalPosition.Base.Y),
                    new Vector2F(element.Size.Width, element.Size.Height)
                );

                using (Brush backgroundBrush = UIBoardStyles.CreateBoardBackgroundBrush(fullArea))
                {
                    g.FillRectangle(backgroundBrush, fullArea);
                }

                // Calculated from the origin point
                // Step 2: Draw vertical lines for the grid
                for (int i = 1; i < BoardConstants.Full.Columns - 1; i++)
                {
                    float x = UILayoutConstants.Board.Grid.Position.X + i * UILayoutConstants.Board.Grid.CellSize;
                    float y = UILayoutConstants.Board.Grid.Position.Y;
                    // Black side vertical lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y,
                        x,
                        y + BoardConstants.Full.RiverLineYBlackSide * UILayoutConstants.Board.Grid.CellSize
                    );
                    // Red side vertical lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y + BoardConstants.Full.RiverLineYRedSide * UILayoutConstants.Board.Grid.CellSize,
                        x,
                        y + (BoardConstants.Full.Rows - 1) * UILayoutConstants.Board.Grid.CellSize
                    );
                }

                // Step 3: Draw the river (empty space between the 5th and 6th row)
                // ----- None -----

                // Step 4: Draw horizontal lines for the grid
                for (int i = 1; i < BoardConstants.Full.Rows - 1; i++)
                {
                    float x = UILayoutConstants.Board.Grid.Position.X;
                    float y = UILayoutConstants.Board.Grid.Position.Y + i * UILayoutConstants.Board.Grid.CellSize;
                    // Horizontal lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y,
                        x + (BoardConstants.Full.Columns - 1) * UILayoutConstants.Board.Grid.CellSize,
                        y
                    );
                }

                // Step 5: Drow palace's diagonal line ("X" shape)
                DrawPalaces(g, _boardPen);

                // Step 6: Draw cannon's and soldier's anchor point ("L" shape)
                DrawPositioningPoints(g, _boardPen);

                // Step 7: Draw board border ("=" line)
                DrawOuterFrame(g, _boardPen);
            }

            // Drow palace's diagonal line ("X" shape)
            private void DrawPalaces(Graphics g, Pen pen)
            {
                // Calculated from the origin point
                // Black side palace (top)
                float x1 = UILayoutConstants.Board.Grid.Position.X + BoardConstants.Full.PalaceXRange.MinX * UILayoutConstants.Board.Grid.CellSize;
                float y1 = UILayoutConstants.Board.Grid.Position.Y + BoardConstants.Full.BlackPalaceYRange.MinY * UILayoutConstants.Board.Grid.CellSize;
                float x2 = UILayoutConstants.Board.Grid.Position.X + BoardConstants.Full.PalaceXRange.MaxX * UILayoutConstants.Board.Grid.CellSize;
                float y2 = UILayoutConstants.Board.Grid.Position.Y + BoardConstants.Full.BlackPalaceYRange.MaxY * UILayoutConstants.Board.Grid.CellSize;

                g.DrawLine(pen, x1, y1, x2, y2);  // Left-top to right-bottom
                g.DrawLine(pen, x2, y1, x1, y2);  // Right-top to left-bottom

                // Red side palace (bottom)
                float x3 = UILayoutConstants.Board.Grid.Position.X + BoardConstants.Full.PalaceXRange.MinX * UILayoutConstants.Board.Grid.CellSize;
                float y3 = UILayoutConstants.Board.Grid.Position.Y + BoardConstants.Full.RedPalaceYRange.MinY * UILayoutConstants.Board.Grid.CellSize;
                float x4 = UILayoutConstants.Board.Grid.Position.X + BoardConstants.Full.PalaceXRange.MaxX * UILayoutConstants.Board.Grid.CellSize;
                float y4 = UILayoutConstants.Board.Grid.Position.Y + BoardConstants.Full.RedPalaceYRange.MaxY * UILayoutConstants.Board.Grid.CellSize;

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
                float cx = UILayoutConstants.Board.Grid.Position.X + x * UILayoutConstants.Board.Grid.CellSize;
                float cy = UILayoutConstants.Board.Grid.Position.Y + y * UILayoutConstants.Board.Grid.CellSize;

                float cornerLength = 6.0f;
                float gap = 4.0f;

                bool leftEdge = x == 0;
                bool rightEdge = x == BoardConstants.Full.Columns - 1;

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
                float boardWidthPx = (BoardConstants.Full.Columns - 1) * UILayoutConstants.Board.Grid.CellSize;
                float boardHeightPx = (BoardConstants.Full.Rows - 1) * UILayoutConstants.Board.Grid.CellSize;

                // Padding is calculated from the origin point, subtracting gap to move outward
                RectangleF outerRect1 = new RectangleF(
                    UILayoutConstants.Board.Grid.Position.X - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    UILayoutConstants.Board.Grid.Position.Y - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    boardWidthPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth,
                    boardHeightPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth
                );

                RectangleF outerRect2 = new RectangleF(
                    UILayoutConstants.Board.Grid.Position.X - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    UILayoutConstants.Board.Grid.Position.Y - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    boardWidthPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth,
                    boardHeightPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth
                );

                g.DrawRectangle(pen, outerRect1);
                g.DrawRectangle(pen, outerRect2);
            }
        }
    }
}
