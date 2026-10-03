/* ----- ----- ----- ----- */
// StarController.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using StarAnimation.Configs;
using StarAnimation.Core.Effect;
using StarAnimation.Models;
using StarAnimation.Renderers;

using Engine.Mathematics;
using Engine.Physics;
using Engine.Platform;
using Engine.Randomization;
using Engine.Timing;

namespace StarAnimation.Controllers
{
    public class StarController
    {
        private int _width;
        private int _height;
        private readonly StarRenderer _renderer;
        private readonly int _starCount;
        private readonly IRandomProvider _rand = GlobalRandom.Instance;

        private readonly List<Star> _stars = new List<Star>();
        public IReadOnlyList<Star> Stars => _stars;

        private readonly Queue<Star> _waitingPool = new Queue<Star>();

        // Visible-count bounds around the area-scaled target (the range scales with the area too).
        private int ScaledCountRange => (int)MathF.Round(Settings.StarCountRange * ((float)_width * _height) / ReferenceArea);
        private int MinVisibleCount => Math.Max(0, _targetCount - ScaledCountRange);
        private int MaxVisibleCount => _targetCount + ScaledCountRange;

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

        // Release of waiting stars (author decision 2026-10-02: the further the scene is below
        // its target count, the more are released; slower near the target, so it fills up
        // gradually). Each second this fraction of the missing stars is released (frame-rate
        // independent); the value is a visual tuning constant for the author.
        private const float ReleaseRatePerSecond = 1.5f;
        private float _releaseRemainder = 0f;

        // Countdown timers for effects
        private int _directionChangeCountdown;
        private int _speedChangeCountdown;

        public StarController(int width, int height, int starCount = 250)
        {
            _width = Math.Max(width, MinDimension);
            _height = Math.Max(height, MinDimension);
            _starCount = starCount;
            _targetCount = TargetCountFor(_width, _height);


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
            _directionChangeCountdown = _rand.NextInt(300, 800);
            _speedChangeCountdown = _rand.NextInt(100, 300);
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
        /// Updates star positions and queues out-of-bounds stars for reuse.
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
        /// Releases stars from the waiting pool toward the target count (see <see cref="CalculateStarsToRelease"/>).
        /// </summary>
        private void ReleaseStars()
        {
            int starsToRelease = CalculateStarsToRelease();

            for (int i = 0; i < starsToRelease; i++)
            {
                if (_waitingPool.Count > 0)
                {
                    Star star = _waitingPool.Dequeue();
                    star.Position.Current.X = _rand.NextInt(_width);
                    star.Position.Current.Y = _rand.NextInt(_height);
                    star.RandomizeBaseSpeed();
                    star.RandomizeAcceleration();
                    _stars.Add(star);
                }
            }
        }

        /// <summary>
        /// How many waiting stars to release this frame: the missing stars (area-scaled target
        /// minus stars in the scene) times the share of <see cref="ReleaseRatePerSecond"/> this
        /// frame's time covers (1 - e^(-rate * dt)), so far below the target many are released
        /// and near it only a few; fractions carry over to the next frame. None at or above the
        /// target. The old version clamped to at least 200 per frame, emptying the pool at once.
        /// </summary>
        private int CalculateStarsToRelease()
        {
            int missing = _targetCount - _stars.Count;
            if (missing <= 0)
            {
                _releaseRemainder = 0f;
                return 0;
            }

            float dt = Math.Max(0f, GlobalTime.Timer.DeltaTimeInSeconds);
            float wanted = missing * (1f - MathF.Exp(-ReleaseRatePerSecond * dt)) + _releaseRemainder;
            int count = Math.Min(missing, (int)wanted);
            _releaseRemainder = wanted - count;
            return count;
        }

        /// <summary>
        /// Removes stars out of bounds after a delay.
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
                _speedChangeCountdown = _rand.NextInt(100, 300);
            }
        }

        /// <summary>
        /// Handles a canvas resize and adjusts the star count accordingly (the renderer keeps its size; it does not use it).
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
                    bool inRightStrip = _rand.NextFloat(rightArea + bottomArea) < rightArea;
                    star.Position.Current = inRightStrip
                        ? new Vector2F(keptWidth + _rand.NextFloat(newWidth - keptWidth), _rand.NextFloat(newHeight))
                        : new Vector2F(_rand.NextFloat(keptWidth), keptHeight + _rand.NextFloat(newHeight - keptHeight));
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
        /// Render all visible stars (the canvas is not cleared here).
        /// </summary>
        /// <param name="g">The graphics context to draw to.</param>
        public void Draw(IGraphics g)
        {
            _renderer.Draw(g, _stars);
        }

        /// <summary>
        /// Get reference to all current stars (e.g. for external effects).
        /// </summary>
        public List<Star> GetStars() => _stars;

        /// <summary>
        /// Dynamically adjusts the number of visible stars using a bell curve-like behavior.
        /// </summary>
        /// <remarks>
        /// [DEPRECATED] Replaced by Gaussian-based dynamic control using ReleaseStars().
        /// </remarks>
        private void AdjustStarCount()
        {
            if (_stars.Count < MaxVisibleCount && _rand.NextDouble() < 0.2)
            {
                _stars.Add(new Star(_width, _height));
            }
            else if (_stars.Count > MinVisibleCount && _rand.NextDouble() < 0.1)
            {
                _stars.RemoveAt(_rand.NextInt(_stars.Count));
            }
        }
    }
}