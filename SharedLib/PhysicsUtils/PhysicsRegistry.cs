/* ----- ----- ----- ----- */
// PhysicsRegistry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedLib.PhysicsUtils
{
    public static class PhysicsRegistry
    {
        private static readonly HashSet<Physics2D> _allPhysics = new();
        public static void Register(Physics2D physics)
        {
            if (!_allPhysics.Add(physics))
                throw new InvalidOperationException("Physics2D instance already registered.");
        }
        public static void Unregister(Physics2D physics) => _allPhysics.Remove(physics);

        public static IEnumerable<Physics2D> GetAll() => _allPhysics;
        
        public static void UpdateAll()
        {
            foreach (var p in _allPhysics)
            {
                p.SmoothUpdate();
            }

            Physics2D.CleanupAllPhysicsEffects();
        }

        // Update all physics objects using parallel processing
        public static void ParallelUpdateAll()
        {
            Parallel.ForEach(_allPhysics, physics => physics.SmoothUpdate());
        }
    }
}