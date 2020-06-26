using System;

namespace E3J.Resources
{
    /// <summary>
    /// RendomExtension class
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Shuffles the specified array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rng">The RNG.</param>
        /// <param name="array">The array.</param>
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
