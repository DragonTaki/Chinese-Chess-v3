/* ----- ----- ----- ----- */
// MainRenderController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using StarAnimation.Renderers;

using SharedLib.RandomTable;
using SharedLib.Timing;

namespace StarAnimation.Controllers
{
    /// <summary>
    /// Central rendering controller that manages and coordinates all visual components.
    /// </summary>
    public class MainRenderController
    {
        /// <summary>
        /// Renderer responsible for drawing and updating star particles.
        /// </summary>
        private readonly StarRenderer starRenderer;

        /// <summary>
        /// Controller that applies visual effects and overlays on the starfield.
        /// </summary>
        private readonly StarEffectController effectController;

        /// <summary>
        /// Optional background renderer (static or animated).
        /// </summary>
        private readonly BackgroundRenderer backgroundRenderer;

        /// <summary>
        /// Star animation timer.
        /// </summary>
        private readonly ITimerProvider timer;

        /// <summary>
        /// Shared random instance for consistent visual randomness.
        /// </summary>
        private readonly RandomTable rand;

        /// <summary>
        /// Initializes the main render controller and its internal components.
        /// </summary>
        /// <param name="rand">Random number generator used for effects and randomness.</param>
        /// <param name="width">Width of the rendering canvas.</param>
        /// <param name="height">Height of the rendering canvas.</param>
        public MainRenderController(int width, int height, ITimerProvider timerProvider, RandomTable GlobalRandomTable)
        {
            this.rand = GlobalRandomTable;

            // Initialize all renderers and controllers
            starRenderer = new StarRenderer(width, height);
            effectController = new StarEffectController(width, height);
            backgroundRenderer = new BackgroundRenderer(width, height);

            // Initialize timer
            timer = timerProvider;
            timer.OnAnimationFrame += Update;
        }

        /// <summary>
        /// Updates the state of all rendering components.
        /// </summary>
        public void Update()
        {
            starRenderer.Update();
            effectController.Update(starRenderer.GetStars(), null); // Optional: pass Graphics if needed for region logic
        }

        /// <summary>
        /// Renders all visual layers onto the provided Graphics context.
        /// </summary>
        /// <param name="g">The Graphics object to draw onto.</param>
        public void Draw(Graphics g)
        {
            if (g == null) return;

            backgroundRenderer.Draw(g);
            starRenderer.Draw(g);
            effectController.Draw(g);
        }

        /// <summary>
        /// Resizes all internal renderers to match a new screen size.
        /// </summary>
        /// <param name="width">New screen width.</param>
        /// <param name="height">New screen height.</param>
        public void Resize(int width, int height)
        {
            starRenderer.Resize(width, height);
            backgroundRenderer.Resize(width, height);
            // effectController usually doesn't depend on size unless region-based logic is added
        }
    }
}