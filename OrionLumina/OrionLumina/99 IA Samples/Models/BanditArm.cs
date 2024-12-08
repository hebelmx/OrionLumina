using System;

namespace Models
{
    namespace NArmBandit
    {
        /// <summary>
        /// Represents a single bandit arm.
        /// </summary>
        public class BanditArm
        {
            private readonly Random _random = new();
            public double SuccessProbability { get; }

            /// <summary>
            /// Initializes the bandit arm with a given success probability.
            /// </summary>
            /// <param name="successProbability">The probability of reward (0.0 to 1.0).</param>
            public BanditArm(double successProbability)
            {
                if (successProbability is < 0.0 or > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(successProbability), "Probability must be between 0.0 and 1.0.");

                SuccessProbability = successProbability;
            }

            /// <summary>
            /// Simulates pulling the arm.
            /// </summary>
            /// <returns>1 for reward, 0 for no reward.</returns>
            public int Pull()
            {
                return _random.NextDouble() < SuccessProbability ? 1 : 0;
            }
        }
    }

}
