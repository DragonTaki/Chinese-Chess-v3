/* ----- ----- ----- ----- */
// Star.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.RandomTable;
using SharedLib.MathUtils;
using SharedLib.PhysicsUtils;

namespace StarAnimation.Core
{
    /// <summary>
    /// Represents a star in the starfield with structured properties for position, speed, direction, color, and animation phases.
    /// </summary>
    public class Star : IPhysical2D
    {
        public Physics2D Physics { get; } = new Physics2D();
        public Position Position => Physics.Position;
        public Velocity Velocity => Physics.Velocity;
        public Acceleration Acceleration => Physics.Acceleration;
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
            //RandomizeTargetPosition(width, height);
            RandomizeBaseSpeed();
            RandomizeAcceleration();
        }

        /// <summary>
        /// Randomizes the star's movement by setting a new target position in a random direction.
        /// </summary>
        /// <param name="distance">The distance from the current position to set the new target.</param>
        public void RandomizeTargetPosition(int width, int height)
        {
            Position.Target = new Vector2F(Rand.NextFloat(width), Rand.NextFloat(height));
        }

        /// <summary>
        /// Randomizes the speed of the star based on its base speed.
        /// </summary>
        public void RandomizeBaseSpeed()
        {
            // Randomize speed within a factor of the base speed.
            Velocity.Base = new Vector2F(Rand.NextFloat(-0.5f, 0.5f), Rand.NextFloat(-0.5f, 0.5f));
            Velocity.Current = Velocity.Base;
        }
        /// <summary>
        /// Randomizes the speed of the star based on its base speed.
        /// </summary>
        public void RandomizeAcceleration()
        {
            // Randomize speed within a factor of the base speed.
            Acceleration.Target = new Vector2F(Rand.NextFloat(-0.5f, 0.5f), Rand.NextFloat(-0.5f, 0.5f));
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
        public void UpdatePhysics()
        {
            Physics.SmoothUpdate();
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