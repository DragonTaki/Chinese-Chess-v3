/* ----- ----- ----- ----- */
// MainMenuPanel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Controls;
using Chinese_Chess_v3.Data;
using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.Renderers;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Panels
{
    public class MainMenuPanel : Panel
    {
        private readonly MainMenuRenderer renderer;
        private readonly ScrollContainer scroll;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ButtonData> Buttons { get; private set; } = MainMenuButtonList.CreateDefaultButtons();

        public MainMenuPanel(MainMenuRenderer renderer, ScrollContainer scroll)
        {
            this.DoubleBuffered = true;
            this.Location = Settings.MainMenu.Position.ToPoint();
            this.Size = Settings.MainMenu.Size.ToSize();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

            this.renderer = renderer;
            this.scroll = scroll;

            //this.Paint += OnPaint;
            this.MouseClick += OnMouseClick;
        }

        public new void InitLayout()
        {
            scroll.ViewportBounds = new RectangleF(
                Settings.MainMenu.Button.Position.X,
                Settings.MainMenu.Button.Position.Y - Settings.MainMenu.Margin,
                Settings.MainMenu.Button.Size.X,
                Settings.MainMenu.Size.Y - Settings.MainMenu.Margin * 2);
            scroll.ContentHeight = Buttons.Count * (Settings.MainMenu.Button.Size.Y + Settings.MainMenu.Margin);

            float margin = Settings.MainMenu.Margin;
            Vector2F size = Settings.MainMenu.Button.Size;
            Vector2F basePosition = Settings.MainMenu.Button.Position;

            for (int i = 0; i < Buttons.Count; i++)
            {
                float y = basePosition.Y + i * (size.Y + margin);
                Buttons[i].Position = new Vector2F(basePosition.X, y);
            }
            scroll.ScrollY = -Settings.MainMenu.Margin;
        }

        public void UpdateScrollLayout()
        {
            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].RenderPosition = new Vector2F(
                    Buttons[i].Position.X,
                    Buttons[i].Position.Y + scroll.GetContentOffsetY());
            }
        }

        public List<ButtonData> GetVisibleButtons()
        {
            RectangleF view = scroll.GetClippingRect();
            return Buttons.Where(b =>
                b.RenderPosition.Y + Settings.MainMenu.Button.Size.Y > view.Top &&
                b.RenderPosition.Y < view.Bottom
            ).ToList();
        }

        // Event handling
        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            //scrollContainer.OnMouseDown(e);
        }
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // TODO: 判斷是否點選到按鈕區域，進行處理
            // 可透過 renderer 傳回的按鈕位置來比對點擊
        }
    }
}