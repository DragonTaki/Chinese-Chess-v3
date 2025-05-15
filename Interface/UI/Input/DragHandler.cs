/* ----- ----- ----- ----- */
// DragHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Interface.UI.Input
{
    /// <summary>
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system.
    /// </summary>
    public class DragHandler : IInputHandler
    {
        /// <summary>Indicates whether the user is currently dragging.</summary>
        public bool IsDragging { get; private set; } = false;

        /// <summary>Indicates whether movement passed drag threshold.</summary>
        public bool HasMovedEnoughToDrag = false;
        public Point DragStartPoint { get; private set; }
        public Point LastMousePoint { get; private set; }
        public DateTime DragStartTime { get; private set; }
        private float totalDragDistance = 0.0f;

        /// <summary>Drag distance threshold (in pixels) to begin scrolling.</summary>
        public float DragThreshold { get; set; } = 5.0f;

        /// <summary>Maximum time (ms) under which tiny movement is treated as click.</summary>
        public float DragTimeThreshold { get; set; } = 160.0f;

        public event Action<Vector2F> OnDrag;     // (start, current)
        public event Action<Point> OnClick;           // if determined as click

        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            IsDragging = true;
            HasMovedEnoughToDrag = false;

            DragStartPoint = e.Location;
            LastMousePoint = e.Location;
            DragStartTime = DateTime.Now;
            
            totalDragDistance = 0.0f;

            return true;
        }

        /// <summary>
        /// Call on mouse move. ScrollDelta is updated only after threshold exceeded.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;

            float deltaX = e.X - LastMousePoint.X;
            float deltaY = e.Y - LastMousePoint.Y;
            Vector2F delta = new Vector2F(deltaX, deltaY);
            float deltaLength = MathF.Sqrt(deltaX * deltaX + deltaY * deltaY);
            totalDragDistance += deltaLength;

            // If move too small, don't give movement yet
            if (!HasMovedEnoughToDrag)
            {
                if (totalDragDistance >= DragThreshold)
                {
                    HasMovedEnoughToDrag = true;
                }
                else
                {
                    return false; // Do not start scrolling yet
                }
            }

            OnDrag?.Invoke(delta);
            LastMousePoint = e.Location;

            return true;
        }

        /// <summary>
        /// Call on mouse release. Computes inertial velocity if drag was valid.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            if (!IsDragging)
                return true;

            IsDragging = false;
            HasMovedEnoughToDrag = false;

            bool isClick = !HasMovedEnoughToDrag ||
                (Math.Abs(e.Y - DragStartPoint.Y) < DragThreshold && (DateTime.Now - DragStartTime).TotalMilliseconds < DragTimeThreshold);
            if (isClick)
            {
                // Treat as click: no velocity
                OnClick?.Invoke(e.Location);
            }
            return true;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public bool OnMouseClick(MouseEventArgs e)
        {
            return false;
        }

        public void EndFrame()
        {
            // Optionally override in subclass
        }
    }
}
