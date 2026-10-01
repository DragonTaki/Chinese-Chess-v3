/* ----- ----- ----- ----- */
// Star.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Mathematics;
using Engine.Physics;
using Engine.Randomization;

namespace StarAnimation.Models
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
        private readonly IRandomProvider _rand = GlobalRandom.Instance;

        /// <summary>
        /// Convenient access to PointF from Position.
        /// </summary>
        public PointF Point => new PointF(Position.Current.X, Position.Current.Y);

        /// <summary>
        /// Initializes a new star at a random position within the given width and height.
        /// Also sets a random base velocity and target acceleration for the star's movement.
        /// </summary>
        /// <param name="width">The width of the starfield area in which the star will be placed.</param>
        /// <param name="height">The height of the starfield area in which the star will be placed.</param>
        public Star(int width, int height)
        {
            Position.Current = new Vector2F(_rand.NextFloat(width), _rand.NextFloat(height));

            // Star size is a random integer, 1 or 2 (the upper bound of NextInt is exclusive)
            Size = _rand.NextInt(1, 3);
            
            // Random base physical value
            //RandomizeTargetPosition(width, height);
            RandomizeBaseSpeed();
            RandomizeAcceleration();
        }

        /// <summary>
        /// Sets a new target position at a random point within the given width and height.
        /// </summary>
        /// <param name="width">Width of the area in which the target is chosen.</param>
        /// <param name="height">Height of the area in which the target is chosen.</param>
        public void RandomizeTargetPosition(int width, int height)
        {
            Position.Target = new Vector2F(_rand.NextFloat(width), _rand.NextFloat(height));
        }

        /// <summary>
        /// Assigns a new random base velocity (each axis in [-0.5, 0.5)) and resets the
        /// current velocity to it.
        /// </summary>
        public void RandomizeBaseSpeed()
        {
            // Pick a random base velocity; current velocity starts from it.
            Velocity.Base = new Vector2F(_rand.NextFloat(-0.5f, 0.5f), _rand.NextFloat(-0.5f, 0.5f));
            Velocity.Current = Velocity.Base;
        }
        /// <summary>
        /// Assigns a new random target acceleration (each axis in [-0.5, 0.5)).
        /// </summary>
        public void RandomizeAcceleration()
        {
            // Pick a random target acceleration.
            Acceleration.Target = new Vector2F(_rand.NextFloat(-0.5f, 0.5f), _rand.NextFloat(-0.5f, 0.5f));
        }

        /// <summary>
        /// Randomizes the star's color.
        /// </summary>
        private void RandomizeColor()
        {
            int red = _rand.NextInt(0, 256);    // Red component (0-255)
            int green = _rand.NextInt(0, 256);  // Green component (0-255)
            int blue = _rand.NextInt(0, 256);   // Blue component (0-255)
            Color.SetColor(red, green, blue);  // Set the star's color
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
        public float Delay { get; set; } = 0.0f;
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