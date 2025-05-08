/* ----- ----- ----- ----- */
// Star.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using StarAnimation.Utils;

namespace StarAnimation.Core
{
    /// <summary>
    /// Represents a star in the starfield with properties such as position, size, speed, and movement direction.
    /// </summary>
    public class Star
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float DirectionX { get; set; }
        public float DirectionY { get; set; }
        public float TargetDirectionX;
        public float TargetDirectionY;
        public float BaseSpeed { get; set; }
        public float Speed { get; set; }
        public float TargetSpeed;
        public float Size { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }
        public Random Rand { get; set; }
        public bool HasColorShiftPhase { get; set; } = false;
        public float ColorShiftPhase { get; set; }
        public bool HasPulsePhase { get; set; } = false;
        public float PulsePhase { get; set; }

        /// <summary>
        /// Initializes a new star at a random position within the given width and height.
        /// Also sets a random speed and direction for the star's movement.
        /// </summary>
        /// <param name="rand">A random number generator used for randomizing star properties.</param>
        /// <param name="width">The width of the starfield area in which the star will be placed.</param>
        /// <param name="height">The height of the starfield area in which the star will be placed.</param>
        public Star(int width, int height, Random rand)
        {
            X = rand.Next(width);
            Y = rand.Next(height);
            Rand = rand;

            Size = rand.Next(1, 3);  // Star size is a random value between 1 and 3.

            BaseSpeed = (float)(0.5 + rand.NextDouble() * 1.5);  // Random base speed between 0.5 and 2.0.
            RandomizeSpeed(rand);
            Speed = TargetSpeed;

            RandomizeDirection(rand);
            DirectionX = TargetDirectionX;
            DirectionY = TargetDirectionY;

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
            TargetDirectionX = (float)Math.Cos(angle);  // X direction is calculated using cosine of the angle.
            TargetDirectionY = (float)Math.Sin(angle);  // Y direction is calculated using sine of the angle.
        }

        /// <summary>
        /// Randomizes the speed of the star based on its base speed.
        /// </summary>
        /// <param name="rand">The random number generator used to adjust the speed.</param>
        public void RandomizeSpeed(Random rand)
        {
            TargetSpeed = BaseSpeed * (0.5f + (float)rand.NextDouble());  // Randomize speed within a factor of the base speed.
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

        public void SmoothMoveUpdate()
        {
            const float directionLerpFactor = 0.05f;  // 越小越平滑
            const float speedLerpFactor = 0.02f;

            DirectionX = MathUtil.Lerp(DirectionX, TargetDirectionX, directionLerpFactor);
            DirectionY = MathUtil.Lerp(DirectionY, TargetDirectionY, directionLerpFactor);
            Speed = MathUtil.Lerp(Speed, TargetSpeed, speedLerpFactor);

            // Normalize to prevent direction magnitude drift
            float mag = (float)Math.Sqrt(DirectionX * DirectionX + DirectionY * DirectionY);
            if (mag > 0.001f)
            {
                DirectionX /= mag;
                DirectionY /= mag;
            }

            X += DirectionX * Speed;
            Y += DirectionY * Speed;
        }
    }
}