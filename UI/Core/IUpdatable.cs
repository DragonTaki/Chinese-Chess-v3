/* ----- ----- ----- ----- */
// IUpdatable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Core
{
    /// <summary>
    /// Interface for objects that require periodic update calls, e.g. each frame.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Called once per frame or update cycle.
        /// </summary>
        void Update();
    }
}