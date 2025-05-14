/* ----- ----- ----- ----- */
// MainMenuController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Controls;
using Chinese_Chess_v3.Panels;
using Chinese_Chess_v3.Renderers;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Controllers
{
    public class MainMenuController
    {
        private readonly int width;
        private readonly int height;
        private readonly MainMenuRenderer renderer;
        private readonly MainMenuPanel panel;
        private readonly ScrollContainer scroll;

        public MainMenuController(int width, int height)
        {
            this.width = width;
            this.height = height;

            renderer = new MainMenuRenderer(width, height);
            scroll = new ScrollContainer();
            panel = new MainMenuPanel(renderer, scroll);
        }
        
        public void Initialize()
        {
            panel.InitLayout();
        }

        public void Render(Graphics g)
        {
            scroll.Update();
            panel.UpdateScrollLayout();
            renderer.Draw(g, panel.GetVisibleButtons(), scroll.GetClippingRect());
        }

        public void OnMouseDown(PointF location) => scroll.OnMouseDown(location);
        public void OnMouseMove(PointF location) => scroll.OnMouseMove(location);
        public void OnMouseUp() => scroll.OnMouseUp();
        public void OnMouseWheel(MouseEventArgs e) => scroll.OnMouseWheel(e);

        public void OnClick(PointF location)
        {/*
            var offset = new Vector2F(location.X, location.Y + Scroll.ScrollY);
            foreach (var btn in Buttons)
            {
                if (btn.Bounds.Contains(offset))
                    btn.OnClick?.Invoke();
            }*/
        }
    }
}
