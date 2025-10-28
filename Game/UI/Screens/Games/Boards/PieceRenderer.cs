/* ----- ----- ----- ----- */
// PieceRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/22
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Game.Configs.Board;
using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Models;
using Chinese_Chess_v3.Game.UI.Elements;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    public class PieceRenderer : UIRenderer<ChessBoard, ChessBoardHandler, ChessBoardRenderer>
    {
        private readonly Pieces _pieces = new Pieces();
        public PieceRenderer() { }

        public override void OnRender(Graphics g, ChessBoard element)
        {
            if (element is ChessBoard board)
            {
                _pieces.Draw(g, board.PieceBinder.UIPieces);
            }
        }

        private class Pieces
        {
            public void Draw(Graphics g, List<UIPiece> uiPieces)
            {
                if (uiPieces == null) return;
                foreach (var uiPiece in uiPieces)
                {
                    DrawPiece(g, uiPiece);
                }
            }

            private void DrawPiece(Graphics g, UIPiece uiPiece)
            {
                Piece piece = uiPiece.PieceModel;
                float centerX = UILayoutConstants.Board.Grid.Position.X + piece.X * UILayoutConstants.Board.Grid.CellSize;
                float centerY = UILayoutConstants.Board.Grid.Position.Y + piece.Y * UILayoutConstants.Board.Grid.CellSize;

                float radius = PieceSettings.Radius;
                float outerRadius = radius - PieceSettings.OuterMargin;

                bool isRed = piece.Side == PlayerSide.Red;

                if (uiPiece.IsSelected)
                {
                    float glowRadius = radius + PieceSettings.GlowMargin;
                    Color glowColor = PieceSettings.GlowColor;
                    using (SolidBrush glowBrush = new SolidBrush(glowColor))
                    {
                        g.FillEllipse(glowBrush, centerX - glowRadius, centerY - glowRadius, glowRadius * 2, glowRadius * 2);
                    }
                }

                // Draw main circle (fill color)
                Brush fillBrush = isRed ? PieceSettings.RedBackgroundBrush : PieceSettings.BlackBackgroundBrush;
                g.FillEllipse(fillBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);

                // Draw border circle (outline color)
                Pen outlinePen = new Pen(isRed ? PieceSettings.RedOutlineColor : PieceSettings.BlackOutlineColor,
                                         isRed ? PieceSettings.RedOutlineWidth : PieceSettings.BlackOutlineWidth);
                g.DrawEllipse(outlinePen, centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);

                // Draw text (label)
                string label = PieceConstants.GetPieceText(piece.Type, isRed);
                Font font = PieceSettings.Font;
                SizeF textSize = g.MeasureString(label, font);
                Brush textBrush = isRed ? PieceSettings.RedTextBrush : PieceSettings.BlackTextBrush;
                g.DrawString(label, font, textBrush, centerX - textSize.Width / 2, centerY - textSize.Height / 2);
            }
        }
    }
}
