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
using Microsoft.Extensions.DependencyInjection;
using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;
using static Chinese_Chess_v3.UI.Input.ScrollInputHandler;

namespace Chinese_Chess_v3.UI.Elements
{
    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia and border elasticity
    /// </summary>
    public class UIScrollContainer : UIElement, IPhysical2D
    {
        // Use Physics2D to calc container movement
        private readonly Physics2D _physics = new Physics2D();

        // Implement IPhysical2D.Physics
        Physics2D IPhysical2D.Physics => _physics;

        // Override UIElement.Physics getter safely
#nullable enable
        public override Physics2D? Physics => _physics;
#nullable disable

        public Position Position => _physics.Position;
        public Velocity Velocity => _physics.Velocity;
        public Acceleration Acceleration => _physics.Acceleration;

        private readonly IScrollInputHandler inputHandler;

        // In abs position
        public RectangleF AbsViewportBounds
        {
            get
            {
                var position = Physics?.Position.Base ?? Vector2F.Zero;

                return new RectangleF(position.ToPointF(), Size.ToSizeF());
            }
        }
        public float ContentHeight { get; set; }
        
        private bool overContent => ContentHeight > AbsViewportBounds.Height;
        public float OverscrollLimit { get; set; } = 40.0f;
        public float BaseScrollX { get; set; } = 0.0f;
        public float BaseScrollY { get; set; } = 0.0f;
        public float ScrollY
        {
            get => Physics.Position.Current.Y - Physics.Position.Base.Y;
            set
            {
                Physics.Position.Current = new Vector2F(
                    Physics.Position.Current.X,  // X axis no changed
                    Physics.Position.Base.Y + value
                );
            }
        }
        public float ScrollVelocity
        {
            get => Physics.Velocity.Current.Y - Physics.Velocity.Base.Y;
            set
            {
                Physics.Velocity.Current = new Vector2F(
                    Physics.Velocity.Current.X,  // X axis no changed
                    Physics.Velocity.Base.Y + value
                );
            }
        }
        public override UIPosition LocalPosition
        {
            get => base.LocalPosition;
            set
            {
                base.LocalPosition = value;
                var absPos = GetCurrentAbsolutePosition();

                if (Physics != null)
                {
                    Physics.Position = absPos;
                }
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
        
        public UIScrollContainer(IScrollInputHandler scroll)
        {
            inputHandler = scroll;
            // Register and bind
            scroll.RegisterScrollTarget(this, Physics, () => this.AbsViewportBounds, new ScrollBehavior
            {
                AllowDragY = true,
                AllowDragX = false,
                AllowWheel = true
            });

            Physics.Movement.CanSpring = true;
            Physics.Movement.CanDamping = true;
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
                    Physics.Position.Target = Physics.Position.Base;
                    Physics.Position.HasTarget = true;
                }
                // Already back to base position
                else
                {
                    Physics.Position.HasTarget = false;
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
                        // Beyond the bottom, rebound to MaxScrollY
                        Physics.Position.Target = Physics.Position.Base + OverscrollLimit;
                        Physics.Position.HasTarget = true;
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
                        // Over the top, rebound to 0
                        Physics.Position.Target = Physics.Position.Base;
                        Physics.Position.HasTarget = true;
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
            Physics.Position.Target = Physics.Position.Current + targetOffset;
            Physics.Position.HasTarget = true;
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
        /// Returns the visible drawing area in absolute position (used for content clipping)
        /// </summary>
        public RectangleF GetAbsClippingRect()
        {
            return AbsViewportBounds;
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
