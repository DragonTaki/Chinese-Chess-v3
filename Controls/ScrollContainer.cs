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

namespace Chinese_Chess_v3.Controls
{
    /// <summary>
    /// Provides a reusable vertical scroll container that supports dragging, scrolling, inertia and border elasticity
    /// </summary>
    public class ScrollContainer
    {
        public RectangleF ViewportBounds { get; set; }
        public float ContentHeight { get; set; }

        public float ScrollY { get; set; } = 0.0f;
        private float velocity = 0.0f;
        private const float Damping = 0.9f;
        private const float OverscrollSpring = 0.25f;
        private const float OverscrollLimit = 40.0f;
        private const float OverscrollSoftMargin = 20f; // 軟邊界範圍，用於過渡

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
            float overscrollAccel = overscrollCalculator.GetOverscrollAcceleration(ScrollY, ScrollVelocity);
            ScrollVelocity += overscrollAccel * deltaTime;
            ScrollY += ScrollVelocity * deltaTime;
            
            if (!dragging)
            {
                ScrollY += velocity;
                velocity *= Damping;

                float overscroll = GetOverscroll();

                // ① 對 overscroll 添加回彈力
                if (overscroll != 0)
                {
                    velocity -= overscroll * OverscrollSpring;
                }

                // ② 若速度與 overscroll 皆很小，則直接停止震動
                if (Math.Abs(velocity) < 0.1f && Math.Abs(overscroll) < 0.5f)
                {
                    velocity = 0;
                    // snap 回合法範圍
                    if (ScrollY < 0)
                        ScrollY = 0;
                    else if (ScrollY > ContentHeight - ViewportBounds.Height)
                        ScrollY = ContentHeight - ViewportBounds.Height;
                }
                Console.WriteLine($"ViewportHeight: {ViewportBounds.Height}, ContentHeight: {ContentHeight}, ScrollY: {ScrollY}, Overscroll: {GetOverscroll()}");
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
        public void OnMouseDown(PointF location)
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
        public void OnMouseMove(PointF location)
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
            float maxScroll = Math.Max(ContentHeight - ViewportBounds.Height, 0);

            if (ScrollY < -OverscrollLimit - OverscrollSoftMargin)
            {
                // 超出硬上限：強制回彈
                return ScrollY + OverscrollLimit;
            }
            else if (ScrollY < -OverscrollLimit)
            {
                // 緩衝區：漸進增加
                float t = (-ScrollY - OverscrollLimit) / OverscrollSoftMargin;
                return (ScrollY + OverscrollLimit) * t;
            }

            if (ScrollY > maxScroll + OverscrollLimit + OverscrollSoftMargin)
            {
                return ScrollY - (maxScroll + OverscrollLimit);
            }
            else if (ScrollY > maxScroll + OverscrollLimit)
            {
                float t = (ScrollY - maxScroll - OverscrollLimit) / OverscrollSoftMargin;
                return (ScrollY - maxScroll - OverscrollLimit) * t;
            }

            return 0;
            /*
            float maxScroll = Math.Max(ContentHeight - ViewportBounds.Height, 0);

            // Beyond the upper limit
            if (ScrollY < -OverscrollLimit)
                return ScrollY + OverscrollLimit;

            // Beyond the lower bound
            if (ScrollY > maxScroll + OverscrollLimit)
                return ScrollY - (maxScroll + OverscrollLimit);

            // Within the tolerance range (including the case where the viewport is larger than the content)
            return 0;*/
        }

        public float GetOverscrollAcceleration(float scrollY, float scrollVelocity)
        {
            float lowerBound = 0f;
            float upperBound = Math.Max(ContentHeight - ViewportHeight, 0);

            float overscroll = 0f;
            float k = 0.5f;   // 回彈力常數（彈簧）
            float b = 0.3f;   // 阻尼常數（速度阻力）

            if (scrollY < lowerBound)
            {
                overscroll = scrollY - lowerBound;
            }
            else if (scrollY > upperBound)
            {
                overscroll = scrollY - upperBound;
            }
            else
            {
                return 0f; // 不超出範圍，不反彈
            }

            return -k * overscroll - b * scrollVelocity;
        }

        /// <summary>
        /// If dragging
        /// </summary>
        public bool IsDragging => dragging;
    }
}
