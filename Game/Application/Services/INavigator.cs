/* ----- ----- ----- ----- */
// INavigator.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// Switches the app between its top-level screens by id, so game logic can navigate
    /// without depending on the UI types of the screens.
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Shows <paramref name="screen"/> in place of the current screen (creating it on first
        /// use); showing the screen already on display leaves it as it is.
        /// </summary>
        /// <param name="screen">The screen to show.</param>
        void Show(ScreenId screen);
    }
}
