/* ----- ----- ----- ----- */
// IAppLifetime.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// The application's lifetime, so game logic can close the app (離開遊戲) without
    /// depending on the window or platform that runs it.
    /// </summary>
    public interface IAppLifetime
    {
        /// <summary>Closes the application (its window and message loop).</summary>
        void Exit();
    }
}
