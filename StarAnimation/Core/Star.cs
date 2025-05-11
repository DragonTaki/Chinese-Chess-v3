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

using SharedLib.RandomTable;
using SharedLib.MathUtils;

namespace StarAnimation.Core
{
    /// <summary>
    /// Represents a star in the starfield with structured properties for position, speed, direction, color, and animation phases.
    /// </summary>
    public class Star
    {
        // Basic physical data of the object
        public Position Position { get; set; } = new Position();
        public Velocity Velocity { get; set; } = new Velocity();
        public Acceleration Acceleration { get; set; } = new Acceleration();

        // Star data
        public StarColor Color { get; set; } = new StarColor();

        public float Size { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public ColorShiftEffect ColorShift { get; set; } = new ColorShiftEffect();
        public PulseEffect Pulse { get; set; } = new PulseEffect();
        public TwistEffect Twist { get; set; } = new TwistEffect();
        private readonly IRandomProvider Rand = GlobalRandom.Instance;

        /// <summary>
        /// Convenient access to PointF from Position.
        /// </summary>
        public PointF Point => new PointF(Position.Current.X, Position.Current.Y);

        /// <summary>
        /// Initializes a new star at a random position within the given width and height.
        /// Also sets a random speed and direction for the star's movement.
        /// </summary>
        /// <param name="rand">A random number generator used for randomizing star properties.</param>
        /// <param name="width">The width of the starfield area in which the star will be placed.</param>
        /// <param name="height">The height of the starfield area in which the star will be placed.</param>
        public Star(int width, int height)
        {
            Position.Current = new Vector2F(Rand.NextFloat(width), Rand.NextFloat(height));

            // Star size is a random value between 1 and 3
            Size = Rand.NextInt(1, 3);
            
            // Random base physical value
            Position.Target = Position.Current;
            //RandomizeTargetPosition(width, height);
            //RandomizeSpeed();
        }

        /// <summary>
        /// Randomizes the star's movement by setting a new target position in a random direction.
        /// </summary>
        /// <param name="distance">The distance from the current position to set the new target.</param>
        public void RandomizeTargetPosition(int width, int height)
        {
            Position.Target.X = Rand.NextFloat(width);
            Position.Target.Y = Rand.NextFloat(height);
        }

        /// <summary>
        /// Randomizes the speed of the star based on its base speed.
        /// </summary>
        public void RandomizeSpeed()
        {
            // Randomize speed within a factor of the base speed.
            Velocity.Target.X = Velocity.Base.X * (0.5f + (float)Rand.NextFloat());
            Velocity.Target.Y = Velocity.Base.Y * (0.5f + (float)Rand.NextFloat());
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
        /// Directly moves the star based on its current direction and speed.
        /// </summary>
        public void Move()
        {
            Position.Current += Velocity.Current;
        }

        /// <summary>
        /// Smoothly updates the star's position and velocity toward the target.
        /// </summary>
        public void SmoothMoveUpdate()
        {
            const float directionLerpFactor = 0.05f; // Smaller is smoother
            const float speedLerpFactor = 0.02f;

            // Calculate the direction vector from the current position to the target position
            Vector2F delta = Position.Target - Position.Current;

            // Calculate length and normalize direction
            float distance = delta.Length();
            if (distance < 0.001f)
                return;  // No move if already too close

            // Unit direction vector
            Vector2F direction = delta.Normalize();

            float targetSpeed = (Velocity.Target.X + Velocity.Target.Y) * 0.5f;
            Vector2F targetVelocity = direction * targetSpeed;

            // Slowly approaching the target speed
            Velocity.Current = Vector2F.Lerp(Velocity.Current, targetVelocity, directionLerpFactor);

            // Update Location
            Position.Current += Velocity.Current;
        }
    }

    /// <summary>
    /// Encapsulates the position-related values for a star.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The current position of the star (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target position of the star (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();
    }

    /// <summary>
    /// Encapsulates the velocity-related values for a star.
    /// </summary>
    public class Velocity
    {
        /// <summary>
        /// The base velocity of the star (X and Y components).
        /// </summary>
        public Vector2F Base { get; set; }

        /// <summary>
        /// The current velocity of the star (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; }

        /// <summary>
        /// The target velocity of the star (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; }

        /// <summary>
        /// Default constructor with all velocities set to (0, 0).
        /// </summary>
        public Velocity()
        {
            Base = new Vector2F(0f, 0f);
            Current = new Vector2F(0f, 0f);
            Target = new Vector2F(0f, 0f);
        }

        /// <summary>
        /// Constructor with a given initial velocity value.
        /// </summary>
        public Velocity(float initialSpeedX, float initialSpeedY)
        {
            Base = new Vector2F(initialSpeedX, initialSpeedY);
            Current = new Vector2F(initialSpeedX, initialSpeedY);
            Target = new Vector2F(initialSpeedX, initialSpeedY);
        }
    }

    /// <summary>
    /// Encapsulates the acceleration-related values for a star.
    /// </summary>
    public class Acceleration
    {
        /// <summary>
        /// The current acceleration of the star (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target acceleration of the star (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();
    }

    /// <summary>
    /// Encapsulates color-related values for a star.
    /// </summary>
    public class StarColor
    {
        public Color Base { get; set; } = Color.White;
        public Color Current { get; set; } = Color.White;
        public Color Target { get; set; } = Color.White;
        public float LerpProgress { get; set; } = 0.0f;

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
    /// Encapsulates ColorShift effect related values for a star.
    /// </summary>
    public class ColorShiftEffect
    {
        public bool HasPhase { get; set; } = false;
        public float StartTime { get; set; } = 0.0f;
        public float Phase { get; set; } = 0.0f;
        public float BiasDirection { get; set; } = 1.0f;
    }
    
    /// <summary>
    /// Encapsulates Pulse effect related values for a star.
    /// </summary>
    public class PulseEffect
    {
        public bool HasPhase { get; set; } = false;
        public float Delay { get; set; } = 0.0f;
        public int ShiningTimes { get; set; } = 0;
    }
    
    /// <summary>
    /// Encapsulates Twist effect related values for a star.
    /// </summary>
    public class TwistEffect
    {
        public float InitialAngle { get; set; } = 0.0f;
    }
}