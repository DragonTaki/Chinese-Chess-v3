/* ----- ----- ----- ----- */
// UIBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/01
// Version: v2.2
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
            // Bind the composite and its parts to this element (nothing else initializes it).
            _composite.Init(Element);
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                // One board drawing per board type (each draws only for its own type).
                _composite
                    .Add(new ClassicBoard())
                    .Add(new HalfCenterBoard())
                    .Add(new UIPieceRenderer());
            }
        }

        public override void OnRender(IGraphics g, UIBoard element)
        {
            _composite.Render(g, element);
        }

        private class ClassicBoard : UIRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
        {
            // Grid pen, rebuilt (and the old one disposed) only when the detail scale changes.
            private IPen _boardPen;
            private float _boardPenScale = float.NaN;

            // Grid geometry for this frame, from the board's resolved rectangle (see
            // UIBoard's Grid Geometry region) - not the authored constants, so the lines
            // follow the board when the layout resizes or moves it.
            private Vector2F _origin;
            private float _cell;
            private float _scale = 1f;       // detail scale (UIBoard.DetailScale)
            private float _lineWidth = UILayoutConstants.Board.Grid.LineWidth;

            // Draw whole board
            public override void OnRender(IGraphics g, UIBoard element)
            {
                // The Full board (9x10, pieces on the crossings) only.
                if (element.BoardType != BoardType.Full)
                    return;

                GraphicsHelper.ApplyHighQualitySettings(g);

                _origin = element.GridOrigin;
                _cell = element.GridCellSize;
                _scale = element.DetailScale;
                _lineWidth = UILayoutConstants.Board.Grid.LineWidth * _scale;
                UpdateBoardPen();

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

                // Step 5: Draw palace's diagonal line ("X" shape)
                DrawPalaces(g, _boardPen);

                // Step 6: Draw cannon's and soldier's anchor point ("L" shape)
                DrawPositioningPoints(g, _boardPen);

                // Step 7: Draw board border ("=" line)
                DrawOuterFrame(g, _boardPen);
            }

            private void UpdateBoardPen()
            {
                if (_boardPen != null && _boardPenScale == _scale)
                    return;
                _boardPen?.Dispose();
                _boardPen = GraphicsBackend.Factory.CreatePen(Color.Black, _lineWidth);
                _boardPenScale = _scale;
            }

            // Draw palace's diagonal line ("X" shape)
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

                g.DrawLine(pen, x3, y3, x4, y4);  // Left-top to right-bottom
                g.DrawLine(pen, x4, y3, x3, y4);  // Right-top to left-bottom
            }

            // Draw cannon's and soldier's anchor point ("L" shape)
            private void DrawPositioningPoints(IGraphics g, IPen pen)
            {
                // Soldier's anchor coordinate
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

                float cornerLength = UILayoutConstants.Board.Grid.MarkLength * _scale;
                float gap = UILayoutConstants.Board.Grid.MarkGap * _scale;

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
                float gap2 = _lineWidth * 2;
                float boardWidthPx = (BoardConstants.Full.Columns - 1) * _cell;
                float boardHeightPx = (BoardConstants.Full.Rows - 1) * _cell;

                // Padding is calculated from the origin point, subtracting gap to move outward
                RectangleF outerRect1 = new RectangleF(
                    _origin.X - gap1 - _lineWidth / 2,
                    _origin.Y - gap1 - _lineWidth / 2,
                    boardWidthPx + 2 * gap1 + _lineWidth,
                    boardHeightPx + 2 * gap1 + _lineWidth
                );

                RectangleF outerRect2 = new RectangleF(
                    _origin.X - gap2 - _lineWidth / 2,
                    _origin.Y - gap2 - _lineWidth / 2,
                    boardWidthPx + 2 * gap2 + _lineWidth,
                    boardHeightPx + 2 * gap2 + _lineWidth
                );

                g.DrawRectangle(pen, outerRect1);
                g.DrawRectangle(pen, outerRect2);
            }
        }

        /// <summary>
        /// The HalfCenter board (台灣暗棋半盤, 8x4): the same background and line style as the
        /// Full board, a grid of Columns x Rows cells (pieces stand in the cells, so there is
        /// no palace, river or position mark) and the same double outer frame around it.
        /// </summary>
        private class HalfCenterBoard : UIRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
        {
            // Grid pen, rebuilt (and the old one disposed) only when the detail scale changes.
            private IPen _boardPen;
            private float _boardPenScale = float.NaN;

            public override void OnRender(IGraphics g, UIBoard element)
            {
                if (element.BoardType != BoardType.HalfCenter)
                    return;

                GraphicsHelper.ApplyHighQualitySettings(g);

                // Grid geometry from the board's resolved rectangle (see UIBoard's Grid Geometry).
                Vector2F origin = element.GridOrigin;
                float cell = element.GridCellSize;
                float scale = element.DetailScale;
                float lineWidth = UILayoutConstants.Board.Grid.LineWidth * scale;
                UpdateBoardPen(scale, lineWidth);

                RectangleF fullArea = element.GetCurrentAbsoluteBounds();
                using (IBrush backgroundBrush = UIBoardStyles.CreateBoardBackgroundBrush(fullArea))
                {
                    g.FillRectangle(backgroundBrush, fullArea);
                }

                int columns = BoardConstants.HalfCenter.Columns;
                int rows = BoardConstants.HalfCenter.Rows;
                float width = columns * cell;
                float height = rows * cell;

                // Inner cell borders (the outermost ones are the frame below).
                for (int i = 1; i < columns; i++)
                {
                    float x = origin.X + i * cell;
                    g.DrawLine(_boardPen, x, origin.Y, x, origin.Y + height);
                }
                for (int j = 1; j < rows; j++)
                {
                    float y = origin.Y + j * cell;
                    g.DrawLine(_boardPen, origin.X, y, origin.X + width, y);
                }

                // Double outer frame, like the Full board's: on the grid's edge, and a line
                // width * 2 further out.
                float gap = lineWidth * 2;
                g.DrawRectangle(_boardPen, new RectangleF(
                    origin.X - lineWidth / 2, origin.Y - lineWidth / 2,
                    width + lineWidth, height + lineWidth));
                g.DrawRectangle(_boardPen, new RectangleF(
                    origin.X - gap - lineWidth / 2, origin.Y - gap - lineWidth / 2,
                    width + 2 * gap + lineWidth, height + 2 * gap + lineWidth));
            }

            private void UpdateBoardPen(float scale, float lineWidth)
            {
                if (_boardPen != null && _boardPenScale == scale)
                    return;
                _boardPen?.Dispose();
                _boardPen = GraphicsBackend.Factory.CreatePen(Color.Black, lineWidth);
                _boardPenScale = scale;
            }
        }
    }
}
