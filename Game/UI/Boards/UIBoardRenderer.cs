/* ----- ----- ----- ----- */
// UIBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.UI.Boards.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.GraphicsUtils;
using Engine.Mathematics;
using Engine.Platform;
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

        public override void OnRender(IGraphics g, UIBoard element)
        {
            _composite.Render(g, element);
        }

        private class ClassicBoard : UIRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
        {
            private IPen _boardPen = GraphicsBackend.Factory.CreatePen(Color.Black, UILayoutConstants.Board.Grid.LineWidth);

            // Grid geometry for this frame, from the board's resolved rectangle (see
            // UIBoard's Grid Geometry region) - not the authored constants, so the lines
            // follow the board when the layout resizes or moves it.
            private Vector2F _origin;
            private float _cell;
            private float _scale = 1f;

            // Draw whole board
            public override void OnRender(IGraphics g, UIBoard element)
            {
                GraphicsHelper.ApplyHighQualitySettings(g);

                _origin = element.GridOrigin;
                _cell = element.GridCellSize;
                _scale = element.GridScale;

                // Step 1: Draw the background (absolute bounds: LocalPosition was used, which
                // only matched while the board's parent sat at the origin)
                RectangleF fullArea = element.GetCurrentAbsoluteBounds();

                using (IBrush backgroundBrush = UIBoardStyles.CreateBoardBackgroundBrush(fullArea))
                {
                    g.FillRectangle(backgroundBrush, fullArea);
                }

                // Calculated from the origin point
                // Step 2: Draw vertical lines for the grid
                for (int i = 1; i < BoardConstants.Full.Columns - 1; i++)
                {
                    float x = _origin.X + i * _cell;
                    float y = _origin.Y;
                    // Black side vertical lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y,
                        x,
                        y + BoardConstants.Full.RiverLineYBlackSide * _cell
                    );
                    // Red side vertical lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y + BoardConstants.Full.RiverLineYRedSide * _cell,
                        x,
                        y + (BoardConstants.Full.Rows - 1) * _cell
                    );
                }

                // Step 3: Draw the river (empty space between the 5th and 6th row)
                // ----- None -----

                // Step 4: Draw horizontal lines for the grid
                for (int i = 1; i < BoardConstants.Full.Rows - 1; i++)
                {
                    float x = _origin.X;
                    float y = _origin.Y + i * _cell;
                    // Horizontal lines
                    g.DrawLine(
                        _boardPen,
                        x,
                        y,
                        x + (BoardConstants.Full.Columns - 1) * _cell,
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
            private void DrawPalaces(IGraphics g, IPen pen)
            {
                // Calculated from the origin point
                // Black side palace (top)
                float x1 = _origin.X + BoardConstants.Full.PalaceXRange.MinX * _cell;
                float y1 = _origin.Y + BoardConstants.Full.BlackPalaceYRange.MinY * _cell;
                float x2 = _origin.X + BoardConstants.Full.PalaceXRange.MaxX * _cell;
                float y2 = _origin.Y + BoardConstants.Full.BlackPalaceYRange.MaxY * _cell;

                g.DrawLine(pen, x1, y1, x2, y2);  // Left-top to right-bottom
                g.DrawLine(pen, x2, y1, x1, y2);  // Right-top to left-bottom

                // Red side palace (bottom)
                float x3 = _origin.X + BoardConstants.Full.PalaceXRange.MinX * _cell;
                float y3 = _origin.Y + BoardConstants.Full.RedPalaceYRange.MinY * _cell;
                float x4 = _origin.X + BoardConstants.Full.PalaceXRange.MaxX * _cell;
                float y4 = _origin.Y + BoardConstants.Full.RedPalaceYRange.MaxY * _cell;

                g.DrawLine(pen, x3, y3, x4, y4);  // Left-bottom to right-top
                g.DrawLine(pen, x4, y3, x3, y4);  // Right-bottom to left-top
            }

            // Draw cannon's and soldier's anchor point ("L" shape)
            private void DrawPositioningPoints(IGraphics g, IPen pen)
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
            void DrawCorner(IGraphics g, int x, int y, IPen pen)
            {
                // Calculated from the origin point
                float cx = _origin.X + x * _cell;
                float cy = _origin.Y + y * _cell;

                float cornerLength = 6.0f * _scale;
                float gap = 4.0f * _scale;

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
            private void DrawOuterFrame(IGraphics g, IPen pen)
            {
                // Gap between grid line and frame line
                float gap1 = 0.0f;
                float gap2 = UILayoutConstants.Board.Grid.LineWidth * 2;
                float boardWidthPx = (BoardConstants.Full.Columns - 1) * _cell;
                float boardHeightPx = (BoardConstants.Full.Rows - 1) * _cell;

                // Padding is calculated from the origin point, subtracting gap to move outward
                RectangleF outerRect1 = new RectangleF(
                    _origin.X - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    _origin.Y - gap1 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    boardWidthPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth,
                    boardHeightPx + 2 * gap1 + UILayoutConstants.Board.Grid.LineWidth
                );

                RectangleF outerRect2 = new RectangleF(
                    _origin.X - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    _origin.Y - gap2 - UILayoutConstants.Board.Grid.LineWidth / 2,
                    boardWidthPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth,
                    boardHeightPx + 2 * gap2 + UILayoutConstants.Board.Grid.LineWidth
                );

                g.DrawRectangle(pen, outerRect1);
                g.DrawRectangle(pen, outerRect2);
            }
        }
    }
}
