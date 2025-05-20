/* ----- ----- ----- ----- */
// MouseInputRouter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core.Elements;

namespace Chinese_Chess_v3.UI.Input
{
    /// <summary>
    /// Centralized mouse input router. Delegates mouse events to UI root and ScrollInputHandler.
    /// Supports unified control of drag, scroll, and click logic.
    /// </summary>
    public class MouseInputRouter : IInputHandler
    {
        /// <summary>The root UI element that will receive mouse events.</summary>
        public UIElement Root { get; set; }
        private readonly List<IInputHandler> _handlers = new();

        /// <summary>Optional scroll handler to control vertical scroll interaction.</summary>
        private readonly IScrollInputHandler _scrollHandler;

        /// <summary>If true, stops further UI mouse event delivery during scroll drag.</summary>
        public bool SuppressUIWhenDragging { get; set; } = true;
        private bool _dragStarted = false;
        private bool _hasDragged = false;
        private UIElement _pressedElement = null;

#nullable enable
        public MouseInputRouter(UIElement root, IScrollInputHandler? scroll = null)
#nullable disable
        {
            Root = root;
            _scrollHandler = scroll;

            if (_scrollHandler != null)
                _handlers.Add(_scrollHandler);
        }

        public void AddHandler(IInputHandler handler) => _handlers.Add(handler);

        /// <summary>Handle MouseDown: trigger scroll + UI input.</summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            _dragStarted = false;
            _hasDragged = false;
            _pressedElement = Root.HitTestDeep(e.Location);
            bool handled = false;

            // Than other mouse event _handlers
            foreach (var h in _handlers)
            {
                if (h.OnMouseDown(e))
                {
                    handled = true;
                    break;
                }
            }

            // Process UI mouse event first
            if (!handled && _pressedElement != null)
            {
                _pressedElement.OnMouseDown(e);
                return true;
            }

            return false;
        }

        /// <summary>Handle MouseMove: update scroll and forward to UI.</summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            // Event _handlers first, then we know if is dragging or not
            foreach (var h in _handlers)
            {
                if (h.OnMouseMove(e))
                {
                    break;
                }
            }

            if (SuppressUIWhenDragging)
            {
                if (!_dragStarted && _scrollHandler?.IsDragging == true && _scrollHandler.HasMovedEnoughToDrag())
                {
                    _hasDragged = true;
                    _dragStarted = true;
                    return true;
                }
                if (_dragStarted && _scrollHandler.IsDraggingWithinActiveTarget(e.Location))
                {
                    return true;
                }
            }

            // Than process UI mouse event
            if (Root.OnMouseMove(e))
            {
                return true;
            }

            return false;
        }

        /// <summary>Handle MouseUp: release drag and forward to UI.</summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            // Release scroll whatever
            foreach (var h in _handlers)
                h.OnMouseUp(e);

            bool blockClick = SuppressUIWhenDragging && _dragStarted && _hasDragged;

            if (_pressedElement != null)
            {
                _pressedElement.OnMouseUp(e);

                if (!blockClick && _pressedElement.HitTest(e.Location))
                    _pressedElement.OnMouseClick(e);

                _pressedElement = null;
            }

            _dragStarted = false;
            return true;
        }

        /// <summary>Handle mouse wheel scrolling (optional).</summary>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseWheel(e))
                return true;

            // Than other mouse event _handlers
            foreach (var h in _handlers)
                if (h.OnMouseWheel(e)) return true;

            return false;
        }

        public bool OnMouseClick(MouseEventArgs e)
        {
            if (!_dragStarted)
            {
                // Process UI mouse event first
                if (Root.OnMouseClick(e))
                    return true;

                // Than other mouse event _handlers
                foreach (var h in _handlers)
                    if (h.OnMouseClick(e)) return true;
            }

            return false;
        }

        /// <summary>Call every frame after processing input to reset scroll delta.</summary>
        public void EndFrame()
        {
            // Process UI mouse event first
            Root.EndFrame();

            foreach (var h in _handlers) h?.EndFrame();
        }
    }
}
