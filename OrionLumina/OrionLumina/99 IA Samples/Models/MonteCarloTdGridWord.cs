using System;
using System.Collections.Generic;
using System.Linq;
using Models.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Models
{
    public class MonteCarloTdGridWord
    {
        private readonly Random _random = new();
        private readonly double _gamma; // Discount factor
        private readonly double _alfa; // Discount factor
        private readonly double _epsilon; // Exploration factor
        private readonly int _maxSteps;
        private readonly int _maxEpisodes;
        private readonly ITestOutputHelper _testOutputHelper;

        public MonteCarloTdGridWord(double gamma, double epsilon, double alpha, ITestOutputHelper testOutputHelper, int maxSteps = 10_000, int maxEpisodes=10_000)
        {
            if (gamma is <= 0 or > 1)
                throw new ArgumentException("Gamma must be in the range (0, 1].");
            if (epsilon is < 0 or > 1)
                throw new ArgumentException("Epsilon must be in the range [0, 1].");
            
            _testOutputHelper = testOutputHelper;
            _gamma = gamma;
            _epsilon = epsilon;
            _alfa=alpha;
            _maxSteps = maxSteps;
            _maxEpisodes = maxEpisodes;
        }

        public (Dictionary<string, Dictionary<string, double>> Q, Dictionary<string, string> Policy) Control(
            GridWorld gridWorld,
            List<GridAction> actions)
        {
            var states = GenerateStateSpace(gridWorld);
            var actionStrings = actions.Select(a => a.Name).ToList();

            // Initialize Q(s, a) arbitrarily and policy π(s) arbitrarily
            var q = states.ToDictionary(
                s => s,
                s => actionStrings.ToDictionary(a => a, a => 0.0)
            );
            var policy = InitializePolicy(states, actionStrings);

            // Returns(s, a): stores all returns for a given state-action pair
            var returns = states.ToDictionary(
                s => s,
                s => actionStrings.ToDictionary(a => a, a => new List<double>())
            );

            for (var episode = 0; episode < _maxEpisodes; episode++)
            {
                // Reset the environment for a new episode
                gridWorld.Reset();
                var episodeSteps = GenerateEpisode(gridWorld, policy, actions);

                

                
                // Process the episode
                var seenPairs = new HashSet<(string state, string action)>();
                var g = 0.0; // Return

                for (var t = episodeSteps.Count - 1; t >= 0; t--)
                {
                    var (state, action, reward) = episodeSteps[t];

                    if (episodeSteps.Count < _maxEpisodes - 2)
                    {
                        _testOutputHelper.WriteLine($"state {state}, action {action}, reward {reward}");
                    }
                    

                    g = reward + _gamma * g; // Accumulate reward

                    // Skip if this state-action pair has already been seen in this episode
                    if (!seenPairs.Add((state, action)))
                        continue;

                    returns[state][action].Add(g);

                    // Update Q(s, a) as the average of the returns
                    q[state][action] = returns[state][action].Average();
                }

                // Policy Improvement: Update policy to be ε-soft

                policy = episode <= _maxEpisodes / 100 ? 
                    InitializePolicy(states, actionStrings) : 
                    UpdatePolicy(states, actionStrings, q, _epsilon);


            }

            return (q, policy);
        }

        private Dictionary<string, string> InitializePolicy(List<string> states, List<string> actions)
        {
            return states.ToDictionary(
                s => s,
                s => actions[_random.Next(actions.Count)]
            );
        }

        private Dictionary<string, string> UpdatePolicy(
            List<string> states,
            List<string> actions,
            Dictionary<string, Dictionary<string, double>> q,
            double epsilon)
        {
            var policy = new Dictionary<string, string>();

            foreach (var state in states)
            {
                // Find the action with the highest Q-value
                var bestAction = actions.OrderByDescending(a => q[state][a]).First();

                // Convert policy to ε-soft by adjusting probabilities
                var probabilities = actions.ToDictionary(
                    a => a,
                    a => a == bestAction
                        ? 1.0 - epsilon + epsilon / actions.Count
                        : epsilon / actions.Count
                );

                // Randomly choose action according to ε-soft probabilities
                policy[state] = ChooseActionBasedOnProbabilities(probabilities);
            }

            return policy;
        }

        private string ChooseActionBasedOnProbabilities(Dictionary<string, double> probabilities)
        {
            var cumulative = 0.0;
            var roll = _random.NextDouble();

            foreach (var (action, probability) in probabilities)
            {
                cumulative += probability;
                if (roll < cumulative)
                    return action;
            }

            return probabilities.Keys.Last(); // Fallback in case of rounding errors
        }

        private List<(string state, string action, double reward)> GenerateEpisode(
            GridWorld gridWorld,
            Dictionary<string, string> policy,
            List<GridAction> actions)
        {
            var episode = new List<(string state, string action, double reward)>();
             gridWorld.Reset();
             var currentState = gridWorld.State;
            var step = 0;

            while (true)
            {
                var stateString = currentState?.ToString();
                var actionName = policy[stateString];
                var action = actions.First(a => a.Name == actionName);

                var (newState, reward, done) = gridWorld.Step(action);
                episode.Add((stateString, actionName, reward));

                currentState = newState as GridState;

                // Check termination conditions
                if (step++ >= _maxSteps || done) // Terminal state
                    break;
            }

            return episode;
        }

        private List<string> GenerateStateSpace(GridWorld gridWorld)
        {
            var states = new List<string>();
            for (var x = 0; x < gridWorld.Rows; x++)
            {
                for (var y = 0; y < gridWorld.Cols; y++)
                {
                    states.Add(new GridState(x, y).ToString());
                }
            }

            return states;
        }
    }
}
