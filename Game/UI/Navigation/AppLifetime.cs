/* ----- ----- ----- ----- */
// AppLifetime.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Services;

using Engine.Platform;

namespace Chinese_Chess_v3.Game.UI.Navigation
{
    /// <summary>
    /// The UI's <see cref="IAppLifetime"/>: exits through <see cref="AppControl.ExitCallback"/>,
    /// which each launcher sets to close its own window / message loop.
    /// </summary>
    public sealed class AppLifetime : IAppLifetime
    {
        public void Exit() => AppControl.ExitCallback?.Invoke();
    }
}
