using System;
using System.Collections.Generic;
using System.Linq;

namespace Models;

public class MonteCarloESoftControl
{
    private readonly Random _random = new();
    private readonly double _gamma; // Discount factor
    private readonly double _epsilon; // Exploration factor
    private readonly int _maxSteps;

    public MonteCarloESoftControl(double gamma, double epsilon, int maxSteps = 10000)
    {
        if (gamma is <= 0 or > 1)
            throw new ArgumentException("Gamma must be in the range (0, 1].");
        if (epsilon is < 0 or > 1)
            throw new ArgumentException("Epsilon must be in the range [0, 1].");

        _gamma = gamma;
        _epsilon = epsilon;
        _maxSteps = maxSteps;
    }

    public (Dictionary<string, Dictionary<string, double>> Q, Dictionary<string, string> Policy) Control(
        List<string> states,
        string endState,
        List<string> actions,
        Func<string, int, string, string, (string nextState, int step, double reward)> transitionDynamics,
        int maxEpisodes)
    {
        // Initialize Q(s, a) arbitrarily and policy π(s) arbitrarily
        var q = states.ToDictionary(
            s => s,
            s => actions.ToDictionary(a => a, a => 0.0)
        );
        var policy = InitializePolicy(states, actions);

        // Returns(s, a): stores all returns for a given state-action pair
        var returns = states.ToDictionary(
            s => s,
            s => actions.ToDictionary(a => a, a => new List<double>())
        );

        for (var episode = 0; episode < maxEpisodes; episode++)
        {
            // Generate an episode
            var episodeSteps = GenerateEpisode(states, endState, policy, transitionDynamics);

            // Process the episode
            var seenPairs = new HashSet<(string state, string action)>();
            var g = 0.0; // Return

            for (var t = episodeSteps.Count - 1; t >= 0; t--)
            {
                var (state, action, reward) = episodeSteps[t];
                g = reward + _gamma * g; // Accumulate reward

                // Skip if this state-action pair has already been seen in this episode
                if (!seenPairs.Add((state, action)))
                    continue;

                returns[state][action].Add(g);

                // Update Q(s, a) as the average of the returns
                q[state][action] = returns[state][action].Average();
            }

            // Policy Improvement: Update policy to be ε-soft
            policy = UpdatePolicy(states, actions, q, _epsilon);
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
                    ? 1.0 - epsilon + (epsilon / actions.Count)
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
        List<string> states,
        string endState,
        Dictionary<string, string> policy,
        Func<string, int, string, string, (string nextState, int step, double reward)> transitionDynamics)
    {
        var episode = new List<(string state, string action, double reward)>();
        var state = states[_random.Next(states.Count)];
        var step = 0;

        while (true)
        {
            var action = policy[state];
            var (nextState, nextStep, reward) = transitionDynamics(state, step, endState, action);

            episode.Add((state, action, reward));

            step = nextStep;
            state = nextState;

            // Check termination conditions
            if (step >= _maxSteps || reward > 100 || state == endState) // Terminal state
                break;
        }

        return episode;
    }
}
