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

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Input;

using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace Chinese_Chess_v3.UI.Elements
{
    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia and border elasticity
    /// </summary>
    public class UIScrollContainer : UIElement
    {
        // Use Physics2D to calc container movement
        public Physics2D ScrollPhysics { get; set; } = new Physics2D();
        private ScrollInputHandler inputHandler;
        public RectangleF ViewportBounds
        {
            get
            {
                var position = ScrollPhysics?.Position.Base ?? Vector2F.Zero;
                return new RectangleF(position.ToPointF(), Size.ToSizeF());
            }
        }
        public float ContentHeight { get; set; }
        
        private bool overContent => ContentHeight > ViewportBounds.Height;
        public float OverscrollLimit { get; set; } = 40.0f;
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
        public override UIPosition LocalPosition
        {
            get => base.LocalPosition;
            set
            {
                base.LocalPosition = value;
                ScrollPhysics.Position = this.LocalPosition?.Current ?? Vector2F.Zero;
            }
        }
        public override Vector2F Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
            }
        }

        public UIScrollContainer()
        {
            ScrollPhysics ??= new Physics2D();
            inputHandler = new ScrollInputHandler();
            inputHandler.Bind(ScrollPhysics, () => this.ViewportBounds);

            ScrollPhysics.Movement.CanSpring = true;
            ScrollPhysics.Movement.CanDamping = true;

            this.Physics = ScrollPhysics;
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
        protected override bool HandleMouseDown(MouseEventArgs e)
        {
            return inputHandler.OnMouseDown(e);
        }

        /// <summary>
        /// Mouse move event handler
        /// </summary>
        protected override bool HandleMouseMove(MouseEventArgs e)
        {
            return inputHandler.OnMouseMove(e);
        }

        /// <summary>
        /// Mouse release event handler
        /// </summary>
        protected override bool HandleMouseUp(MouseEventArgs e)
        {
            return inputHandler.OnMouseUp(e);
        }

        /// <summary>
        /// Mouse wheel event handler
        /// </summary>
        protected override bool HandleMouseWheel(MouseEventArgs e)
        {
            return inputHandler.OnMouseWheel(e);
        }

        /// <summary>
        /// Mouse click event handler
        /// </summary>
        protected override bool HandleMouseClick(MouseEventArgs e)
        {
            // If no event need to be handle, return base to pass to child
            return base.HandleMouseClick(e);
        }
        
        /// <summary>
        /// Call every frame after processing input to reset scroll delta.
        /// </summary>
        public override void EndFrame()
        {
            inputHandler.EndFrame();
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
    }
}
