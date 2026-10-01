/* ----- ----- ----- ----- */
// StarAnimation.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using StarAnimation.Controllers;

using Engine.Platform;
using Engine.Timing;

namespace StarAnimation
{
    public class StarAnimationApp
    {
        private readonly MainRenderController _controller;
        private readonly bool _initialized = false;

        /// <summary>
        /// Initialize the StarAnimation module.
        /// Construct once before calling Update().
        /// </summary>
        public StarAnimationApp()
        {
            if (_initialized) return;

            _controller = new MainRenderController();
            ITimerProvider timer = GlobalTime.Timer;
            //timer.OnAnimationFrame += Update;

            _initialized = true;
        }

        /// <summary>
        /// Updates the internal animation state.
        /// Must be driven by the host each frame (the timer subscription in the
        /// constructor is currently commented out).
        /// </summary>
        public void Update()
        {
            _controller?.Update();
        }

        /// <summary>
        /// Optional: allows resizing the animation region (if window resizes).
        /// </summary>
        public void Resize(int width, int height)
        {
            _controller?.Resize(width, height);
        }

        /// <summary>
        /// Optional: Expose a render function if drawing is handled here.
        /// </summary>
        public void Render(IGraphics g)
        {
            _controller?.Render(g);
        }
    }
}