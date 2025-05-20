/* ----- ----- ----- ----- */
// ConfirmDialogRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.UI.Core.Elements;

namespace Chinese_Chess_v3.UI.Dialog
{
    public class ConfirmDialogRenderer
    {
        public void Draw(Graphics g, ConfirmDialog dialog)
        {
            var bounds = dialog.GetCurrentAbsoluteBounds();
            using var bgBrush = new SolidBrush(Color.FromArgb(220, Color.DarkGray));
            using var borderPen = new Pen(Color.Black, 2);

            g.FillRectangle(bgBrush, bounds);
            g.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            
            foreach (var child in dialog.Children.OfType<UIButton<ConfirmDialogResult>>())
            {
                DrawButton(g, child);
            }
        }
        
        private void DrawButton(Graphics g, UIButton<ConfirmDialogResult> button)
        {
            var bounds = button.GetCurrentAbsoluteBounds();

            using var brush = new SolidBrush(Color.LightGray);
            using var border = new Pen(Color.Black, 1);
            using var textBrush = new SolidBrush(Color.Black);
            using var font = new Font("Microsoft JhengHei", 12);

            // 背景
            g.FillRectangle(brush, bounds);
            g.DrawRectangle(border, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            // 文字置中
            var text = button.Text;
            var textSize = g.MeasureString(text, font);
            var textX = bounds.X + (bounds.Width - textSize.Width) / 2;
            var textY = bounds.Y + (bounds.Height - textSize.Height) / 2;

            g.DrawString(text, font, textBrush, textX, textY);
        }
    }
}
