/* ----- ----- ----- ----- */
// MouseInputRouter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core;

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
        private readonly List<IInputHandler> handlers = new();

        /// <summary>Optional scroll handler to control vertical scroll interaction.</summary>
        private readonly ScrollInputHandler scrollHandler;

        /// <summary>If true, stops further UI mouse event delivery during scroll drag.</summary>
        public bool SuppressUIWhenDragging { get; set; } = true;

        public MouseInputRouter(UIElement root = null)
        {
            this.Root = root;
            this.scrollHandler = ScrollInputHandler.Instance;

            this.handlers = new List<IInputHandler>();
            if (scrollHandler != null)
                this.handlers.Add(scrollHandler);
        }

        public void AddHandler(IInputHandler handler) => handlers.Add(handler);

        /// <summary>Handle MouseDown: trigger scroll + UI input.</summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseDown(e))
                return true;

            // Than other mouse event handlers
            foreach (var h in handlers)
                if (h.OnMouseDown(e)) return true;

            return false;
        }

        /// <summary>Handle MouseMove: update scroll and forward to UI.</summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseMove(e))
            {
                //Console.WriteLine("[Router] Consumed by UI.");
                return true;
            }

            if (SuppressUIWhenDragging && scrollHandler?.IsDragging == true)
            {
                if (scrollHandler.IsDraggingWithinActiveTarget(e.Location))
                    return false;
            }

            // Than other mouse event handlers
            foreach (var h in handlers)
            {
                if (h.OnMouseMove(e))
                {
                    //Console.WriteLine("[Router] Consumed by handler.");
                    return true;
                }
            }

            return false;
        }

        /// <summary>Handle MouseUp: release drag and forward to UI.</summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseUp(e))
                return true;

            // Than other mouse event handlers
            foreach (var h in handlers)
                if (h.OnMouseUp(e)) return true;

            return false;
        }

        /// <summary>Handle mouse wheel scrolling (optional).</summary>
        public bool OnMouseWheel(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseWheel(e))
                return true;

            // Than other mouse event handlers
            foreach (var h in handlers)
                if (h.OnMouseWheel(e)) return true;

            return false;
        }

        public bool OnMouseClick(MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseClick(e))
                return true;

            // Than other mouse event handlers
            foreach (var h in handlers)
                if (h.OnMouseClick(e)) return true;

            return false;
        }

        /// <summary>Call every frame after processing input to reset scroll delta.</summary>
        public void EndFrame()
        {
            // Process UI mouse event first
            Root.EndFrame();

            foreach (var h in handlers) h?.EndFrame();
        }
    }
}
