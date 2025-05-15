/* ----- ----- ----- ----- */
// UIElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.Interface.UI.Core
{
    public class UIElement
    {
        // If `Parent == null`, means the most top UI object
        #nullable enable
        public UIElement? Parent { get; set; }
        #nullable disable
        public List<UIElement> Children { get; } = new();

        public UIPosition LocalPosition { get; set; } = new UIPosition(Vector2F.Zero);
        public Vector2F Size { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;

        // Initialize when need animated calculation
        #nullable enable
        public Physics2D? Physics { get; set; }
        #nullable disable
     
        /// <summary>
        /// Get absolute position of this UI element.
        /// </summary>
        /// <returns>Absolute position of this UI element.</returns>
        public Vector2F GetAbsolutePosition()
        {
            if (Parent == null) return LocalPosition.Current;
            return Parent.GetAbsolutePosition() + LocalPosition.Current;
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

        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
        public virtual void OnMouseUp(MouseEventArgs e) { }
        public virtual void OnMouseWheel(MouseEventArgs e) { }
        public virtual void OnMouseClick(MouseEventArgs e) { }
    }

}