/* ----- ----- ----- ----- */
// BackgroundController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using StarAnimation.Renderers;

namespace StarAnimation.Controllers
{
    public class BackgroundController
    {
        private readonly int width;
        private readonly int height;
        private readonly BackgroundRenderer renderer;
        public BackgroundController(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            renderer = new BackgroundRenderer(width, height);
        }

        public void Resize(int width, int height)
        {
            renderer.Resize(width, height);
        }

        public void Update()
        {

        }

        public void Draw(Graphics g)
        {
            renderer.Draw(g);
        }
    }
}