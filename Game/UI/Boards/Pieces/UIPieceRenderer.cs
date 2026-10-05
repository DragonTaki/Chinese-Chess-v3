/* ----- ----- ----- ----- */
// UIPieceRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/05
// Version: v2.6
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Game.Application.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Boards.Pieces
{
    public class UIPieceRenderer : UIRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
    {
        private readonly Pieces _pieces = new Pieces();
        public UIPieceRenderer() { }

        public override void OnRender(IGraphics g, UIBoard element)
        {
            if (element is UIBoard board)
            {
                // Board hints, already filtered by the player settings ([hints] in settings.ini).
                var viewModel = board.ViewModel;
                _pieces.Draw(g, board, board.PieceBinder.UIPieces, viewModel.LegalTargets, viewModel.HangingSquares);
            }
        }

        private class Pieces
        {
            // Piece font at the current detail scale: PieceSettings.Font itself at scale 1,
            // otherwise a scaled copy owned here, rebuilt (and the old one disposed) only
            // when the scale changes - never per frame.
            private IFont _font = PieceSettings.Font;
            private float _fontScale = 1f;

            public void Draw(IGraphics g, UIBoard board, List<UIPiece> uiPieces, IReadOnlyList<(int x, int y)> legalMoveTargets, IReadOnlyList<HangingSquare> hangingSquares)
            {
                if (uiPieces == null) return;
                float scale = board.DetailScale;
                IFont font = GetFont(scale);

                // Rings first, pieces after: a ring lies just outside a piece's radius and
                // must never cover a piece (not even a neighbour's edge). Where rings share a
                // square the later one wins: hanging < legal move < selected.
                // Face-up hanging pieces only (BoardViewModel.HangingSquares).
                if (hangingSquares != null)
                {
                    foreach (var square in hangingSquares)
                    {
                        Color color = square.Color == PieceColor.Red ? PieceSettings.HangingRedRingColor : PieceSettings.HangingBlackRingColor;
                        DrawRing(g, board, square.X, square.Y, scale, color);
                    }
                }
                if (legalMoveTargets != null)
                {
                    foreach (var (x, y) in legalMoveTargets)
                        DrawRing(g, board, x, y, scale, PieceSettings.LegalMoveRingColor);
                }
                foreach (var uiPiece in uiPieces)
                {
                    if (uiPiece.IsSelected)
                        DrawRing(g, board, uiPiece.PieceModel.X, uiPiece.PieceModel.Y, scale, PieceSettings.GlowColor);
                }

                foreach (var uiPiece in uiPieces)
                {
                    DrawPiece(g, board, uiPiece, scale, font);
                }
            }

            /// <summary>
            /// The ring around grid point (x, y) used for the selection glow and the board
            /// hints: from the piece radius out to <c>GlowMargin</c> past it, so it surrounds
            /// a piece standing there without covering it.
            /// </summary>
            private static void DrawRing(IGraphics g, UIBoard board, int x, int y, float scale, Color color)
            {
                var center = board.GridToPixel(x, y);
                float radius = board.PieceRadius;
                float width = UILayoutConstants.Board.Piece.GlowMargin * scale;
                if (width <= 0f)
                    return;
                // A pen is centered on its path: the path at the middle of the ring.
                float ringRadius = radius + width / 2f;
                using IPen pen = GraphicsBackend.Factory.CreatePen(color, width);
                g.DrawEllipse(pen, center.X - ringRadius, center.Y - ringRadius, ringRadius * 2, ringRadius * 2);
            }

            private IFont GetFont(float scale)
            {
                if (scale == _fontScale)
                    return _font;

                if (_font != PieceSettings.Font)
                    _font.Dispose();

                var baseFont = PieceSettings.Font;
                _font = scale == 1f
                    ? baseFont
                    : GraphicsBackend.Factory.CreateFont(baseFont.FontFamily, baseFont.Size * scale, baseFont.Style);
                _fontScale = scale;
                return _font;
            }

            private void DrawPiece(IGraphics g, UIBoard board, UIPiece uiPiece, float scale, IFont font)
            {
                Piece piece = uiPiece.PieceModel;
                // Grid point from the board's resolved rectangle, like the grid lines; sizes
                // scaled with the board by its detail scale (exactly 1 at the authored size).
                var center = board.GridToPixel(piece.X, piece.Y);
                float centerX = center.X;
                float centerY = center.Y;

                float radius = board.PieceRadius;
                float outerRadius = radius - UILayoutConstants.Board.Piece.OuterMargin * scale;

                // Face down (暗棋, 揭棋): only the piece's back - the same disc and border for
                // every piece, no character - so neither its type nor its colour shows.
                if (!piece.CurrentInfo.IsFaceUp)
                {
                    g.FillEllipse(PieceSettings.FaceDownBackgroundBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    using IPen backPen = GraphicsBackend.Factory.CreatePen(PieceSettings.FaceDownOutlineColor,
                        UILayoutConstants.Board.Piece.FaceDownOutlineWidth * scale);
                    g.DrawEllipse(backPen, centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);
                    return;
                }

                // Visual color is piece.Color, not piece.Side — they're
                // deliberately decoupled (see PieceInfo.Color's doc
                // comment): a HalfCross faction-3 piece can be owned by
                // PlayerSide.Player3 while still being colored Red, and
                // must render as Red, not as whatever Player1 looks like.
                bool isRed = piece.Color == PieceColor.Red;

                // The selection glow ring is drawn in Draw's ring pass, before all pieces.

                // Draw main circle (fill color)
                IBrush fillBrush = isRed ? PieceSettings.RedBackgroundBrush : PieceSettings.BlackBackgroundBrush;
                g.FillEllipse(fillBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);

                // Draw border circle (outline color)
                using IPen outlinePen = GraphicsBackend.Factory.CreatePen(isRed ? PieceSettings.RedOutlineColor : PieceSettings.BlackOutlineColor,
                                         (isRed ? UILayoutConstants.Board.Piece.RedOutlineWidth : UILayoutConstants.Board.Piece.BlackOutlineWidth) * scale);
                g.DrawEllipse(outlinePen, centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);

                // Draw text (label)
                string label = PieceConstants.GetPieceText(piece.Type, piece.Color);
                SizeF textSize = g.MeasureString(label, font);
                IBrush textBrush = isRed ? PieceSettings.RedTextBrush : PieceSettings.BlackTextBrush;
                g.DrawString(label, font, textBrush, centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            }
        }
    }
}
