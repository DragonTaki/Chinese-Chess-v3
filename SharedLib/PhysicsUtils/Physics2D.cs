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
        private const float DirectionLerpFactor = 0.05f; // Smaller is smoother
        private const float SpeedLerpFactor = 0.02f;
        private const float AccelerationLerpFactor = 0.05f;
        private const float CalculateThreshold = 0.00001f;

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
        /// Smoothly updates the star's position and velocity toward the target.
        /// </summary>
        public void SmoothUpdate()
        {
            // Integrate all sources of acceleration
            Acceleration.Target = Vector2F.Zero;
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
                if (distance >= CalculateThreshold)
                {
                    Vector2F direction = delta.Normalize();

                    // Adjust only if when the speed direction deviates from the target direction
                    float angleDifference = Vector2F.AngleBetween(Velocity.Current, direction);
                    
                    if (angleDifference > CalculateThreshold)
                    {
                        Acceleration.Target = direction * Velocity.Target.Length();
                    }
                    else
                    {
                        // Direction aligned and stop acceleration
                        Acceleration.Current = Vector2F.Zero;
                    }
                }
                else
                {
                    Acceleration.Target = Vector2F.Zero;
                }
            }

            Acceleration.Current = Vector2F.Lerp(
                Acceleration.Current,
                Acceleration.Target,
                AccelerationLerpFactor
            );

            // Update velocity
            Velocity.Current += Acceleration.Current;

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

        /// <summary>
        /// Determines whether Target-based movement is enabled.
        /// </summary>
        public bool HasTarget { get; set; } = false;
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
            Base = new Vector2F(0.0f, 0.0f);
            Current = new Vector2F(0.0f, 0.0f);
            Target = new Vector2F(0.0f, 0.0f);
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

}