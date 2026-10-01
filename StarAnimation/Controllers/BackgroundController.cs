/* ----- ----- ----- ----- */
// BackgroundController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */


using Engine.Platform;
using StarAnimation.Renderers;

namespace StarAnimation.Controllers
{
    public class BackgroundController
    {
        private readonly int _width;
        private readonly int _height;
        private readonly BackgroundRenderer _renderer;
        public BackgroundController(int width, int height)
        {
            _width = width;
            _height = height;
            
            _renderer = new BackgroundRenderer(_width, _height);
        }

        public void Resize(int width, int height)
        {
            _renderer.Resize(width, height);
        }

        public void Update()
        {

        }

        public void Draw(IGraphics g)
        {
            _renderer.Draw(g);
        }
    }
}