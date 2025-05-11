/* ----- ----- ----- ----- */
// IRandomProvider.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

namespace SharedLib.RandomTable
{
    public interface IRandomProvider
    {
        int NextInt();
        int NextInt(int max);
        int NextInt(int min, int max);
        float NextFloat();
        float NextFloat(float max);
        float NextFloat(float min, float max);
        double NextDouble();
        double NextDouble(double max);
        double NextDouble(double min, double max);
    }
}