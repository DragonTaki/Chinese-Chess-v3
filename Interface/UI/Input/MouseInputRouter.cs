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

using Chinese_Chess_v3.Interface.UI.Core;

namespace Chinese_Chess_v3.Interface.UI.Input
{
    /// <summary>
    /// Centralized mouse input router. Delegates mouse events to UI root and ScrollInputHandler.
    /// Supports unified control of drag, scroll, and click logic.
    /// </summary>
    public class MouseInputRouter
    {
        /// <summary>The root UI element that will receive mouse events.</summary>
        public UIElement Root { get; set; }
        private readonly List<IInputHandler> handlers = new();

        /// <summary>Optional scroll handler to control vertical scroll interaction.</summary>
        private readonly ScrollInputHandler scrollHandler;

        /// <summary>If true, stops further UI mouse event delivery during scroll drag.</summary>
        public bool SuppressUIWhenDragging { get; set; } = true;

        public MouseInputRouter(UIElement root = null, ScrollInputHandler scrollHandler = null)
        {
            this.Root = root;
            this.scrollHandler = scrollHandler;
            
            this.handlers = new List<IInputHandler>();
            if (scrollHandler != null)
                this.handlers.Add(scrollHandler);
        }

        public void AddHandler(IInputHandler handler) => handlers.Add(handler);

        /// <summary>Handle MouseDown: trigger scroll + UI input.</summary>
        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseDown(e))
                return;
                
            // Than other mouse event handlers
            foreach (var h in handlers)
            {
                h.OnMouseDown(e);
            }
        }

        /// <summary>Handle MouseMove: update scroll and forward to UI.</summary>
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseMove(e))
                return;
                
            // Than other mouse event handlers
            foreach (var h in handlers)
            {
                if (SuppressUIWhenDragging && scrollHandler?.IsDragging == true)
                    return;

                h.OnMouseMove(e);
            }
        }

        /// <summary>Handle MouseUp: release drag and forward to UI.</summary>
        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseUp(e))
                return;
                
            // Than other mouse event handlers
            foreach (var h in handlers) h.OnMouseUp(e);
        }

        /// <summary>Handle mouse wheel scrolling (optional).</summary>
        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseWheel(e))
                return;
                
            // Than other mouse event handlers
            foreach (var h in handlers) h.OnMouseWheel(e);
        }

        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            // Process UI mouse event first
            if (Root.OnMouseClick(e))
                return;
                
            // Than other mouse event handlers
            foreach (var h in handlers) h.OnMouseClick(e);
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
