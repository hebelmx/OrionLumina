using System;
using System.Collections.Generic;
using System.Linq;

namespace Models;

public class MonteCarloControl
{
    private readonly Random _random = new();
    private readonly double _gamma; // Discount factor

    public MonteCarloControl(double gamma)
    {
        if (gamma is <= 0 or > 1)
            throw new ArgumentException("Gamma must be in the range (0, 1].");

        _gamma = gamma;
    }

    public (Dictionary<string, Dictionary<string, double>> Q, Dictionary<string, string> Policy) Control(
        List<string> states,
        List<string> actions,
        Func<string, string, (string nextState, double reward)> transitionDynamics,
        int maxEpisodes)
    {
        // Initialize Q(s, a) arbitrarily and policy π(s) arbitrarily
        var q = states.ToDictionary(
            s => s,
            s => actions.ToDictionary(a => a, a => 0.0)
        );
        var policy = states.ToDictionary(
            s => s, 
            s => actions[_random.Next(actions.Count)]
            );

        // Returns(s, a): stores all returns for a given state-action pair
        var returns = states.ToDictionary(
            s => s, // The state is the key
            s => actions.ToDictionary(a => a, a => new List<double>()) // The value is an inner dictionary
        );
        

        for (var episode = 0; episode < maxEpisodes; episode++)
        {
            // Generate an episode
            var episodeSteps = GenerateEpisode(states, actions, policy, transitionDynamics);

            // Process the episode
            var seenPairs = new HashSet<(string state, string action)>();
            var g = 0.0; // Return

            for (var t = episodeSteps.Count - 1; t >= 0; t--)
            {
                var (state, action, reward) = episodeSteps[t];
                g = reward + _gamma * g; // Accumulate reward

                // Ensure the outer dictionary exists for the state
                if (!returns.ContainsKey(state))
                {
                    returns[state] = [];
                }

                // Ensure the inner dictionary exists for the action
                if (!returns[state].ContainsKey(action))
                {
                    returns[state][action] = [];
                }

                // Add the return G to the list
                returns[state][action].Add(g);

                // Update Q(s, a) as the average of the returns
                q[state][action] = returns[state][action].Average();
            }

            // Policy Improvement
            foreach (var state in states)
            {
                policy[state] = actions.OrderByDescending(a => q[state][a]).First();
            }
        }

        return (q, policy);
    }

    private List<(string state, string action, double reward)> GenerateEpisode(
        List<string> states,
        List<string> actions,
        Dictionary<string, string> policy,
        Func<string, string, (string nextState, double reward)> transitionDynamics)
    {
        var episode = new List<(string state, string action, double reward)>();

        // Randomly initialize the starting state and action
        var state = states[_random.Next(states.Count)];
        var action = actions[_random.Next(actions.Count)];

        while (true)
        {
            var (nextState, reward) = transitionDynamics(state, action);
            episode.Add((state, action, reward));

            if (nextState == state) // Terminal state
                break;
            if (reward > 0)
                break;

            state = nextState;
            action = policy[state];
        }

        return episode;
    }
}