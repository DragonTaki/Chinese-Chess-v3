/* ----- ----- ----- ----- */
// IDrawable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

namespace Chinese_Chess_v3.UI.Core
{
    /// <summary>
    /// Interface for objects that require periodic draw calls, e.g. each frame.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Called once per frame or update cycle.
        /// </summary>
        void Draw(Graphics g);
    }
}