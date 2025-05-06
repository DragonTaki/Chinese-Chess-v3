/* ----- ----- ----- ----- */
// PieceRenderer.cs
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
    public class PieceRenderer
    {
        public void DrawInitialPieces(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);
            
            foreach (var piece in PieceConstants.InitialPieces)
            {
                DrawPiece(g, piece);
            }
        }
        private void DrawPiece(Graphics g, PieceInfo piece)
        {
            int centerX = BoardSettings.StartX + piece.X * BoardSettings.GridSize;
            int centerY = BoardSettings.StartY + piece.Y * BoardSettings.GridSize;

            int radius = PieceSettings.Radius;
            int outerRadius = radius - PieceSettings.OuterMargin;

            // Draw main circle (fill color)
            Brush fillBrush = piece.IsRed ? Brushes.White : Brushes.Black;
            g.FillEllipse(fillBrush, centerX - radius, centerY - radius, radius * 2, radius * 2);

            // Draw border circle (outline color)
            Pen outlinePen = new Pen(piece.IsRed ? Color.Red : Color.White, 2);
            g.DrawEllipse(outlinePen, centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2);

            // Draw text (label)
            string label = PieceConstants.GetPieceText(piece.Type, piece.IsRed);
            Font font = PieceSettings.Font;
            SizeF textSize = g.MeasureString(label, font);
            Brush textBrush = piece.IsRed ? Brushes.Red : Brushes.White;
            g.DrawString(label, font, textBrush, centerX - textSize.Width / 2, centerY - textSize.Height / 2);
        }
    }
}