/* ----- ----- ----- ----- */
// MainMenuPanel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Interface.Controls;
using Chinese_Chess_v3.Interface.Renderers;

namespace Chinese_Chess_v3.Interface.Panels
{
    public class MainMenuPanel : Panel
    {
        private MainMenuRenderer renderer;
        private ScrollContainer scrollContainer;

        public MainMenuPanel()
        {
            this.DoubleBuffered = true;
            this.Location = Settings.MainMenu.Position.ToPoint();
            this.Size = Settings.MainMenu.Size.ToSize();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            

            renderer = new MainMenuRenderer();

            this.Paint += OnPaint;
            this.MouseClick += OnMouseClick;
        }

        public void Init()
        {
            renderer = new MainMenuRenderer();
            scrollContainer = new ScrollContainer();
        }

        public void Render(Graphics g)
        {
            //scrollContainer.ApplyClip(g); // 裁切可視區域
            //renderer.Draw(g, scrollContainer.ScrollOffset);
            //scrollContainer.ClearClip(g);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            //renderer.Draw(e.Graphics, this.ClientRectangle);
        }

        // Event handling
        public void OnMouseDown(MouseEventArgs e)
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