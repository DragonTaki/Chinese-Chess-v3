/* ----- ----- ----- ----- */
// PieceRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Configs.Board;
using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Interface.UI.Constants;
using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.Utils.GraphicsUtils;

namespace Chinese_Chess_v3.Renderers
{
    public class PieceRenderer
    {
        public void DrawPieces(Graphics g, List<Piece> pieces, Piece selectedPiece = null)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);

            foreach (var piece in pieces)
            {
                bool isSelected = selectedPiece != null && piece == selectedPiece;
                DrawPiece(g, piece, isSelected);
            }
        }
        private void DrawPiece(Graphics g, Piece piece, bool isSelected)
        {
            float centerX = UILayoutConstants.Board.Position.X + piece.X * UILayoutConstants.Board.Grid.Size;
            float centerY = UILayoutConstants.Board.Position.Y + piece.Y * UILayoutConstants.Board.Grid.Size;

            float radius = PieceSettings.Radius;
            float outerRadius = radius - PieceSettings.OuterMargin;

            bool isRed = piece.Side == PlayerSide.Red;

            if (isSelected)
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