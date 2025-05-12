/* ----- ----- ----- ----- */
// PhysicsRegistry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/11
// Update Date: 2025/05/11
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace SharedLib.PhysicsUtils
{
    public static class PhysicsRegistry
    {
        private static readonly HashSet<Physics2D> _allPhysics = new();

        public static void Register(Physics2D physics) => _allPhysics.Add(physics);
        public static void Unregister(Physics2D physics) => _allPhysics.Remove(physics);

        public static IEnumerable<Physics2D> GetAll() => _allPhysics;
    }
}