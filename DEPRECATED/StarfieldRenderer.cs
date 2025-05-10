/* ----- ----- ----- ----- */
// StarfieldRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Chinese_Chess_v3.Interface.Renderers
{
    public class StarfieldRenderer
    {
        public List<LocalEffect> Effects { get; private set; } = new List<LocalEffect>();
        // 用於儲存框線區域
        public List<RectangleF> Areas { get; private set; } = new List<RectangleF>();
        #region Settings

        #endregion

        #region Star Class
        /// <summary>
        /// Represents a star in the starfield with properties such as position, size, speed, and movement direction.
        /// </summary>
        public class Star
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Speed { get; set; }
            public float BaseSpeed { get; set; }
            public float Size { get; set; }
            public float DirectionX { get; set; }
            public float DirectionY { get; set; }
            public Color Color { get; set; }
            public float Opacity { get; set; }
            public Random Rand { get; set; }

            /// <summary>
            /// Initializes a new star at a random position within the given width and height.
            /// Also sets a random speed and direction for the star's movement.
            /// </summary>
            /// <param name="rand">A random number generator used for randomizing star properties.</param>
            /// <param name="width">The width of the starfield area in which the star will be placed.</param>
            /// <param name="height">The height of the starfield area in which the star will be placed.</param>
            public Star(Random rand, int width, int height)
            {
                Rand = rand;
                X = rand.Next(width);
                Y = rand.Next(height);
                Size = rand.Next(1, 3);  // Star size is a random value between 1 and 3.
                BaseSpeed = (float)(0.5 + rand.NextDouble() * 1.5);  // Random base speed between 0.5 and 2.0.
                Speed = BaseSpeed;
                RandomizeDirection(rand);
                Color = Color.White;
                Opacity = 1.0f;
            }

            /// <summary>
            /// Moves the star based on its current direction and speed.
            /// </summary>
            public void Move()
            {
                X += DirectionX * Speed;
                Y += DirectionY * Speed;
            }

            /// <summary>
            /// Randomizes the star's movement direction by selecting a random angle.
            /// </summary>
            /// <param name="rand">The random number generator used for randomizing the direction.</param>
            public void RandomizeDirection(Random rand)
            {
                double angle = rand.NextDouble() * 2 * Math.PI;  // Random angle between 0 and 2π.
                DirectionX = (float)Math.Cos(angle);  // X direction is calculated using cosine of the angle.
                DirectionY = (float)Math.Sin(angle);  // Y direction is calculated using sine of the angle.
            }

            /// <summary>
            /// Randomizes the speed of the star based on its base speed.
            /// </summary>
            /// <param name="rand">The random number generator used to adjust the speed.</param>
            public void RandomizeSpeed(Random rand)
            {
                Speed = BaseSpeed * (0.5f + (float)rand.NextDouble());  // Randomize speed within a factor of the base speed.
            }

            /// <summary>
            /// Randomizes the star's color.
            /// </summary>
            /// <param name="rand">The random number generator used for randomizing the color.</param>
            private void RandomizeColor(Random rand)
            {
                int red = rand.Next(0, 256);    // Red component (0-255)
                int green = rand.Next(0, 256);  // Green component (0-255)
                int blue = rand.Next(0, 256);   // Blue component (0-255)
                Color = Color.FromArgb(red, green, blue);  // Set the star's color
            }
        }
        #endregion

        #region LocalEffect Class
        /// <summary>
        /// Represents a local visual effect applied to a specific area of the starfield, such as twisting or color-shifting.
        /// </summary>
        public class LocalEffect
        {
            public RectangleF Area { get; set; }
            public float TimeLeft { get; set; }
            public string Type { get; set; } // Type of effect: "twist", "pulse", "colorShift".
            public float Strength { get; set; }
            public Star AffectedStar { get; set; }
            public Color? TargetColor { get; set; }

            /// <summary>
            /// Initializes a new local effect with a specified area, duration, type, strength, and optional target color.
            /// </summary>
            /// <param name="area">The area within which the effect will be applied.</param>
            /// <param name="duration">The duration for which the effect will last, in seconds.</param>
            /// <param name="type">The type of effect ("twist", "pulse", or "colorShift").</param>
            /// <param name="strength">The strength of the effect (affects intensity).</param>
            /// <param name="color">Optional target color for the effect (only used for "colorShift").</param>
            public LocalEffect(RectangleF area, float duration, string type, float strength, Star star, Color? color = null)
            {
                Area = area;
                TimeLeft = duration;
                Type = type;
                Strength = strength;
                AffectedStar = star;
                TargetColor = color;
            }
        }
        #endregion

        // Fields for starfield state
        private readonly List<Star> stars = new();
        private readonly List<LocalEffect> effects = new();
        private readonly Random rand = new();
        private List<Rectangle> excludedAreas = new();

        // Configuration fields for visible star count
        private int minVisibleCount = 100;  // Minimum number of visible stars
        private int maxVisibleCount = 200;  // Maximum number of visible stars
        private int currentVisibleCount;
        private Queue<Star> waitingPool = new();  // Pool for storing stars that are temporarily invisible
        private int width, height;
        private int starCount;
        private float globalDirectionAngle = 90f;
        private int directionChangeCountdown;
        private int speedChangeCountdown;
        private int twistEffectCountdown;
        private int pulseEffectCountdown;
        private int colorShiftEffectCountdown;

        // Resize handling fields
        private DateTime lastResizeTime = DateTime.Now;
        private const int ResizeCleanupDelaySeconds = 3;
        private bool pendingShrinkCleanup = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StarfieldRenderer"/> class.
        /// </summary>
        /// <param name="width">The width of the rendering area (in pixels).</param>
        /// <param name="height">The height of the rendering area (in pixels).</param>
        /// <param name="starCount">The initial number of stars to generate (default is 150).</param>
        public StarfieldRenderer(int width, int height, int starCount = 150)
        {
            this.width = width;
            this.height = height;
            this.starCount = starCount;
            InitializeStars();
        }

        /// <summary>
        /// Initializes the stars in the starfield with random positions and speeds.
        /// </summary>
        private void InitializeStars()
        {
            stars.Clear();
            currentVisibleCount = minVisibleCount;  // Initial number of visible stars is set to minimum
            for (int i = 0; i < currentVisibleCount; i++)
            {
                stars.Add(new Star(rand, width, height));
            }

            directionChangeCountdown = rand.Next(300, 800);
            speedChangeCountdown = rand.Next(100, 300);
            twistEffectCountdown = rand.Next(50, 100);
            pulseEffectCountdown = rand.Next(50, 100);
            colorShiftEffectCountdown = rand.Next(50, 100);
        }

        /// <summary>
        /// Updates the state of the starfield, including star movement, direction, speed, and effects.
        /// </summary>
        public void Update()
        {
            HandleDirectionChange();
            HandleSpeedChange();
            HandleEffectSpawn();
            UpdateStarPositions();
            ReleaseStars();
            CleanUpAfterResize();
            UpdateEffects();
        }

        /// <summary>
        /// Handles the change of the global direction for stars' movement.
        /// </summary>
        private void HandleDirectionChange()
        {
            if (--directionChangeCountdown <= 0)
            {
                globalDirectionAngle = (float)(rand.NextDouble() * 360.0);  // Random new direction angle
                float rad = globalDirectionAngle * (float)Math.PI / 180;
                float dx = (float)Math.Cos(rad);
                float dy = (float)Math.Sin(rad);
                foreach (var s in stars)
                {
                    s.DirectionX = dx;
                    s.DirectionY = dy;
                }
                directionChangeCountdown = rand.Next(300, 800);  // Reset countdown for next direction change
            }
        }

        /// <summary>
        /// Handles the random change of speed for all stars.
        /// </summary>
        private void HandleSpeedChange()
        {
            if (--speedChangeCountdown <= 0)
            {
                foreach (var s in stars)
                    s.RandomizeSpeed(rand);
                speedChangeCountdown = rand.Next(100, 300);  // Reset countdown for next speed change
            }
        }

        /// <summary>
        /// Handles the spawning of visual effects in the starfield.
        /// </summary>
        private void HandleEffectSpawn()
        {
            // Decrement each effect's countdown
            if (--twistEffectCountdown <= 0)
            {
                // Spawn a new twist effect
                RectangleF area = new(
                    rand.Next((width - 50) / 2),
                    rand.Next((height - 50) / 2),
                    rand.Next(400, 1000),
                    rand.Next(400, 1000));
                    Areas.Add(area);

                Star affectedStar = new Star(rand, width, height);

                effects.Add(new LocalEffect(area, rand.Next(30, 60), "twist", 0.9f + (float)rand.NextDouble(), affectedStar, null));

                // Reset countdown for next twist effect spawn
                twistEffectCountdown = rand.Next(50, 100);
            }

            if (--pulseEffectCountdown <= 0)
            {
                // Spawn a new pulse effect
                RectangleF area = new(
                    rand.Next(width - 50),
                    rand.Next(height - 50),
                    rand.Next(60, 300),
                    rand.Next(60, 300));
                    Areas.Add(area);

                Star affectedStar = new Star(rand, width, height);

                effects.Add(new LocalEffect(area, rand.Next(30, 60), "pulse", 0.5f + (float)rand.NextDouble(), affectedStar, null));

                // Reset countdown for next pulse effect spawn
                pulseEffectCountdown = rand.Next(50, 100);
            }

            if (--colorShiftEffectCountdown <= 0)
            {
                // Spawn a new colorShift effect
                RectangleF area = new(
                    rand.Next(width - 50),
                    rand.Next(height - 50),
                    rand.Next(100, 500),
                    rand.Next(100, 500));
                    Areas.Add(area);

                Star affectedStar = new Star(rand, width, height);
                Color targetColor = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));

                effects.Add(new LocalEffect(area, rand.Next(30, 60), "colorShift", 0.5f + (float)rand.NextDouble(), affectedStar, targetColor));

                // Reset countdown for next colorShift effect spawn
                colorShiftEffectCountdown = rand.Next(50, 100);
            }
        }

        /// <summary>
        /// Updates the positions of all stars and removes stars that are out of bounds.
        /// </summary>
        private void UpdateStarPositions()
        {
            foreach (var s in stars.ToArray())  // Use ToArray to avoid modification errors while iterating
            {
                s.Move();
                if (s.X < 0 || s.Y < 0 || s.X > width || s.Y > height)
                {
                    waitingPool.Enqueue(s);
                    stars.Remove(s);
                    s.X = -100; s.Y = -100; s.Speed = 0;
                }
            }
        }

        /// <summary>
        /// Releases stars from the waiting pool to the starfield based on the calculated release count.
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
        /// Calculates the number of stars to release based on a Gaussian distribution.
        /// </summary>
        private int CalculateStarsToRelease()
        {
            int maxStars = maxVisibleCount;
            int minStars = minVisibleCount;
            int targetStars = 150;

            int starsInScene = stars.Count;

            float normalizedProbability = (float)Math.Exp(-0.5 * Math.Pow((starsInScene - targetStars) / 25.0, 2));

            int starsToRelease = Math.Max(minStars, Math.Min(maxStars, (int)(normalizedProbability * (maxStars - minStars))));

            return starsToRelease;
        }

        /// <summary>
        /// Cleans up stars after a resize if necessary.
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
        /// Updates and handles all local effects in the star field (e.g. distortions, pulses, color changes, etc.).
        /// </summary>
        private void UpdateEffects()
        {
            foreach (var effect in effects.ToArray())
            {
                effect.TimeLeft -= 1;

                if (effect.TimeLeft <= 0)
                {
                    effects.Remove(effect);
                }
                else
                {
                    switch (effect.Type)
                    {
                        case "twist":
                            ApplyTwistEffect(effect);
                            break;

                        case "pulse":
                            ApplyPulseEffect(effect);
                            break;

                        case "colorShift":
                            ApplyColorShiftEffect(effect);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Applies the twist effect to the given star effect.
        /// </summary>
        private void ApplyTwistEffect(LocalEffect effect)
        {
            float angle = DateTime.Now.Millisecond / 1000.0f * 2 * (float)Math.PI; // 時間與角度的關係
            float radius = 100 + effect.Strength * 100; // 根據強度決定偏移範圍
            float offsetX = radius * (float)Math.Cos(angle);
            float offsetY = radius * (float)Math.Sin(angle);

            effect.AffectedStar.X += offsetX;  // 偏移X
            effect.AffectedStar.Y += offsetY;  // 偏移Y
            // Twist effect: currently only supports updating of simple distortion effects over time
            // Distortion strength decreases over time
            effect.Strength *= 0.98f;  // Reduce strength
            // Can add more complex distortion logic here, such as offset of star positions, etc.
        }

        /// <summary>
        /// Applies the pulse effect to the given star effect.
        /// </summary>
        private void ApplyPulseEffect(LocalEffect effect)
        {
            // Pulse effect: changing the transparency of stars over time
            effect.Strength = 0.5f + 0.5f * (float)Math.Sin(DateTime.Now.Millisecond / 100.0);  // Use a sine wave to vary the transparency
            effect.AffectedStar.Opacity = effect.Strength;
            // This effect will change according to time, achieving a pulse-like effect
        }

        /// <summary>
        /// Applies the color shift effect to the given star effect.
        /// </summary>
        private void ApplyColorShiftEffect(LocalEffect effect)
        {
            float changeChance = 0.5f;

            float t = (float)Math.Sin(DateTime.Now.Millisecond / 1000.0);
            int red = (int)(255 * (0.5f - 0.5f * t));
            int blue = (int)(255 * (0.5f + 0.5f * t));
            Color colorShift = Color.FromArgb(red, 0, blue);

            if (rand.NextDouble() < changeChance)
            {
                effect.TargetColor = colorShift;
            }
            else
            {
                effect.TargetColor = Color.White;
            }

            effect.AffectedStar.Color = effect.TargetColor ?? Color.White;
        }

        /// <summary>
        /// Draws the starfield and effects onto the provided graphics object.
        /// </summary>
        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, width, height);
            GraphicsState state = g.Save();

            foreach (var s in stars)
            {
                if (IsInExcludedArea(s.X, s.Y)) continue;

                Color color = s.Color;
                float alpha = s.Opacity;

                foreach (var fx in effects)
                {
                    if (fx.Area.Contains(s.X, s.Y))
                    {
                        switch (fx.Type)
                        {
                            case "twist": alpha = 1f; break;
                            case "pulse": alpha = 0.5f + 0.5f * (float)Math.Sin(DateTime.Now.Millisecond / 100.0); break;
                            case "colorShift": color = fx.TargetColor ?? Color.White; break;
                        }
                    }
                }

                using var brush = new SolidBrush(Color.FromArgb((int)(alpha * 255), color));
                g.FillEllipse(brush, s.X, s.Y, s.Size, s.Size);
            }
            g.Restore(state);
        }

        /// <summary>
        /// Checks if a position is within any excluded area (e.g., blocked regions).
        /// </summary>
        private bool IsInExcludedArea(float x, float y)
        {
            foreach (var area in excludedAreas)
            {
                if (area.Contains((int)x, (int)y)) return true;
            }
            return false;
        }

        /// <summary>
        /// Sets excluded areas for the renderer (e.g., UI elements blocking star rendering).
        /// </summary>
        public void SetExcludedAreas(List<Rectangle> areas)
        {
            excludedAreas = areas;
        }

        /// <summary>
        /// Resizes the starfield renderer to the new dimensions and adjusts the number of stars accordingly.
        /// If the new dimensions are larger than the current dimensions, additional stars are added. 
        /// If the new dimensions are smaller, the renderer will prepare to remove stars outside the new bounds after a delay.
        /// </summary>
        /// <param name="newWidth">The new width of the renderer.</param>
        /// <param name="newHeight">The new height of the renderer.</param>
        public void Resize(int newWidth, int newHeight)
        {
            const int MinDimension = 10;
            newWidth = Math.Max(newWidth, MinDimension);
            newHeight = Math.Max(newHeight, MinDimension);

            if (newWidth > width || newHeight > height)
            {
                int added = (int)((newWidth * newHeight - width * height) / (1920f * 1080f) * starCount);
                for (int i = 0; i < added; i++)
                    stars.Add(new Star(rand, newWidth, newHeight));

                width = newWidth;
                height = newHeight;
            }
            else
            {
                width = newWidth;
                height = newHeight;
                lastResizeTime = DateTime.Now;
                pendingShrinkCleanup = true;
            }
        }
    }
}