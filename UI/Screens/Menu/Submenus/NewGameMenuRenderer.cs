/* ----- ----- ----- ----- */
// NewGameMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.Utils.GraphicsUtils;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    public class NewGameMenuRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        private int width;
        public int Width
        {
            get => width;
            set
            {
                width = Math.Max(value, 1);
            }
        }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        private int height;
        public int Height
        {
            get => height;
            set
            {
                height = Math.Max(value, 1);
            }
        }
        private readonly NewGameMenu menu;

        public NewGameMenuRenderer(NewGameMenu menu)
        {
            this.menu = menu;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var buttons = menu.Buttons;
            var clip = menu.GetClipRect();

            DrawOutline(g);

            g.SetClip(clip);
            foreach (var button in buttons)
            {
                DrawButton(g, button.Text, button.GetAbsolutePosition(), button.Size);
            }
            g.ResetClip();
        }

        private void DrawOutline(Graphics g)
        {
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                float margin = 1.0f;
                debugPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(debugPen,
                UILayoutConstants.SecondMenu.Position.X + margin,
                UILayoutConstants.SecondMenu.Position.Y + margin,
                menu.Size.X - margin * 2,
                menu.Size.Y - margin * 2);
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

            using (GraphicsPath outerPath = GraphicsPaths.CreateRoundedRectPath(
                outerPathRect.Width,
                outerPathRect.Height,
                Styles.MainMenu.Button.Border.CornerRadius))
            using (GraphicsPath innerPath = GraphicsPaths.CreateRoundedRectPath(
                innerPathRect.Width,
                innerPathRect.Height,
                Styles.MainMenu.Button.Border.CornerRadius - Styles.MainMenu.Button.Border.Margin))
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
