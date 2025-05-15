/* ----- ----- ----- ----- */
// UIScrollContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Interface.UI.Core;

using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.Interface.UI.Elements
{
    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia and border elasticity
    /// </summary>
    public class UIScrollContainer : UIElement
    {
        public RectangleF ViewportBounds { get; set; }
        public float ContentHeight { get; set; }
        
        // Use Physics2D to calc container movement
        public Physics2D ScrollPhysics { get; set; } = new Physics2D();
        private bool overContent => ContentHeight > ViewportBounds.Height;
        public float OverscrollLimit { get; set; } = 40.0f;
        private float dragStartY;
        private DateTime dragStartTime;
        private const float DragThreshold = 5.0f; // px
        private const int DragTimeThreshold = 100; // ms
        private bool hasMovedEnoughToDrag = false;
        private bool isDragging = false;
        private float lastMouseY;

        public bool EnableWheel = true;
        public bool EnableDrag = true;
        public float BaseScrollY { get; set; } = 0.0f;
        public float ScrollY
        {
            get => ScrollPhysics.Position.Current.Y - ScrollPhysics.Position.Base.Y;
            set
            {
                ScrollPhysics.Position.Current = new Vector2F(
                    ScrollPhysics.Position.Current.X,  // X axis no changed
                    ScrollPhysics.Position.Base.Y + value
                );
            }
        }
        public float ScrollVelocity
        {
            get => ScrollPhysics.Velocity.Current.Y - ScrollPhysics.Velocity.Base.Y;
            set
            {
                ScrollPhysics.Velocity.Current = new Vector2F(
                    ScrollPhysics.Velocity.Current.X,  // X axis no changed
                    ScrollPhysics.Velocity.Base.Y + value
                );
            }
        }

        public void InitializeScrollPhysics()
        {
            ScrollPhysics ??= new Physics2D();

            ScrollPhysics.Position.Base.X = ViewportBounds.X;
            ScrollPhysics.Position.Base.Y = ViewportBounds.Y - BaseScrollY;
            ScrollPhysics.Position.Current.X = ScrollPhysics.Position.Base.X;
            ScrollPhysics.Position.Current.Y = ScrollPhysics.Position.Base.Y;
            ScrollPhysics.Movement.CanSpring = true;
            ScrollPhysics.Movement.CanDamping = true;

            this.Physics = ScrollPhysics;
            
            ViewportBounds = new RectangleF(
                ScrollPhysics.Position.Base.X,
                ScrollPhysics.Position.Base.Y,
                Size.X,
                Size.Y
            );
        }

        /// <summary>
        /// Update object location (animation)
        /// </summary>
        public override void Update()
        {
            // Content is smaller than viewpoint, position return to base position
            if (!overContent)
            {
                // Moved
                if (ScrollY != 0)
                {
                    ScrollPhysics.Position.Target = ScrollPhysics.Position.Base;
                    ScrollPhysics.Position.HasTarget = true;
                }
                // Already back to base position
                else
                {
                    ScrollPhysics.Position.HasTarget = false;
                }
            }
            // Content is bigger than viewpoint
            else
            {
                // Case A: Scrolling up
                if (ScrollVelocity < 0)
                {
                    if (ScrollY <= OverscrollLimit)
                    {
                        // 超過底部，回彈至 MaxScrollY
                        ScrollPhysics.Position.Target = ScrollPhysics.Position.Base + OverscrollLimit;
                        ScrollPhysics.Position.HasTarget = true;
                    }
                    else
                    {
                        // Apply target position based on velocity
                        ApplyVelocityBasedTarget(ScrollVelocity);
                    }
                }
                // Case B: Scrolling down
                else if (ScrollVelocity > 0)
                {
                    if (ScrollY >= 0)
                    {
                        // 超過頂部，回彈至 0
                        ScrollPhysics.Position.Target = ScrollPhysics.Position.Base;
                        ScrollPhysics.Position.HasTarget = true;
                    }
                    else
                    {
                        // Apply target position based on velocity
                        ApplyVelocityBasedTarget(ScrollVelocity);
                    }
                }
            }
        }

        /// <summary>
        /// Adjusts the target position based on the velocity of scrolling.
        /// </summary>
        /// <param name="velocity">The current scrolling velocity.</param>
        private void ApplyVelocityBasedTarget(float velocity)
        {
            // For fast scroll, set the target position farther (e.g., 60px based on velocity)
            float targetOffset = Math.Sign(velocity) * Math.Min(Math.Abs(velocity), 60); // You can adjust this multiplier (e.g., 60) for higher velocities.

            // Apply target offset depending on scroll direction
            ScrollPhysics.Position.Target = ScrollPhysics.Position.Current + targetOffset;
            ScrollPhysics.Position.HasTarget = true;
        }

        /// <summary>
        /// Mouse down event handler
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (ViewportBounds.Contains(e.Location))
            {
                isDragging = true;
                hasMovedEnoughToDrag = false;
                lastMouseY = e.Y;
                dragStartY = e.Y;
                dragStartTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Mouse move event handler
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                float totalDeltaY = e.Y - lastMouseY;
                
                // 尚未判斷為有效拖動
                if (!hasMovedEnoughToDrag)
                {
                    if (Math.Abs(totalDeltaY) >= DragThreshold)
                    {
                        hasMovedEnoughToDrag = true;
                    }
                    else
                    {
                        return; // 尚未達到拖動門檻，不更新畫面
                    }
                }
                
                float deltaY = e.Y - lastMouseY;
                ScrollPhysics.Position.Current -= new Vector2F(0, deltaY);
                ScrollPhysics.Velocity.Current = new Vector2F(0, 0);  // Pause inertia while dragging
                lastMouseY = e.Y;
            }
        }

        /// <summary>
        /// Mouse release event handler
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;

            float totalDeltaY = e.Y - dragStartY;
            TimeSpan dragDuration = DateTime.Now - dragStartTime;

            bool isClick = !hasMovedEnoughToDrag || 
                           (Math.Abs(totalDeltaY) < DragThreshold && dragDuration.TotalMilliseconds < DragTimeThreshold);

            if (isClick)
            {
                // Treat as click – do nothing, or trigger click handler if needed
                ScrollPhysics.Velocity.Current = Vector2F.Zero;
            }
            else
            {
                // Apply inertia based on last delta
                float deltaY = e.Y - lastMouseY;
                ScrollPhysics.Velocity.Current = new Vector2F(0, -deltaY);  // ← inertia direction
            }
        }

        /// <summary>
        /// Mouse wheel event handler
        /// </summary>
        public void OnMouseWheel(MouseEventArgs e)
        {
            ScrollPhysics.Position.Current += new Vector2F(0, -e.Delta * 0.2f); // 方向需反轉
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
        /// If dragging
        /// </summary>
        public bool IsDragging => isDragging;
    }
}
