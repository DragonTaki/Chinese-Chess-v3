/* ----- ----- ----- ----- */
// UIPieceRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

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
                _pieces.Draw(g, board.PieceBinder.UIPieces);
            }
        }

        private class Pieces
        {
            public void Draw(IGraphics g, List<UIPiece> uiPieces)
            {
                if (uiPieces == null) return;
                foreach (var uiPiece in uiPieces)
                {
                    DrawPiece(g, uiPiece);
                }
            }

            private void DrawPiece(IGraphics g, UIPiece uiPiece)
            {
                Piece piece = uiPiece.PieceModel;
                float centerX = UILayoutConstants.Board.Grid.Position.X + piece.X * UILayoutConstants.Board.Grid.CellSize;
                float centerY = UILayoutConstants.Board.Grid.Position.Y + piece.Y * UILayoutConstants.Board.Grid.CellSize;

                float radius = PieceSettings.Radius;
                float outerRadius = radius - PieceSettings.OuterMargin;

                // Visual color is piece.Color, not piece.Side — they're
                // deliberately decoupled (see PieceInfo.Color's doc
                // comment): a HalfCross faction-3 piece can be owned by
                // PlayerSide.Player3 while still being colored Red, and
                // must render as Red, not as whatever Player1 looks like.
                bool isRed = piece.Color == PieceColor.Red;

                if (uiPiece.IsSelected)
                {
                    float glowRadius = radius + PieceSettings.GlowMargin;
                    Color glowColor = PieceSettings.GlowColor;
                    using (IBrush glowBrush = GraphicsBackend.Factory.CreateSolidBrush(glowColor))
                    {
                        g.FillEllipse(glowBrush, centerX - glowRadius, centerY - glowRadius, glowRadius * 2, glowRadius * 2);
                    }
                }

                // Draw main circle (fill color)
                IBrush fillBrush = isRed ? PieceSettings.RedBackgroundBrush : PieceSettings.BlackBackgroundBrush;
                g.FillEllipse(fillBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);

                // Draw border circle (outline color)
                using IPen outlinePen = GraphicsBackend.Factory.CreatePen(isRed ? PieceSettings.RedOutlineColor : PieceSettings.BlackOutlineColor,
                                         isRed ? PieceSettings.RedOutlineWidth : PieceSettings.BlackOutlineWidth);
                g.DrawEllipse(outlinePen, centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);

                // Draw text (label)
                string label = PieceConstants.GetPieceText(piece.Type, piece.Color);
                IFont font = PieceSettings.Font;
                SizeF textSize = g.MeasureString(label, font);
                IBrush textBrush = isRed ? PieceSettings.RedTextBrush : PieceSettings.BlackTextBrush;
                g.DrawString(label, font, textBrush, centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            }
        }
    }
}
