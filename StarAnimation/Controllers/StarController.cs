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
using StarAnimation.Core.Effect;
using StarAnimation.Models;
using StarAnimation.Renderers;

using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.Randomization;

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

        // _starCount is the star count for this reference area; the actual count
        // scales with the canvas area so density stays constant across sizes.
        private const float ReferenceArea = 1920f * 1080f;
        private const int MinDimension = 10;

        // Current area-scaled target count, and the fractional star carried
        // between resize events (a drag-resize grows a few pixels at a time).
        private int _targetCount;
        private float _spawnRemainder = 0f;

        // Countdown timers for effects
        private int _directionChangeCountdown;
        private int _speedChangeCountdown;

        public StarController(int _width, int _height, int _starCount = 250)
        {
            this._width = Math.Max(_width, MinDimension);
            this._height = Math.Max(_height, MinDimension);
            this._starCount = _starCount;
            _targetCount = TargetCountFor(this._width, this._height);

            _minVisibleCount = _starCount - Settings.StarCountRange;
            _maxVisibleCount = _starCount + Settings.StarCountRange;

            _renderer = new StarRenderer(_width, _height);
            InitializeStars();
        }

        private void InitializeStars()
        {
            _stars.Clear();
            _waitingPool.Clear();

            for (int i = 0; i < _targetCount; i++)
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
        /// Per-frame update: clears stale physics effects, recycles out-of-bounds stars,
        /// releases waiting stars, and runs the post-resize cleanup.
        /// </summary>
        public void Update()
        {
            Physics2D.CleanupAllPhysicsEffects(EffectInstance.GetAllActiveEffectIds());
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
            int targetStars = _targetCount;
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
        /// Handles normal effects. Only the periodic speed (acceleration) change exists,
        /// and it is currently disabled by the "false &&" guard; the direction-change
        /// countdown is initialized but unused.
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
        /// <remarks>
        /// Keeps star density constant. Stars that end up outside a shrunk canvas are
        /// dropped right away: left in place, UpdateStarPositions would recycle them
        /// through the waiting pool back *inside* the smaller canvas (a sudden density
        /// jump), and a later grow would then add more on top. When growing, only the
        /// newly exposed area gets new stars, at the same density as everywhere else.
        /// </remarks>
        public void Resize(int newWidth, int newHeight)
        {
            newWidth = Math.Max(newWidth, MinDimension);
            newHeight = Math.Max(newHeight, MinDimension);

            int oldWidth = _width;
            int oldHeight = _height;

            if (newWidth < oldWidth || newHeight < oldHeight)
            {
                _stars.RemoveAll(star => star.Position.Current.X > newWidth || star.Position.Current.Y > newHeight);

                // The delayed sweep stays as a safety net for anything outside after the resize settles.
                _lastResizeTime = DateTime.Now;
                _pendingShrinkCleanup = true;
            }

            if (newWidth > oldWidth || newHeight > oldHeight)
            {
                int keptWidth = Math.Min(oldWidth, newWidth);
                int keptHeight = Math.Min(oldHeight, newHeight);

                // Newly exposed region = right strip (full new height) + bottom strip (kept width).
                float rightArea = (float)(newWidth - keptWidth) * newHeight;
                float bottomArea = (float)keptWidth * (newHeight - keptHeight);

                float toSpawn = (rightArea + bottomArea) * _starCount / ReferenceArea + _spawnRemainder;
                int added = (int)toSpawn;
                _spawnRemainder = toSpawn - added;

                for (int i = 0; i < added; i++)
                {
                    var star = new Star(newWidth, newHeight);
                    bool inRightStrip = Rand.NextFloat(rightArea + bottomArea) < rightArea;
                    star.Position.Current = inRightStrip
                        ? new Vector2F(keptWidth + Rand.NextFloat(newWidth - keptWidth), Rand.NextFloat(newHeight))
                        : new Vector2F(Rand.NextFloat(keptWidth), keptHeight + Rand.NextFloat(newHeight - keptHeight));
                    _stars.Add(star);
                }
            }

            _width = newWidth;
            _height = newHeight;
            _targetCount = TargetCountFor(newWidth, newHeight);

            // Recycled stars waiting to respawn count toward the total too.
            while (_waitingPool.Count > 0 && _stars.Count + _waitingPool.Count > _targetCount)
                _waitingPool.Dequeue();
        }

        private int TargetCountFor(int width, int height) =>
            Math.Max(1, (int)MathF.Round(_starCount * ((float)width * height) / ReferenceArea));

        /// <summary>
        /// Render all visible _stars (the canvas is not cleared here).
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(IGraphics g)
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
        /// <remarks>
        /// [DEPRECATED] Replaced by Gaussian-based dynamic control using ReleaseStars().
        /// </remarks>
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