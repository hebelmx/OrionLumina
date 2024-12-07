using System;

namespace Models;

    public static class Strategies
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Epsilon-greedy strategy to select an arm.
        /// </summary>
        /// <param name="averageRewards">Average rewards for each arm.</param>
        /// <param name="epsilon">Probability of exploring a random arm (0 to 1).</param>
        /// <returns>Index of the selected arm.</returns>
        public static int EpsilonGreedy(double[] averageRewards, double epsilon)
        {
            return _random.NextDouble() < epsilon ?
                // Exploration: Choose a random arm
                _random.Next(averageRewards.Length) :
                // Exploitation: Choose the arm with the highest average reward
                Array.IndexOf(averageRewards, averageRewards.Max());
        }

        
    }