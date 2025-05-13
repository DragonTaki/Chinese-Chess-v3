/* ----- ----- ----- ----- */
// ScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chinese_Chess_v3.Interface.Controls
{
    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia and border elasticity
    /// </summary>
    public class ScrollContainer
    {
        public RectangleF ViewportBounds { get; set; }
        public float ContentHeight { get; set; }

        public float ScrollY { get; private set; } = 0.0f;
        private float velocity = 0.0f;
        private const float Damping = 0.9f;
        private const float OverscrollSpring = 0.25f;
        private const float MaxOverscroll = 80.0f;

        private bool dragging = false;
        private float dragStartY;
        private float dragStartScroll;

        public bool EnableWheel = true;
        public bool EnableDrag = true;

        /// <summary>
        /// 更新每一幀動畫位置，建議於 Timer 或 Draw 事件內呼叫
        /// </summary>
        public void Update()
        {
            //ApplyInertia();
            //ApplyBounceEffect();
            if (!dragging)
            {
                ScrollY += velocity;
                velocity *= Damping;

                float overscroll = GetOverscroll();
                if (overscroll != 0)
                {
                    velocity -= overscroll * OverscrollSpring;
                }
            }
        }

        /// <summary>
        /// Mouse down event handler
        /// </summary>
        public void OnMouseWheel(MouseEventArgs e)
        {
            if (!EnableWheel) return;
            velocity -= e.Delta * 0.25f;
        }
        public void OnMouseDown(Point location)
        {
            if (!EnableDrag || !ViewportBounds.Contains(location)) return;

            dragging = true;
            dragStartY = location.Y;
            dragStartScroll = ScrollY;
            velocity = 0;
        }

        /// <summary>
        /// Mouse click down event handler
        /// </summary>
/*
        public void OnMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            lastMouseY = e.Y;
        }
*/
        /// <summary>
        /// Mouse move event handler
        /// </summary>
        public void OnMouseMove(Point location)
        {
            if (!dragging) return;

            float delta = location.Y - dragStartY;
            ScrollY = dragStartScroll - delta;
        }

        /// <summary>
        /// Mouse release event handler
        /// </summary>
        public void OnMouseUp()
        {
            dragging = false;
        }

        /// <summary>
        /// Returns the visible drawing area (used for content clipping)
        /// </summary>
        public RectangleF GetClippingRect()
        {
            return ViewportBounds;
        }

        /// <summary>
        /// Returns the visual start Y of the content (used to offset content)
        /// </summary>
        public float GetContentOffsetY()
        {
            return -ScrollY;
        }

        /// <summary>
        /// Returns the scroll boundary offset (positive: beyond the lower boundary, negative: beyond the upper boundary)
        /// </summary>
        private float GetOverscroll()
        {
            if (ScrollY < 0)
                return ScrollY;
            if (ScrollY > ContentHeight - ViewportBounds.Height)
                return ScrollY - (ContentHeight - ViewportBounds.Height);
            return 0;
        }

        /// <summary>
        /// If dragging
        /// </summary>
        public bool IsDragging => dragging;
    }
}
