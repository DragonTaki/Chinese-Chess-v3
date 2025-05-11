/* ----- ----- ----- ----- */
// GlobalRandom.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

namespace SharedLib.RandomTable
{
    public static class GlobalRandom
    {
        public static IRandomProvider Instance { get; set; } = new RandomTable(size: 10000, seed: 12345);
    }
}