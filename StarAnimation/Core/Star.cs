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

using SharedLib.RandomTable;

namespace StarAnimation.Core
{
    /// <summary>
    /// Represents a star in the starfield with structured properties for position, speed, direction, color, and animation phases.
    /// </summary>
    public class Star
    {
        public Vector2F Position { get; set; } = new Vector2F();
        public Vector2F Direction { get; set; } = new Vector2F();
        public Vector2F TargetDirection { get; set; } = new Vector2F();

        public StarSpeed Speed { get; set; } = new StarSpeed();
        public StarColor Color { get; set; } = new StarColor();

        public float Size { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public ColorShiftEffect ColorShift { get; set; } = new ColorShiftEffect();
        public PulseEffect Pulse { get; set; } = new PulseEffect();
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        /// <summary>
        /// Convenient access to PointF from Position.
        /// </summary>
        public PointF Point => new PointF(Position.X, Position.Y);

        /// <summary>
        /// Initializes a new star at a random position within the given width and height.
        /// Also sets a random speed and direction for the star's movement.
        /// </summary>
        /// <param name="rand">A random number generator used for randomizing star properties.</param>
        /// <param name="width">The width of the starfield area in which the star will be placed.</param>
        /// <param name="height">The height of the starfield area in which the star will be placed.</param>
        public Star(int width, int height)
        {
            Position = new Vector2F(Rand.NextFloat(width), Rand.NextFloat(height));

            // Star size is a random value between 1 and 3
            Size = Rand.NextInt(1, 3);
            
            // Random base speed between 0.5 and 2.0
            float speed = 0.5f + Rand.NextFloat() * 1.5f;
            Speed = new StarSpeed(speed);
        }

        /// <summary>
        /// Randomizes the star's movement direction by selecting a random angle.
        /// </summary>
        public void RandomizeDirection()
        {
            double angle = Rand.NextFloat() * 2 * Math.PI;  // Random angle between 0 and 2π.
            TargetDirection.X = (float)Math.Cos(angle);  // X direction is calculated using cosine of the angle.
            TargetDirection.Y = (float)Math.Sin(angle);  // Y direction is calculated using sine of the angle.
        }

        /// <summary>
        /// Randomizes the speed of the star based on its base speed.
        /// </summary>
        public void RandomizeSpeed()
        {
            Speed.Target = Speed.Base * (0.5f + (float)Rand.NextDouble());  // Randomize speed within a factor of the base speed.
        }

        /// <summary>
        /// Randomizes the star's color.
        /// </summary>
        private void RandomizeColor()
        {
            int red = Rand.NextInt(0, 256);    // Red component (0-255)
            int green = Rand.NextInt(0, 256);  // Green component (0-255)
            int blue = Rand.NextInt(0, 256);   // Blue component (0-255)
            Color.SetColor(red, green, blue);  // Set the star's color
        }

        /// <summary>
        /// Moves the star based on its current direction and speed.
        /// </summary>
        public void Move()
        {
            Position = new Vector2F(
                Position.X += Direction.X * Speed.Current,
                Position.Y += Direction.Y * Speed.Current
            );
        }

        public void SmoothMoveUpdate()
        {
            const float directionLerpFactor = 0.05f;  // The smaller the smoother
            const float speedLerpFactor = 0.02f;

            Direction.X = MathUtil.Lerp(Direction.X, TargetDirection.X, directionLerpFactor);
            Direction.Y = MathUtil.Lerp(Direction.Y, TargetDirection.Y, directionLerpFactor);
            Speed.Current = MathUtil.Lerp(Speed.Current, Speed.Target, speedLerpFactor);

            // Normalize to prevent direction magnitude drift
            float mag = (float)Math.Sqrt(Direction.X * Direction.X + Direction.Y * Direction.Y);
            if (mag > 0.001f)
            {
                Direction.X /= mag;
                Direction.Y /= mag;
            }

            Position.X += Direction.X * Speed.Current;
            Position.Y += Direction.Y * Speed.Current;
        }
    }
    
    /// <summary>
    /// Represents a 2D vector with X and Y coordinates.
    /// </summary>
    public class Vector2F
    {
        public float X { get; set; }
        public float Y { get; set; }

        /// <summary>
        /// Sets the X and Y using default values `(0f, 0f)`.
        /// </summary>
        public Vector2F()
        {
            X = 0f;
            Y = 0f;
        }

        /// <summary>
        /// Sets the X and Y using given values.
        /// </summary>
        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2F Zero => new Vector2F(0, 0);

        public float Length => (float)Math.Sqrt(X * X + Y * Y);
        public PointF ToPointF => new PointF(X, Y);
    }

    /// <summary>
    /// Encapsulates speed-related values for a star.
    /// </summary>
    public class StarSpeed
    {
        public float Base { get; set; }
        public float Current { get; set; }
        public float Target { get; set; }

        /// <summary>
        /// Sets the base, current, and target speeds using default values (all `0f`).
        /// </summary>
        public StarSpeed()
        {
            Base = 0f;
            Current = 0f;
            Target = 0f;
        }

        /// <summary>
        /// Sets the base, current, and target speeds using given values.
        /// </summary>
        public StarSpeed(float initialSpeed)
        {
            Base = initialSpeed;
            Current = initialSpeed;
            Target = initialSpeed;
        }

    }

    /// <summary>
    /// Encapsulates color-related values for a star.
    /// </summary>
    public class StarColor
    {
        public Color Base { get; set; } = Color.White;
        public Color Current { get; set; } = Color.White;
        public Color Target { get; set; } = Color.White;
        public float LerpProgress { get; set; } = 0f;

        /// <summary>
        /// Sets the base, current, and target colors using RGB values.
        /// </summary>
        /// <param name="red">Red component (0–255).</param>
        /// <param name="green">Green component (0–255).</param>
        /// <param name="blue">Blue component (0–255).</param>
        public void SetColor(int red, int green, int blue)
        {
            var color = Color.FromArgb(red, green, blue);
            Base = color;
            Current = color;
            Target = color;
        }
    }
    
    /// <summary>
    /// Encapsulates colorshift effect related values for a star.
    /// </summary>
    public class ColorShiftEffect
    {
        public bool HasPhase { get; set; } = false;
        public float StartTime { get; set; } = 0f;
        public float Phase { get; set; } = 0f;
        public float BiasDirection { get; set; } = 1f;
    }
    
    /// <summary>
    /// Encapsulates pulse effect related values for a star.
    /// </summary>
    public class PulseEffect
    {
        public bool HasPhase { get; set; } = false;
        public float Delay { get; set; } = 0f;
        public int ShiningTimes { get; set; } = 0;
    }
}