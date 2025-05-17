/* ----- ----- ----- ----- */
// MainRenderController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.Globals;
using SharedLib.Timing;

namespace StarAnimation.Controllers
{
    /// <summary>
    /// Central rendering controller that manages and coordinates all visual components.
    /// </summary>
    public class MainRenderController
    {
        /// <summary>
        /// Optional background renderer (static or animated).
        /// </summary>
        private readonly BackgroundController backgroundController;

        /// <summary>
        /// Controller that applies visual effects and overlays on the starfield.
        /// </summary>
        private readonly EffectController effectController;

        /// <summary>
        /// Renderer responsible for drawing and updating star particles.
        /// </summary>
        private readonly StarController starController;

        /// <summary>
        /// Star animation timer.
        /// </summary>
        private readonly ITimerProvider timer;


        /// <summary>
        /// Initializes the main render controller and its internal components.
        /// </summary>
        /// <param name="rand">Random number generator used for effects and randomness.</param>
        /// <param name="width">Width of the rendering canvas.</param>
        /// <param name="height">Height of the rendering canvas.</param>
        public MainRenderController()
        {
            int width = GlobalWindow.Width;
            int height = GlobalWindow.Height;

            timer = GlobalTime.Timer;

            // Initialize all renderers and controllers
            backgroundController = new BackgroundController(width, height);
            starController = new StarController(width, height);
            effectController = new EffectController(width, height, starController);

            // Register update event
            //timer.OnAnimationFrame += Update;
        }

        /// <summary>
        /// Updates the state of all rendering components.
        /// </summary>
        public void Update()
        {
            backgroundController.Update();
            effectController.Update();
            starController.Update();
        }

        /// <summary>
        /// Renders all visual layers onto the provided Graphics context.
        /// </summary>
        /// <param name="g">The Graphics object to draw onto.</param>
        public void Render(Graphics g)
        {
            if (g == null) return;

            backgroundController.Draw(g);
            effectController.Draw(g);
            starController.Draw(g);
        }

        /// <summary>
        /// Resizes all internal renderers to match a new screen size.
        /// </summary>
        /// <param name="width">New screen width.</param>
        /// <param name="height">New screen height.</param>
        public void Resize(int width, int height)
        {
            backgroundController.Resize(width, height);
            effectController.Resize(width, height);
            starController.Resize(width, height);
        }
    }
}