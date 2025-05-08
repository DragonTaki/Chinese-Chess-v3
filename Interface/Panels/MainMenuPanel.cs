/* ----- ----- ----- ----- */
// MainMenuPanel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;
using Chinese_Chess_v3.Interface.Panels;
using Chinese_Chess_v3.Interface.Renderers;

namespace Chinese_Chess_v3.Interface.Panels
{
    public class MainMenuPanel : Panel
    {
        private MainMenuRenderer renderer;

        public MainMenuPanel()
        {
            this.DoubleBuffered = true;
            this.Dock = DockStyle.Fill;

            renderer = new MainMenuRenderer();

            this.Paint += OnPaint;
            this.MouseClick += OnMouseClick;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            renderer.Draw(e.Graphics, this.ClientRectangle);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // TODO: 判斷是否點選到按鈕區域，進行處理
            // 可透過 renderer 傳回的按鈕位置來比對點擊
        }
    }
}