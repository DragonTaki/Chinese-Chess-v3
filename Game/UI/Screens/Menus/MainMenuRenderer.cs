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

using Chinese_Chess_v3.Game.Constants.UI;

using Engine.Mathematics;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus
{
    public class MainMenuRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = Math.Max(value, 1);
            }
        }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                _height = Math.Max(value, 1);
            }
        }
        private readonly MainMenu _menu;

        public MainMenuRenderer(MainMenu menu)
        {
            _menu = menu;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var buttons = _menu.GetVisibleButtons();
            var clip = _menu.GetAbsClipRect();

            float margin = 3.0f;
            RectangleF rect = new RectangleF(UILayoutConstants.MainMenu.Position.X + margin,
                UILayoutConstants.MainMenu.Position.Y + margin,
                _menu.Size.X - margin * 2,
                _menu.Size.Y - margin * 2);
            DrawOutline(g);

            g.SetClip(clip);
            foreach (var button in buttons)
            {
                DrawButton(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
            }
            g.ResetClip();
        }
        
        private void DrawOutline(Graphics g)
        {
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                float margin = 3.0f;
                debugPen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(debugPen,
                UILayoutConstants.MainMenu.Position.X + margin,
                UILayoutConstants.MainMenu.Position.Y + margin,
                _menu.Size.X - margin * 2,
                _menu.Size.Y - margin * 2);
            }
        }
        private void DrawButton(Graphics g, string text, Vector2F position, Vector2F size)
        {
            IButtonDrawStyle style = UILayoutStyles.MainMenu.Button.Style;
            style.Draw(g, text, position, size);
        }
    }
}
