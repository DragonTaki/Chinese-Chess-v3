/* ----- ----- ----- ----- */
// Physics2D.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using StarAnimation.Core.Effect;

using SharedLib.MathUtils;

namespace SharedLib.PhysicsUtils
{
    /// <summary>
    /// Represents a float range with inclusive minimum and maximum values.
    /// </summary>
    public class Physics2D
    {
        // Basic physical data of the object
        public Position Position { get; set; } = new Position();
        public Velocity Velocity { get; set; } = new Velocity();
        public Acceleration Acceleration { get; set; } = new Acceleration();
        public Dictionary<Guid, Vector2F> AccelerationContributions = new();

#nullable enable
        public Boundary? Boundary { get; set; } = new Boundary();
#nullable disable
        
        public Movement Movement { get; set; } = new Movement();

        private const float DirectionLerpFactor = 0.05f; // Smaller is smoother
        private const float SpeedLerpFactor = 0.02f;
        private const float AccelerationLerpFactor = 0.05f;
        private const float CalculateThreshold = 0.00001f;
        public float DragFactor { get; set; } = 0.98f;

        public Physics2D()
        {
            PhysicsRegistry.Register(this);
        }

        ~Physics2D()
        {
            PhysicsRegistry.Unregister(this);
        }

        // Implement in Star.cs
        //public Physics2D Physics => this;

        //public void UpdatePhysics()
        //{
        //    SmoothUpdate();
        //}

        /// <summary>
        /// Smoothly updates the object's position and velocity toward the target.
        /// </summary>
        public void SmoothUpdate()
        {
            // Integrate all sources of acceleration
            Acceleration.Target = Vector2F.Zero;

            if (AccelerationContributions == null)
                AccelerationContributions = new Dictionary<Guid, Vector2F>();

            foreach (var accel in AccelerationContributions.Values)
            {
                Acceleration.Target += accel;
            }

            // If position target was set, move towards
            if (Position.HasTarget)
            {
                    
                // Calculate the direction vector from the current position to the target position
                Vector2F delta = Position.Target - Position.Current;

                // Calculate length and normalize direction
                float distance = delta.Length();

                // Spring and Damping logic based on distance to target
                if (distance >= CalculateThreshold)
                {
                    Vector2F direction = delta.Normalize();

                    // Check if spring is enabled
                    if (Movement.CanSpring)
                    {
                        // Apply spring force towards the target position
                        float springForce = distance * Movement.SpringK;
                        Acceleration.Target += direction * springForce;
                    }
                    else
                    {
                        // Without spring, move directly towards the target
                        Acceleration.Target += direction * distance;
                    }

                    // Check if damping is enabled and the object is close to the target
                    if (Movement.CanDamping && distance < CalculateThreshold)
                    {
                        // Apply damping force to slow down as we approach the target
                        float dampingForce = Velocity.Current.Length() * Movement.Damping;
                        Acceleration.Target -= direction * dampingForce;
                    }

                    // Adjust only if the velocity direction deviates from the target direction
                    float angleDifference = Vector2F.AngleBetween(Velocity.Current, direction);

                    if (angleDifference > CalculateThreshold)
                    {
                        // If the velocity direction is not aligned with the target direction, apply corrective acceleration
                        Acceleration.Target = direction * Velocity.Target.Length();
                    }
                    else
                    {
                        // Direction is aligned, stop acceleration
                        Acceleration.Current = Vector2F.Zero;
                    }
                }
                else
                {
                    // When close to the target, stop acceleration to avoid overshooting
                    Acceleration.Target = Vector2F.Zero;

                    // Ensure that the object is precisely at the target position
                    Acceleration.Current = Vector2F.Zero;
                    Velocity.Current = Vector2F.Zero;
                    Position.Current = Position.Target;
                }
            }

            // Interpolate current acceleration towards target acceleration
            Acceleration.Current = Vector2F.Lerp(
                Acceleration.Current,
                Acceleration.Target,
                AccelerationLerpFactor
            );

            // Update velocity
            Velocity.Current += Acceleration.Current;

            // If target-driven movement, check next location before update, prevent "over" the target
            if (Position.HasTarget)
            {
                Vector2F nextPosition = Position.Current + Velocity.Current;
                Vector2F toTargetNow = Position.Target - Position.Current;
                Vector2F toTargetNext = Position.Target - nextPosition;

                // If the direction changes, means already over the target
                if (Vector2F.DotProduct(toTargetNow, toTargetNext) < 0)
                {
                    // Snap to target and stop motion
                    Position.Current = Position.Target;
                    Velocity.Current = Vector2F.Zero;
                    Acceleration.Current = Vector2F.Zero;
                    Acceleration.Target = Vector2F.Zero;
                    return;
                }
            }

            // Update Location
            Position.Current += Velocity.Current;
        }

        public static void CleanupAllPhysicsEffects()
        {
            foreach (var physics in PhysicsRegistry.GetAll())
                physics.CleanupInvalidEffectReferences();
        }

        private void CleanupInvalidEffectReferences()
        {
            var validIds = EffectInstance.GetAllActiveEffectIds();
            var keysToRemove = AccelerationContributions.Keys
                .Where(id => !validIds.Contains(id))
                .ToList();

            foreach (var id in keysToRemove)
                AccelerationContributions.Remove(id);
        }

        public void EnforceBoundaries()
        {
            if (Boundary.Min != null && Boundary.Max != null)
            {
                Position.Current.X = Math.Clamp(Position.Current.X, Boundary.Min.X, Boundary.Max.X);
                Position.Current.Y = Math.Clamp(Position.Current.Y, Boundary.Min.Y, Boundary.Max.Y);
            }
        }
    }

    /// <summary>
    /// Encapsulates the position-related values.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// The base position (X and Y components).
        /// </summary>
        public Vector2F Base { get; set; } = new Vector2F();

        /// <summary>
        /// The current position (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target position (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();

        /// <summary>
        /// Determines whether Target-based movement is enabled.
        /// </summary>
        public bool HasTarget { get; set; } = false;
        
        /// <summary>
        /// Default constructor with all positions set to (0, 0).
        /// </summary>
        public Position()
        {
            Base = Vector2F.Zero;
            Current = Vector2F.Zero;
            Target = Vector2F.Zero;
        }

        /// <summary>
        /// Constructor with a given initial position value.
        /// </summary>
        public Position(float positionX, float positionY)
        {
            Base = new Vector2F(positionX, positionY);
            Current = new Vector2F(positionX, positionY);
            Target = new Vector2F(positionX, positionY);
        }
        public Position(Vector2F position)
        {
            Base = position;
            Current = position;
            Target = position;
        }

        public static implicit operator Position(Vector2F v)
        {
            return new Position(v);
        }
    }

    /// <summary>
    /// Encapsulates the velocity-related values.
    /// </summary>
    public class Velocity
    {
        /// <summary>
        /// The base velocity (X and Y components).
        /// </summary>
        public Vector2F Base { get; set; } = new Vector2F();

        /// <summary>
        /// The current velocity (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target velocity (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();

        /// <summary>
        /// Default constructor with all velocities set to (0, 0).
        /// </summary>
        public Velocity()
        {
            Base = Vector2F.Zero;
            Current = Vector2F.Zero;
            Target = Vector2F.Zero;
        }

        /// <summary>
        /// Constructor with a given initial velocity value.
        /// </summary>
        public Velocity(float speedX, float speedY)
        {
            Base = new Vector2F(speedX, speedY);
            Current = new Vector2F(speedX, speedY);
            Target = new Vector2F(speedX, speedY);
        }
        public Velocity(Vector2F speed)
        {
            Base = speed;
            Current = speed;
            Target = speed;
        }
    }

    /// <summary>
    /// Encapsulates the acceleration-related values.
    /// </summary>
    public class Acceleration
    {
        /// <summary>
        /// The current acceleration (X and Y components).
        /// </summary>
        public Vector2F Current { get; set; } = new Vector2F();

        /// <summary>
        /// The target acceleration (X and Y components).
        /// </summary>
        public Vector2F Target { get; set; } = new Vector2F();
    }

    /// <summary>
    /// Encapsulates the boundary-related values.
    /// </summary>
    public class Boundary
    {
        /// <summary>
        /// The minimum boundary position (X and Y components).
        /// </summary>
        public Vector2F Min { get; set; } = new Vector2F();

        /// <summary>
        /// The maximum boundary position (X and Y components).
        /// </summary>
        public Vector2F Max { get; set; } = new Vector2F();
    }

    /// <summary>
    /// Encapsulates the movement-related values.
    /// </summary>
    public class Movement
    {
        /// <summary>
        /// Spring movement constant.
        /// </summary>
        public float SpringK { get; set; } = 0.2f;

        /// <summary>
        /// Determines when close to target position, whether spring movement is enabled.
        /// </summary>
        public bool CanSpring { get; set; } = false;

        /// <summary>
        /// Damping constant.
        /// </summary>
        public float Damping { get; set; } = 0.85f;

        /// <summary>
        /// Determines when close to target position, whether damping movement is enabled.
        /// </summary>
        public bool CanDamping { get; set; } = false;
    }
}