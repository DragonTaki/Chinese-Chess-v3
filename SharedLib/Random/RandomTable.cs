/* ----- ----- ----- ----- */
// RandomTable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace SharedLib.RandomTable
{
    /// <summary>
    /// High-performance reusable random number table for predictable and efficient random values.
    /// </summary>
    public class RandomTable : IRandomProvider
    {
        private readonly int[] intTable;
        private readonly float[] floatTable;
        private readonly double[] doubleTable;
        private int index;
        private readonly int tableSize;

        /// <summary>
        /// Initializes a new random table with the specified size and seed.
        /// </summary>
        /// <param name="size">Number of pre-generated values.</param>
        /// <param name="seed">Seed for deterministic behavior.</param>
        public RandomTable(int size, int seed)
        {
            tableSize = size;
            intTable = new int[size];
            floatTable = new float[size];
            doubleTable = new double[size];
            Random rand = new Random(seed);

            for (int i = 0; i < size; i++)
            {
                intTable[i] = rand.Next();
                floatTable[i] = (float)rand.NextDouble();
                doubleTable[i] = rand.NextDouble();
            }

            index = 0;
        }

        /// <summary>
        /// Gets the next pre-generated integer.
        /// </summary>
        public int NextInt()
        {
            int value = intTable[index];
            Advance();
            return value;
        }

        /// <summary>
        /// Gets the next pre-generated integer, smaller than `max`.
        /// </summary>
        public int NextInt(int max)
        {
            return NextInt() % max;
        }

        /// <summary>
        /// Gets the next pre-generated integer, between `min` and `max`.
        /// </summary>
        public int NextInt(int min, int max)
        {
            return min + NextInt(max - min);
        }

        /// <summary>
        /// Gets the next pre-generated float between 0 and 1.
        /// </summary>
        public float NextFloat()
        {
            float value = floatTable[index];
            Advance();
            return value;
        }

        /// <summary>
        /// Gets the next pre-generated float between 0 and `max`.
        /// </summary>
        public float NextFloat(float max)
        {
            return NextFloat() * max;
        }
        
        /// <summary>
        /// Gets the next pre-generated float between `min` and `max`.
        /// </summary>
        public float NextFloat(float min, float max)
        {
            return min + NextFloat(max - min);
        }

        /// <summary>
        /// Gets the next pre-generated double between 0 and 1.
        /// </summary>
        public double NextDouble()
        {
            double value = doubleTable[index];
            Advance();
            return value;
        }

        /// <summary>
        /// Gets the next pre-generated double between 0 and `max`.
        /// </summary>
        public double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        /// <summary>
        /// Gets the next pre-generated double between `min` and `max`.
        /// </summary>
        public double NextDouble(double min, double max)
        {
            return min + NextDouble(max - min);
        }

        /// <summary>
        /// Advances the index circularly.
        /// </summary>
        private void Advance()
        {
            index = (index + 1) % tableSize;
        }

        /// <summary>
        /// Resets the index to 0.
        /// </summary>
        public void Reset() => index = 0;
    }
}
