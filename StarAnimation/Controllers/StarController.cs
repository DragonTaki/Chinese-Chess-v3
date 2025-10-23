/* ----- ----- ----- ----- */
// StarController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using StarAnimation.Configs;
using StarAnimation.Models;
using StarAnimation.Renderers;

using Engine.Mathematics;
using Engine.Randomization;
using Engine.Physics;

namespace StarAnimation.Controllers
{
    public class StarController
    {
        private int _width;
        private int _height;
        private readonly StarRenderer _renderer;
        private readonly int _starCount;
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        private readonly List<Star> _stars = new List<Star>();
        public IReadOnlyList<Star> Stars => _stars;

        private readonly Queue<Star> _waitingPool = new Queue<Star>();

        private readonly int _minVisibleCount;
        private readonly int _maxVisibleCount;

        private DateTime _lastResizeTime;
        private bool _pendingShrinkCleanup = false;
        private const float ResizeCleanupDelaySeconds = 1.0f;
        private const float OutsideCanvasMargin = 40.0f;

        // Countdown timers for effects
        private int _directionChangeCountdown;
        private int _speedChangeCountdown;

        public StarController(int _width, int _height, int _starCount = 250)
        {
            this._width = _width;
            this._height = _height;
            this._starCount = _starCount;

            _minVisibleCount = _starCount - Settings.StarCountRange;
            _maxVisibleCount = _starCount + Settings.StarCountRange;

            _renderer = new StarRenderer(_width, _height);
            InitializeStars();
        }

        private void InitializeStars()
        {
            _stars.Clear();
            _waitingPool.Clear();

            for (int i = 0; i < _starCount; i++)
            {
                _stars.Add(new Star(_width, _height));
            }
            
            InitializeCounters();
        }

        private void InitializeCounters()
        {
            _directionChangeCountdown = Rand.NextInt(300, 800);
            _speedChangeCountdown = Rand.NextInt(100, 300);
        }

        /// <summary>
        /// Update all _stars' movement and handle dynamic effects.
        /// </summary>
        public void Update()
        {
            Physics2D.CleanupAllPhysicsEffects();
            UpdateStarPositions();
            ReleaseStars();
            CleanUpAfterResize();
            UpdateEffects();
        }

        /// <summary>
        /// Updates star positions and queues out-of-bounds _stars for reuse.
        /// </summary>
        private void UpdateStarPositions()
        {
            foreach (var star in _stars.ToArray())
            {
                // If outside canvas, clear all status and put to waiting area
                if (star.Position.Current.X < -OutsideCanvasMargin || star.Position.Current.Y < -OutsideCanvasMargin ||
                    star.Position.Current.X > _width + OutsideCanvasMargin || star.Position.Current.Y > _height + OutsideCanvasMargin)
                {
                    _waitingPool.Enqueue(star);
                    star.Position.Current = new Vector2F(-100.0f, -100.0f);
                    star.Position.Target = Vector2F.Zero;
                    star.Position.HasTarget = false;
                    star.Velocity.Base = Vector2F.Zero;
                    star.Velocity.Current = Vector2F.Zero;
                    star.Velocity.Target = Vector2F.Zero;
                    star.Acceleration.Current = Vector2F.Zero;
                    star.Acceleration.Target = Vector2F.Zero;
                    _stars.Remove(star);
                }
            }
        }

        /// <summary>
        /// Releases _stars from waiting pool based on Gaussian probability.
        /// </summary>
        private void ReleaseStars()
        {
            int _starsToRelease = CalculateStarsToRelease();

            for (int i = 0; i < _starsToRelease; i++)
            {
                if (_waitingPool.Count > 0)
                {
                    Star star = _waitingPool.Dequeue();
                    star.Position.Current.X = Rand.NextInt(_width);
                    star.Position.Current.Y = Rand.NextInt(_height);
                    star.RandomizeBaseSpeed();
                    star.RandomizeAcceleration();
                    _stars.Add(star);
                }
            }
        }

        /// <summary>
        /// Bell-curve like star release count.
        /// </summary>
        private int CalculateStarsToRelease()
        {
            int targetStars = _starCount;
            int _starsInScene = _stars.Count;
            float normalized = (float)Math.Exp(-0.5 * Math.Pow((_starsInScene - targetStars) / 25.0, 2));
            return Math.Max(_minVisibleCount, Math.Min(_maxVisibleCount, (int)(normalized * (_maxVisibleCount - _minVisibleCount))));
        }

        /// <summary>
        /// Removes _stars out of bounds after a delay.
        /// </summary>
        private void CleanUpAfterResize()
        {
            if (_pendingShrinkCleanup && (DateTime.Now - _lastResizeTime).TotalSeconds > ResizeCleanupDelaySeconds)
            {
                _stars.RemoveAll(star => star.Position.Current.X > _width || star.Position.Current.Y > _height);
                _pendingShrinkCleanup = false;
            }
        }

        /// <summary>
        /// Handles normal effects (direction and speed).
        /// </summary>
        private void UpdateEffects()
        {
            if (false && --_speedChangeCountdown <= 0)
            {
                foreach (var star in _stars)
                    star.RandomizeAcceleration();
                _speedChangeCountdown = Rand.NextInt(100, 300);
            }
        }

        /// <summary>
        /// Handles resizing of the _renderer and adjusts star count accordingly.
        /// </summary>
        public void Resize(int newWidth, int newHeight)
        {
            const int MinDimension = 10;
            newWidth = Math.Max(newWidth, MinDimension);
            newHeight = Math.Max(newHeight, MinDimension);

            if (newWidth > _width || newHeight > _height)
            {
                int added = (int)((newWidth * newHeight - _width * _height) / (1920f * 1080f) * _starCount);
                for (int i = 0; i < added; i++)
                    _stars.Add(new Star(newWidth, newHeight));
            }
            else
            {
                _lastResizeTime = DateTime.Now;
                _pendingShrinkCleanup = true;
            }

            _width = newWidth;
            _height = newHeight;
        }

        /// <summary>
        /// Clear canvas and render all visible _stars.
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(Graphics g)
        {
            _renderer.Draw(g, _stars);
        }

        /// <summary>
        /// Get reference to all current _stars (e.g. for external effects).
        /// </summary>
        public List<Star> GetStars() => _stars;

        /// <summary>
        /// Dynamically adjusts the number of visible _stars using a bell curve-like behavior.
        /// </summary>
        /// [DEPRECATED] Replaced by Gaussian-based dynamic control using ReleaseStars()
        private void AdjustStarCount()
        {
            if (_stars.Count < _maxVisibleCount && Rand.NextDouble() < 0.2)
            {
                _stars.Add(new Star(_width, _height));
            }
            else if (_stars.Count > _minVisibleCount && Rand.NextDouble() < 0.1)
            {
                _stars.RemoveAt(Rand.NextInt(_stars.Count));
            }
        }
    }
}