/* ----- ----- ----- ----- */
// UIElement.cs
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

using Chinese_Chess_v3.UI.Input;
using SharedLib.Geometry;
using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.UI.Core
{
    public class UIElement : IUpdatable, IDrawable, IInputHandler
    {
        // If `Parent == null`, means the most top UI object
#nullable enable
        public UIElement? Parent { get; set; }
#nullable disable
        public List<UIElement> Children { get; } = new();

        public virtual UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);
        public virtual Vector2F Size { get; set; } = Vector2F.Zero;
        public virtual LayoutF Layout
        {
            get => new LayoutF(LocalPosition.Current, Size);
            set
            {
                LocalPosition = new UIPosition(value.Position);
                Size = value.Size;
            }
        }
        /// <summary>
        /// Is it logically visible (use for UI display and HitTest).
        /// `IsVisible = false` means that the object is not drawn and cannot be clicked.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Whether it can interact with the mouse (click, drag, etc.) or keybord.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether it can interact with the mouse (click, drag, etc.) or keybord.
        /// </summary>
        public bool IsInteractable => IsVisible && IsEnabled;

        /// <summary>
        /// Is it still possible to HitTest even if `IsVisible == false` (e.g. transparent mask).
        /// </summary>
        public virtual bool AllowHitWhenInvisible => false;

        /// <summary>
        /// Only controls whether the element is rendered (can be overridden).
        /// When `IsVisible = false`, it will not be drawn automatically, but some objects can be forced not to be drawn.
        /// </summary>
        public virtual bool DisableRender => !IsVisible;

        // Initialize when need animated calculation
#nullable enable
        public virtual Physics2D? Physics { get; set; }
#nullable disable

        /// <summary>
        /// Calculate absolute coordinates: Base + offsets of all ancestors
        /// </summary>
        public Vector2F GetBaseAbsolutePosition()
        {
            Vector2F pos = LocalPosition.Base;
            while (Parent != null)
            {
                pos += Parent.LocalPosition.Base;
                Parent = Parent.Parent;
            }
            return pos;
        }
        
        /// <summary>
        /// Get absolute position of this UI element.
        /// </summary>
        /// <returns>Absolute position of this UI element.</returns>
        public Vector2F GetCurrentAbsolutePosition()
        {
            Vector2F accumulated = LocalPosition.Current;

#nullable enable
            UIElement? current = this.Parent;
#nullable disable
            
            while (current != null)
            {
                // Once a parent object with Physics2D is encountered, the position is the absolute anchor point and recursion stops
                if (current.Physics != null)
                {
                    accumulated += current.Physics.Position.Current;
                    break;
                }

                accumulated += current.LocalPosition.Current;
                current = current.Parent;
            }

            return accumulated;
        }
        
        /// <summary>
        /// Get absolute bounds of this UI element.
        /// </summary>
        /// <returns>Absolute bounds of this UI element.</returns>
        public RectangleF GetCurrentAbsoluteBounds()
        {
            return new RectangleF(GetCurrentAbsolutePosition().ToPointF(), Size.ToSizeF());
        }

        /// <summary>
        /// Checks if a screen-space point is within the bounds of this UI element.
        /// </summary>
        /// <param name="screenPoint">The point in global/screen coordinates.</param>
        /// <returns>True if the point is inside this element's bounds.</returns>
        public virtual bool ContainsScreenPoint(Vector2F screenPoint)
        {
            var absPos = this.GetCurrentAbsolutePosition();  // Full resolved screen-space position
            return screenPoint.X >= absPos.X &&
                screenPoint.X <= absPos.X + Size.X &&
                screenPoint.Y >= absPos.Y &&
                screenPoint.Y <= absPos.Y + Size.Y;
        }

        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
            // Update child's phisics2D absolute position
            child.OnAddedToParent();
        }

        protected virtual void OnAddedToParent()
        {
            if (Physics != null)
                Physics.Position = GetCurrentAbsolutePosition();
        }

        public virtual void RemoveChild(UIElement child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        /// <summary>
        /// Determines whether the given point is within the visible area of ​​this UI element.
        /// </summary>
        public virtual bool HitTest(PointF point)
        {
            if (!IsEnabled)
                return false;

            // Skip if it is not visible and transparent hits are not allowed
            if (!IsVisible && !AllowHitWhenInvisible)
                return false;

            return GetCurrentAbsoluteBounds().Contains(point);
        }

        /// <summary>
        /// Starting from this element, search the sub-elements depth-first to find the top-level UIElement that hits the point
        /// </summary>
#nullable enable
        public UIElement? HitTestDeep(PointF point)
#nullable disable
        {
            // If no hit, return `null`

            // Check self
            if (!HitTest(point))
                return null;

            // Check ancestors
            var current = this.Parent;
            while (current != null)
            {
                if (!current.IsVisible && !current.AllowHitWhenInvisible)
                    return null;
                current = current.Parent;
            }

            // Search from the last child element forward (elements with higher ZIndex are at the back)
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                var child = Children[i];
                if (!child.HitTest(point))
                    continue;

                var hit = child.HitTestDeep(point);
                if (hit != null)
                    return hit;
            }

            // If none of the child elements are hit, return self
            return this;
        }

        public virtual void Update()
        {
            Physics?.SmoothUpdate();
            OnUpdate();

            foreach (var child in Children)
                child.Update();
        }

        protected virtual void OnUpdate() { }

        public virtual void Draw(Graphics g)
        {
            if (DisableRender)
                return;

            OnDraw(g);

            foreach (var child in Children)
                if (child.IsVisible)
                    child.Draw(g);
        }
        
        protected virtual void OnDraw(Graphics g) { }

        // Mouse event handling
        protected bool PropagateMouseEvent(MouseEventArgs e, UIEventType eventName)
        {
            bool isInside = IsInteractable && GetCurrentAbsoluteBounds().Contains(e.Location);

            // Propagate to child
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                bool handled = eventName switch
                {
                    UIEventType.MouseClick => Children[i].OnMouseClick(e),
                    UIEventType.MouseDown => Children[i].OnMouseDown(e),
                    UIEventType.MouseMove => Children[i].OnMouseMove(e),
                    UIEventType.MouseUp => Children[i].OnMouseUp(e),
                    UIEventType.MouseWheel => Children[i].OnMouseWheel(e),
                    _ => false
                };

                if (handled)
                    return true;
            }

            switch (eventName)
            {
                // These events must be within the area
                case UIEventType.MouseDown:
                case UIEventType.MouseWheel:
                case UIEventType.MouseClick:
                    if (!isInside) return false;
                    break;

                // No need to detect the area
                case UIEventType.MouseMove:
                case UIEventType.MouseUp:
                    break;
            }

            // Self handling
            return eventName switch
            {
                UIEventType.MouseClick => HandleMouseClick(e),
                UIEventType.MouseDown => HandleMouseDown(e),
                UIEventType.MouseMove => HandleMouseMove(e),
                UIEventType.MouseUp => HandleMouseUp(e),
                UIEventType.MouseWheel => HandleMouseWheel(e),
                _ => false
            };
        }

        public virtual bool OnMouseDown(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseDown);
        }
        protected virtual bool HandleMouseDown(MouseEventArgs e)
        {
            return false; // The default is not to process, and the subclass can return true to indicate successful processing
        }

        public virtual bool OnMouseMove(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseMove);
        }
        protected virtual bool HandleMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseUp(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseUp);
        }
        protected virtual bool HandleMouseUp(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseWheel(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, UIEventType.MouseWheel);
        }
        protected virtual bool HandleMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseClick(MouseEventArgs e)
        {
            //return false;
            return PropagateMouseEvent(e, UIEventType.MouseClick);
        }
        protected virtual bool HandleMouseClick(MouseEventArgs e)
        {
            return false;
        }

        public virtual void EndFrame()
        {
            // Optionally override in subclass
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    child.EndFrame();
                }
            }
        }
    }

}