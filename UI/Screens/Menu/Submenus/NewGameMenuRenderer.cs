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

using Chinese_Chess_v3.Configs.Style;
using Chinese_Chess_v3.UI.Constants;

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
        private NewGameMenu menu;

        public NewGameMenuRenderer(NewGameMenu menu)
        {
            this.menu = menu;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var buttons = menu.Buttons;
            var clip = menu.GetAbsClipRect();

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
                UILayoutConstants.Submenu.Position.X + margin,
                UILayoutConstants.Submenu.Position.Y + margin,
                menu.Size.X - margin * 2,
                menu.Size.Y - margin * 2);
            }
        }

        private void DrawButton(Graphics g, string text, Vector2F position, Vector2F size)
        {
            IButtonDrawStyle style = UILayoutStyles.MainMenu.Button.Style;
            style.Draw(g, text, position, size);
        }
    }
}
