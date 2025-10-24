/* ----- ----- ----- ----- */
// MainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Game.Constants.UI;

using Engine.Styles;
using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus
{
    public class MainMenuRenderer
    {
        private readonly MainMenu _menu;

        public MainMenuRenderer(MainMenu menu)
        {
            _menu = menu;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 取得可見按鈕
            var buttons = _menu.GetVisibleButtons();
            var clip = _menu.GetAbsClipRect();

            DrawOutline(g);

            g.SetClip(clip);
            foreach (var button in buttons)
            {
                DrawButton(g, button);
            }
            g.ResetClip();
        }
        
        private void DrawOutline(Graphics g)
        {
            // -----------------------------
            // 舊版 MainMenu 依賴常數檔案繪製
            // -----------------------------
            /*
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
            */

            // 新版：依 UIMenu 自身的尺寸與位置繪製
            using (Pen debugPen = new Pen(Color.FromArgb(100, 128, 128, 128), 4))
            {
                float margin = 3.0f;
                debugPen.DashStyle = DashStyle.Dash;
                var pos = _menu.LocalPosition;
                g.DrawRectangle(debugPen,
                    pos.Current.X + margin,
                    pos.Current.Y + margin,
                    _menu.Size.X - margin * 2,
                    _menu.Size.Y - margin * 2);
            }
        }
        private void DrawButton(Graphics g, UIButton button)
        {
            // -----------------------------
            // 舊版依靠 Text, Vector2F, Size 參數
            // -----------------------------
            /*
            DrawButton(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
            */

            // 新版直接使用 UIButton 內建屬性
            IButtonDrawStyle style = button.Style ?? UILayoutStyles.MainMenu.Button.Style;
            style.Draw(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
        }
    }
}
