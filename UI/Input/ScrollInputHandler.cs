/* ----- ----- ----- ----- */
// ScrollInputHandler.cs
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
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.UI.Input
{
    /// <summary>
    /// Handles mouse-based scroll input, including drag detection, threshold filtering,
    /// and inertial velocity output. This class does not directly update any physics system.
    /// </summary>
    public class ScrollInputHandler : IInputHandler
    {
#nullable enable
        private Physics2D? physics;
        private Func<RectangleF>? viewportGetter;
#nullable disable
        
        private readonly DragHandler dragHandler;
        public bool IsDragging => dragHandler.IsDragging;

        public ScrollInputHandler()
        {
            dragHandler = new DragHandler();
            dragHandler.OnDrag += HandleDrag;
            dragHandler.OnClick += _ => physics?.Velocity.Reset();
        }

        public void Bind(Physics2D physics, Func<RectangleF> viewportGetter)
        {
            this.physics = physics;
            this.viewportGetter = viewportGetter;
        }
        
        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            if (viewportGetter?.Invoke().Contains(e.Location) == true)
            {
                dragHandler.OnMouseDown(e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call on mouse move. ScrollDelta is updated only after threshold exceeded.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            dragHandler.OnMouseMove(e);
            return true;
        }

        /// <summary>
        /// Call on mouse release. Computes inertial velocity if drag was valid.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            dragHandler.OnMouseUp(e);
            return true;
        }

        private void HandleDrag(Vector2F delta)
        {
            if (physics == null) return;

            physics.Position.Current += new Vector2F(0, delta.Y);
            physics.Velocity.Current = Vector2F.Zero;
        }

        /// <summary>
        /// Call on mouse wheels.
        /// </summary>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            if (physics == null) return false;

            physics.Position.Current += new Vector2F(0, -e.Delta * 0.25f);
            return true;
        }

        /// <summary>
        /// Call on mouse clicks.
        /// </summary>
        public bool OnMouseClick(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Call at the end of frame to reset scroll delta after applying.
        /// </summary>
        public void ResetDelta()
        {
            if (physics == null) return;

            // Only reset delta if not dragging
            if (!IsDragging)
            {
                physics.Velocity.Current = Vector2F.Zero;
            }
        }
        public void EndFrame() => ResetDelta();
    }
}
