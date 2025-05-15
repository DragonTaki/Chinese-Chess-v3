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
        public void OnMouseDown(MouseEventArgs e)
        {
            panel.OnMouseDown(e);
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            panel.OnMouseMove(e);
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            panel.OnMouseUp(e);
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            panel.OnMouseWheel(e);
        }

        public void OnClick(PointF location)
        {
        }
    }
}
