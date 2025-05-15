/* ----- ----- ----- ----- */
// ScrollInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Interface.UI.Input
{
    /// <summary>
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system.
    /// </summary>
    public class ScrollInputHandler : IInputHandler
    {
        /// <summary>Indicates whether the user is currently dragging.</summary>
        public bool IsDragging { get; private set; } = false;

        /// <summary>Indicates whether movement passed drag threshold.</summary>
        private bool hasMovedEnoughToDrag = false;

        /// <summary>Drag distance threshold (in pixels) to begin scrolling.</summary>
        public float DragThreshold { get; set; } = 5.0f;

        /// <summary>Maximum time (ms) under which tiny movement is treated as click.</summary>
        public float DragTimeThreshold { get; set; } = 160.0f;

        /// <summary>Total scroll offset accumulated during drag.</summary>
        public Vector2F ScrollDelta { get; private set; } = Vector2F.Zero;

        /// <summary>Velocity determined on mouse release (for inertia).</summary>
        public Vector2F ReleasedVelocity { get; private set; } = Vector2F.Zero;

        private float dragStartY = 0.0f;
        private float lastMouseY = 0.0f;
        private DateTime dragStartTime;

        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
        {
            IsDragging = true;
            hasMovedEnoughToDrag = false;

            dragStartY = e.Y;
            lastMouseY = e.Y;
            dragStartTime = DateTime.Now;

            ScrollDelta = Vector2F.Zero;
            ReleasedVelocity = Vector2F.Zero;
        }

        /// <summary>
        /// Call on mouse move. ScrollDelta is updated only after threshold exceeded.
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!IsDragging)
                return;

            float totalDeltaY = e.Y - dragStartY;

            // If move too small, don't give movement yet
            if (!hasMovedEnoughToDrag)
            {
                if (Math.Abs(totalDeltaY) >= DragThreshold)
                {
                    hasMovedEnoughToDrag = true;
                }
                else
                {
                    return; // Do not start scrolling yet
                }
            }

            float deltaY = e.Y - lastMouseY;

            ScrollDelta -= new Vector2F(0, deltaY);  // Subtract: upward drag means upward scroll
            lastMouseY = e.Y;

            // No ReleasedVelocity yet during drag
            ReleasedVelocity = Vector2F.Zero;
        }

        /// <summary>
        /// Call on mouse release. Computes inertial velocity if drag was valid.
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            if (!IsDragging)
                return;

            IsDragging = false;

            float totalDeltaY = e.Y - dragStartY;
            TimeSpan duration = DateTime.Now - dragStartTime;

            if (!hasMovedEnoughToDrag ||
                (Math.Abs(totalDeltaY) < DragThreshold && duration.TotalMilliseconds < DragTimeThreshold))
            {
                // Treat as click: no velocity
                ReleasedVelocity = Vector2F.Zero;
                ScrollDelta = Vector2F.Zero;
            }
            else
            {
                float deltaY = e.Y - lastMouseY;
                ReleasedVelocity = new Vector2F(0, -deltaY); // Negative: same as scroll direction
            }
        }

        /// <summary>
        /// Call on mouse wheels.
        /// </summary>
        public void OnMouseWheel(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Call on mouse clicks.
        /// </summary>
        public void OnMouseClick(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Call at the end of frame to reset scroll delta after applying.
        /// </summary>
        public void ResetDelta()
        {
            // Reset delta
            ScrollDelta = Vector2F.Zero;
        }
        public void EndFrame() => ResetDelta();
    }
}
