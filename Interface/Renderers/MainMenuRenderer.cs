/* ----- ----- ----- ----- */
// MainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chinese_Chess_v3.Interface.Renderers
{
    public class MainMenuRenderer
    {
        private readonly Font titleFont = new Font("Arial", 36, FontStyle.Bold);
        private readonly Font buttonFont = new Font("Arial", 18, FontStyle.Bold);
        private readonly Brush titleBrush = Brushes.DarkRed;
        private readonly Brush buttonBrush = Brushes.Black;
        private readonly Brush buttonFill = Brushes.LightGray;

        public void Draw(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Beige);

            // 標題
            string title = "Chinese Chess v3";
            SizeF titleSize = g.MeasureString(title, titleFont);
            float titleX = (bounds.Width - titleSize.Width) / 2;
            float titleY = bounds.Height * 0.15f;
            g.DrawString(title, titleFont, titleBrush, titleX, titleY);

            // 按鈕們（起始範例：只有一個按鈕）
            DrawButton(g, "Start Game", bounds.Width / 2, (int)(bounds.Height * 0.4), 200, 50);
            DrawButton(g, "Settings", bounds.Width / 2, (int)(bounds.Height * 0.52), 200, 50);
            DrawButton(g, "Exit", bounds.Width / 2, (int)(bounds.Height * 0.64), 200, 50);
        }

        private void DrawButton(Graphics g, string text, int centerX, int topY, int width, int height)
        {
            Rectangle buttonRect = new Rectangle(centerX - width / 2, topY, width, height);
            g.FillRectangle(buttonFill, buttonRect);
            g.DrawRectangle(Pens.Black, buttonRect);

            SizeF textSize = g.MeasureString(text, buttonFont);
            float textX = centerX - textSize.Width / 2;
            float textY = topY + (height - textSize.Height) / 2;
            g.DrawString(text, buttonFont, buttonBrush, textX, textY);
        }
    }
}
