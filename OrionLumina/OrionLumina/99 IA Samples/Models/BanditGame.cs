using System;
using System.Collections.Generic;
using System.Linq;
using Models.NArmBandit;

namespace Models
{
    public class BanditGame
    {
        private readonly List<BanditArm> _arms;
        private readonly Random _random = new();

        public int[] PullCounts { get; }
        public double[] Rewards { get; }

        public BanditGame(IEnumerable<double> successProbabilities)
        {
            _arms = successProbabilities.Select(p => new BanditArm(p)).ToList();
            PullCounts = new int[_arms.Count];
            Rewards = new double[_arms.Count];
        }

        /// <summary>
        /// Pulls the specified arm.
        /// </summary>
        /// <param name="armIndex">Index of the arm to pull.</param>
        /// <returns>The reward obtained (0 or 1).</returns>
        public int PullArm(int armIndex)
        {
            if (armIndex < 0 || armIndex >= _arms.Count)
                throw new ArgumentOutOfRangeException(nameof(armIndex), "Invalid arm index.");

            int reward = _arms[armIndex].Pull();
            PullCounts[armIndex]++;
            Rewards[armIndex] += reward;

            return reward;
        }

        /// <summary>
        /// Returns the average reward for each arm.
        /// </summary>
        public double[] GetAverageRewards()
        {
            return Rewards.Select((reward, i) => PullCounts[i] == 0 ? 0.0 : reward / PullCounts[i]).ToArray();
        }
    }
}