/* ----- ----- ----- ----- */
// ScrollInputHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core;

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
        //Singleton
        private static readonly Lazy<ScrollInputHandler> _instance =
            new Lazy<ScrollInputHandler>(() => new ScrollInputHandler());

        public static ScrollInputHandler Instance => _instance.Value;

        private readonly List<ScrollTarget> scrollTargets = new();

#nullable enable
        private ScrollTarget? activeTarget = null;
#nullable disable

        private readonly DragHandler dragHandler;

        public bool IsDragging => dragHandler.IsDragging;

        private ScrollInputHandler()
        {
            dragHandler = new DragHandler();
            dragHandler.OnDrag += HandleDrag;
            {
                if (activeTarget?.Physics != null)
                    activeTarget.Physics.Velocity.Reset();
            };
        }

        public void RegisterScrollTarget(
            UIElement element,
            Physics2D physics,
            Func<RectangleF> viewportGetter,
            ScrollBehavior behavior = null,
            int zIndex = 0)
        {
            Console.WriteLine($"[Register] ScrollTarget: {element} ({element.GetHashCode()})");
            if (scrollTargets.Exists(t => t.Element == element)) return;

            scrollTargets.Add(new ScrollTarget
            {
                Element = element,
                Physics = physics,
                ViewportGetter = viewportGetter,
                Behavior = behavior ?? new ScrollBehavior()
            });
        }

        /// <summary>
        /// Returns true if currently dragging and mouse is inside the active scroll target.
        /// Used to optionally suppress UI interaction while dragging.
        /// </summary>
        public bool IsDraggingWithinActiveTarget(Point location)
        {
            return IsDragging &&
                activeTarget?.ViewportGetter().Contains(location) == true;
        }

        /// <summary>
        /// Call when mouse button is pressed to begin scroll detection.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            if (IsDragging)
            {
                dragHandler.OnMouseUp(e);
                activeTarget = null;
            }
            for (int i = scrollTargets.Count - 1; i >= 0; i--)
            {
                var target = scrollTargets[i];

                    Console.WriteLine($"[HitTest] target[{i}]: {target.Element}, {target.Element.IsVisible}, {target.Element.IsInteractable}");
                // If not IsVisible or not IsEnabled
                if (!target.Element.IsInteractable || target.Element.Parent == null)
                {
                    continue;
                }

                var bounds = target.ViewportGetter();
                if (bounds == RectangleF.Empty || !bounds.Contains(e.Location))
                    continue;

                var hitElement = target.Element.HitTestDeep(e.Location);
                if (hitElement != null)
                {
                    activeTarget = target;
                    dragHandler.OnMouseDown(e);
                    return true;
                }
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
            if (activeTarget.Physics == null) return;

            var b = activeTarget.Behavior;
            float dx = b.AllowDragX ? delta.X : 0;
            float dy = b.AllowDragY ? delta.Y : 0;

            activeTarget.Physics.Position.Current += new Vector2F(dx, dy);
            activeTarget.Physics.Velocity.Current = Vector2F.Zero;
        }

        /// <summary>
        /// Call on mouse wheels.
        /// </summary>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            if (activeTarget?.Physics == null || activeTarget?.Behavior?.AllowWheel != true)
                return false;

            activeTarget.Physics.Position.Current += new Vector2F(0, -e.Delta * 0.25f);
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
            if (activeTarget?.Physics == null) return;

            // Only reset delta if not dragging
            if (!IsDragging)
            {
                activeTarget.Physics.Velocity.Current = Vector2F.Zero;
            }
        }
        public void EndFrame() => ResetDelta();
        private class ScrollTarget
        {
            public UIElement Element;
            public Physics2D Physics;
            public Func<RectangleF> ViewportGetter;
            public ScrollBehavior Behavior;
        }

        public class ScrollBehavior
        {
            public bool AllowDragX { get; set; } = false;
            public bool AllowDragY { get; set; } = true;
            public bool AllowWheel { get; set; } = true;
        }
    }
}
