/* ----- ----- ----- ----- */
// StarAnimation.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using StarAnimation.Controllers;

using SharedLib.Timing;

namespace StarAnimation
{
    public class StarAnimationApp
    {
        private MainRenderController controller;
        private bool initialized = false;

        /// <summary>
        /// Initialize the StarAnimation module.
        /// Must be called once before Update().
        /// </summary>
        public StarAnimationApp()
        {
            if (initialized) return;

            controller = new MainRenderController();
            ITimerProvider timer = GlobalTime.Timer;
            timer.OnAnimationFrame += Update;

            initialized = true;
        }

        /// <summary>
        /// Updates the internal animation state.
        /// Will be called automatically by timer.
        /// </summary>
        public void Update()
        {
            controller?.Update();
        }

        /// <summary>
        /// Optional: allows resizing the animation region (if window resizes).
        /// </summary>
        public void Resize(int width, int height)
        {
            controller?.Resize(width, height);
        }

        /// <summary>
        /// Optional: Expose a render function if drawing is handled here.
        /// </summary>
        public void Render(Graphics g)
        {
            controller?.Render(g);
        }
    }
}