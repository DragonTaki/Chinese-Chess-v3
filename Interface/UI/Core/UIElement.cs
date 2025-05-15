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

using Chinese_Chess_v3.Interface.UI.Input;

using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.Interface.UI.Core
{
    public class UIElement : IInputHandler
    {
        // If `Parent == null`, means the most top UI object
#nullable enable
        public UIElement? Parent { get; set; }
#nullable disable
        public List<UIElement> Children { get; } = new();

        public virtual UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);
        public virtual Vector2F Size { get; set; } = Vector2F.Zero;

        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        // Initialize when need animated calculation
#nullable enable
        public Physics2D? Physics { get; set; }
#nullable disable

        /// <summary>
        /// Get the local position that should actually be used for drawing and positioning.
        /// If there is a Physics2D, its position is used first.
        /// </summary>
        public virtual Vector2F ComputedLocalPosition =>
            Physics?.Position.Current ?? LocalPosition.Current;

        /// <summary>
        /// Get absolute position of this UI element.
        /// </summary>
        /// <returns>Absolute position of this UI element.</returns>
        public Vector2F GetAbsolutePosition()
        {
            if (Parent == null) return ComputedLocalPosition;
            return Parent.GetAbsolutePosition() + ComputedLocalPosition;
        }

        /// <summary>
        /// Get absolute bounds of this UI element.
        /// </summary>
        /// <returns>Absolute bounds of this UI element.</returns>
        public RectangleF GetAbsoluteBounds()
        {
            return new RectangleF(GetAbsolutePosition().ToPointF(), Size.ToSizeF());
        }

        /// <summary>
        /// Checks if a screen-space point is within the bounds of this UI element.
        /// </summary>
        /// <param name="screenPoint">The point in global/screen coordinates.</param>
        /// <returns>True if the point is inside this element's bounds.</returns>
        public virtual bool ContainsScreenPoint(Vector2F screenPoint)
        {
            var absPos = this.GetAbsolutePosition();  // Full resolved screen-space position
            return screenPoint.X >= absPos.X &&
                screenPoint.X <= absPos.X + Size.X &&
                screenPoint.Y >= absPos.Y &&
                screenPoint.Y <= absPos.Y + Size.Y;
        }

        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public virtual void Update()
        {
            Physics?.SmoothUpdate();
            foreach (var child in Children)
                child.Update();
        }

        public virtual void Draw(Graphics g)
        {
            foreach (var child in Children)
                if (child.IsVisible)
                    child.Draw(g);
        }

        // Mouse event handling
        protected bool PropagateMouseEvent(MouseEventArgs e, Func<UIElement, MouseEventArgs, bool> handler, string eventName)
        {
            bool isInside = IsVisible && GetAbsoluteBounds().Contains(e.Location);

            switch (eventName)
            {
                case "MouseDown":
                case "Click":
                case "MouseWheel":
                    if (!isInside) return false;  // 這些事件必須在區域內
                    break;

                case "MouseMove":
                case "MouseUp":
                    // 移動和放開不用判斷區域
                    break;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (handler(Children[i], e))
                    return true;
            }

            return handler(this, e);  // Call self
        }

        public virtual bool OnMouseDown(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, (element, ev) => element.HandleMouseDown(ev), "MouseDown");
        }
        protected virtual bool HandleMouseDown(MouseEventArgs e)
        {
            return false; // The default is not to process, and the subclass can return true to indicate successful processing
        }

        public virtual bool OnMouseMove(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, (element, ev) => element.HandleMouseMove(ev), "MouseMove");
        }
        protected virtual bool HandleMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseUp(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, (element, ev) => element.HandleMouseUp(ev), "MouseUp");
        }
        protected virtual bool HandleMouseUp(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseWheel(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, (element, ev) => element.HandleMouseWheel(ev), "MouseWheel");
        }
        protected virtual bool HandleMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public virtual bool OnMouseClick(MouseEventArgs e)
        {
            return PropagateMouseEvent(e, (element, ev) => element.HandleMouseClick(ev), "MouseClick");
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