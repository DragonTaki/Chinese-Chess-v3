/* ----- ----- ----- ----- */
// IInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Windows.Forms;

namespace Chinese_Chess_v3.Interface.UI.Input
{
    /// <summary>
    /// Defines the contract for input handler modules. Each module processes specific input events.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Called when mouse button is pressed.
        /// </summary>
        void OnMouseDown(MouseEventArgs e);

        /// <summary>
        /// Called when mouse moves.
        /// </summary>
        void OnMouseMove(MouseEventArgs e);

        /// <summary>
        /// Called when mouse button is released.
        /// </summary>
        void OnMouseUp(MouseEventArgs e);

        /// <summary>
        /// Called when mouse wheel scrolls.
        /// </summary>
        void OnMouseWheel(MouseEventArgs e);

        /// <summary>
        /// Called when mouse clicks.
        /// </summary>
        void OnMouseClick(MouseEventArgs e);

        /// <summary>
        /// Called at the end of each frame to finalize per-frame state (e.g., reset delta).
        /// </summary>
        void EndFrame();
    }
}
