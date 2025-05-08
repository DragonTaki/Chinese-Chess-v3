/* ----- ----- ----- ----- */
// StarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/09
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Configs;
using StarAnimation.Core;
using StarAnimation.Core.Effect;

namespace StarAnimation.Renderers
{
    public class StarRenderer
    {
        private int width;
        private int height;
        private int starCount;
        private Random rand;

        private List<Star> stars = new List<Star>();
        private Queue<Star> waitingPool = new Queue<Star>();

        private int minVisibleCount;
        private int maxVisibleCount;

        private DateTime lastResizeTime;
        private bool pendingShrinkCleanup = false;
        private const double ResizeCleanupDelaySeconds = 1.0;

        // Countdown timers for effects
        private int directionChangeCountdown;
        private int speedChangeCountdown;
        private int twistEffectCountdown;
        private int pulseEffectCountdown;
        private int colorShiftEffectCountdown;

        // Optional: external effect instances
        private Twist twistEffect = new Twist();
        private Pulse pulseEffect = new Pulse();
        private ColorShift colorShiftEffect = new ColorShift();

        public StarRenderer(int width, int height, int starCount = 150, Random rand = null)
        {
            this.width = width;
            this.height = height;
            this.starCount = starCount;
            this.rand = rand ?? new Random();

            minVisibleCount = starCount - Settings.StarCountRange;
            maxVisibleCount = starCount + Settings.StarCountRange;

            InitializeStars();
        }

        private void InitializeStars()
        {
            stars.Clear();
            waitingPool.Clear();

            for (int i = 0; i < maxVisibleCount; i++)
            {
                stars.Add(new Star(width, height, rand));
            }

            InitializeCounters();
        }

        private void InitializeCounters()
        {
            directionChangeCountdown = rand.Next(300, 800);
            speedChangeCountdown = rand.Next(100, 300);
            twistEffectCountdown = rand.Next(50, 100);
            pulseEffectCountdown = rand.Next(50, 100);
            colorShiftEffectCountdown = rand.Next(50, 100);
        }

        /// <summary>
        /// Update all stars' movement and handle dynamic effects.
        /// </summary>
        public void Update()
        {
            UpdateStarPositions();
            ReleaseStars();
            CleanUpAfterResize();
            UpdateEffects();
        }

        /// <summary>
        /// Clear canvas and render all visible stars.
        /// </summary>
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, width, height);

            foreach (var star in stars)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(star.Opacity * 255), star.Color)))
                {
                    g.FillEllipse(brush, star.X, star.Y, star.Size, star.Size);
                }
            }
        }

        /// <summary>
        /// Updates star positions and queues out-of-bounds stars for reuse.
        /// </summary>
        private void UpdateStarPositions()
        {
            foreach (var s in stars.ToArray())
            {
                s.SmoothMoveUpdate();
                if (s.X < 0 || s.Y < 0 || s.X > width || s.Y > height)
                {
                    waitingPool.Enqueue(s);
                    stars.Remove(s);
                    s.X = -100;
                    s.Y = -100;
                    s.Speed = 0;
                }
            }
        }

        /// <summary>
        /// Releases stars from waiting pool based on Gaussian probability.
        /// </summary>
        private void ReleaseStars()
        {
            int starsToRelease = CalculateStarsToRelease();

            for (int i = 0; i < starsToRelease; i++)
            {
                if (waitingPool.Count > 0)
                {
                    Star s = waitingPool.Dequeue();
                    s.X = rand.Next(width);
                    s.Y = rand.Next(height);
                    s.Speed = s.BaseSpeed * (0.5f + (float)rand.NextDouble());
                    stars.Add(s);
                }
            }
        }

        /// <summary>
        /// Bell-curve like star release count.
        /// </summary>
        private int CalculateStarsToRelease()
        {
            int targetStars = starCount;
            int starsInScene = stars.Count;
            float normalized = (float)Math.Exp(-0.5 * Math.Pow((starsInScene - targetStars) / 25.0, 2));
            return Math.Max(minVisibleCount, Math.Min(maxVisibleCount, (int)(normalized * (maxVisibleCount - minVisibleCount))));
        }

        /// <summary>
        /// Removes stars out of bounds after a delay.
        /// </summary>
        private void CleanUpAfterResize()
        {
            if (pendingShrinkCleanup && (DateTime.Now - lastResizeTime).TotalSeconds > ResizeCleanupDelaySeconds)
            {
                stars.RemoveAll(s => s.X > width || s.Y > height);
                pendingShrinkCleanup = false;
            }
        }

        /// <summary>
        /// Handles all active effects (twist, pulse, color shift).
        /// </summary>
        private void UpdateEffects()
        {
            if (--directionChangeCountdown <= 0)
            {
                foreach (var star in stars)
                    star.RandomizeDirection(rand);
                directionChangeCountdown = rand.Next(300, 800);
            }

            if (--speedChangeCountdown <= 0)
            {
                foreach (var star in stars)
                    star.RandomizeSpeed(rand);
                speedChangeCountdown = rand.Next(100, 300);
            }

            if (--twistEffectCountdown <= 0 && false)
            {
                twistEffect.Apply(stars, rand);
                twistEffectCountdown = rand.Next(50, 100);
            }

            if (--pulseEffectCountdown <= 0 && false)
            {
                pulseEffect.Apply(stars, rand);
                pulseEffectCountdown = rand.Next(50, 100);
            }

            if (--colorShiftEffectCountdown <= 0 && false)
            {
                colorShiftEffect.Apply(stars, rand);
                colorShiftEffectCountdown = rand.Next(50, 100);
            }
        }

        /// <summary>
        /// Handles resizing of the renderer and adjusts star count accordingly.
        /// </summary>
        public void Resize(int newWidth, int newHeight)
        {
            const int MinDimension = 10;
            newWidth = Math.Max(newWidth, MinDimension);
            newHeight = Math.Max(newHeight, MinDimension);

            if (newWidth > width || newHeight > height)
            {
                int added = (int)((newWidth * newHeight - width * height) / (1920f * 1080f) * starCount);
                for (int i = 0; i < added; i++)
                    stars.Add(new Star(newWidth, newHeight, rand));
            }
            else
            {
                lastResizeTime = DateTime.Now;
                pendingShrinkCleanup = true;
            }

            width = newWidth;
            height = newHeight;
        }

        /// <summary>
        /// Get reference to all current stars (e.g. for external effects).
        /// </summary>
        public List<Star> GetStars() => stars;

        /// <summary>
        /// Dynamically adjusts the number of visible stars using a bell curve-like behavior.
        /// </summary>
        /// [DEPRECATED] Replaced by Gaussian-based dynamic control using ReleaseStars()
        private void AdjustStarCount()
        {
            if (stars.Count < maxVisibleCount && rand.NextDouble() < 0.2)
            {
                stars.Add(new Star(width, height, rand));
            }
            else if (stars.Count > minVisibleCount && rand.NextDouble() < 0.1)
            {
                stars.RemoveAt(rand.Next(stars.Count));
            }
        }
    }
}
