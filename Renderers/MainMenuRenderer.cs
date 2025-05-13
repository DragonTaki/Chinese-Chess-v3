/* ----- ----- ----- ----- */
// MainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Interface.Controls;
using Chinese_Chess_v3.Utils.GraphicsUtils;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Interface.Renderers
{
    public class MainMenuRenderer
    {
        private ScrollContainer scroll = new ScrollContainer();
        private class MenuButton
        {
            public string Text { get; set; }
            public Vector2F Position { get; set; }
        }
/*
        public void Init(List<ButtonData> buttonList)
        {
            this.buttons = buttonList;
        }
        public void Draw(Graphics g, Vector2F scrollOffset)
        {
            foreach (var button in buttons)
            {
                var pos = button.Position + scrollOffset;
                DrawButton(g, button.Text, pos, Settings.MainMenu.Button.Size);
            }
        }
        */
        
        public void Draw(Graphics g, RectangleF bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Debug 虛線外框
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                debugPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(debugPen, bounds);
            }

            //scroll.ViewportBounds = new RectangleF(0, 40, 360, 800); // 顯示區域（預留上下透明區域）
            //sscroll.ContentHeight = 按鈕總高度;

            var buttons = new List<MenuButton>
            {
                new MenuButton { Text = "開新一局" },
                new MenuButton { Text = "遊戲設定" },
                new MenuButton { Text = "規則設定" },
                new MenuButton { Text = "教學／幫助" },
                new MenuButton { Text = "讀取存檔" },
                new MenuButton { Text = "離開遊戲" }
            };

            float margin = Settings.MainMenu.Margin;
            Vector2F size = Settings.MainMenu.Button.Size;
            Vector2F basePosition = Settings.MainMenu.Button.Position;

            for (int i = 0; i < buttons.Count; i++)
            {
                float y = basePosition.Y + i * (size.Y + margin);
                buttons[i].Position = new Vector2F(basePosition.X, y);
            }

            foreach (var button in buttons)
            {
                DrawButton(g, button.Text, button.Position, Settings.MainMenu.Button.Size);
            }
        }

        private void DrawButton(Graphics g, string text, Vector2F position, Vector2F size)
        {
            RectangleF rect = new RectangleF(position.X, position.Y, size.X, size.Y);

            // Outer border
            float outerGap = Styles.MainMenu.Button.Border.OuterWidth;
            RectangleF outerPathRect = new RectangleF(
                position.X + outerGap / 2,
                position.Y + outerGap / 2,
                size.X - outerGap,
                size.Y - outerGap
            );

            // Inner border
            float innerGap = Styles.MainMenu.Button.Border.OuterWidth + Styles.MainMenu.Button.Border.Margin * 2 + Styles.MainMenu.Button.Border.InnerWidth;
            RectangleF innerPathRect = new RectangleF(
                position.X + innerGap / 2,
                position.Y + innerGap / 2,
                size.X - innerGap,
                size.Y - innerGap
            );
            
            using (GraphicsPath outerPath = GraphicsPaths.CreateRoundedRectPath(outerPathRect.Width, outerPathRect.Height, Styles.MainMenu.Button.Border.CornerRadius))
            using (GraphicsPath innerPath = GraphicsPaths.CreateRoundedRectPath(innerPathRect.Width, innerPathRect.Height, Styles.MainMenu.Button.Border.CornerRadius - Styles.MainMenu.Button.Border.Margin))
            using (Matrix mOuter = new Matrix())
            using (Matrix mInner = new Matrix())
            {
                mOuter.Translate(outerPathRect.X, outerPathRect.Y);
                outerPath.Transform(mOuter);

                mInner.Translate(innerPathRect.X, innerPathRect.Y);
                innerPath.Transform(mInner);

                // Frosted fill
                using (LinearGradientBrush fillBrush = new LinearGradientBrush(
                    rect,
                    Styles.MainMenu.Button.Background.TopColor,
                    Styles.MainMenu.Button.Background.BottomColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(fillBrush, outerPath);
                }

                // Outer thick border
                using (Pen outerPen = new Pen(Styles.MainMenu.Button.Border.OuterColor, 4))
                {
                    g.DrawPath(outerPen, outerPath);
                }

                // Inner thin highlight
                using (Pen innerPen = new Pen(Styles.MainMenu.Button.Border.InnerColor, 2))
                {
                    g.DrawPath(innerPen, innerPath);
                }

                // Draw button text
                SizeF textSize = g.MeasureString(text, Styles.MainMenu.Button.Font);
                float textX = position.X + (size.X - textSize.Width) / 2;
                float textY = position.Y + (size.Y - textSize.Height) / 2;
                g.DrawString(text, Styles.MainMenu.Button.Font, Styles.MainMenu.Button.TextBrush, textX, textY);
            }
        }
    }
}
