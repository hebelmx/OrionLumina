using System;
using System.Collections.Generic;
using System.Linq;

namespace Models;

public class PolicyIteration
{
    private readonly double _gamma; // Discount factor
    private readonly double _theta; // Threshold for convergence

    public PolicyIteration(double gamma, double theta)
    {
        if (gamma is <= 0 or > 1)
            throw new ArgumentException("Gamma must be in the range (0, 1].");
        if (theta <= 0)
            throw new ArgumentException("Theta must be a small positive number.");

        _gamma = gamma;
        _theta = theta;
    }

    public (Dictionary<string, double> V, Dictionary<string, string> Policy) Iterate(
        List<string> states,
        List<string> actions,
        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics)
    {
        // 1. Initialization
        var v = states.ToDictionary(s => s, s => 0.0); // Initialize V(s) arbitrarily
        var policy = states.ToDictionary(s => s, s => actions[0]); // Initialize π(s) arbitrarily

        bool policyStable;

        do
        {
            // 2. Policy Evaluation
            PolicyEvaluation(states, policy, transitionDynamics, v);

            // 3. Policy Improvement
            policyStable = true;

            foreach (var state in states)
            {
                var oldAction = policy[state];
                string bestAction = null;
                var bestValue = double.MinValue;

                foreach (var action in actions)
                {
                    var actionValue = 0.0;

                    foreach (var (probability, nextState, reward) in transitionDynamics(state, action))
                    {
                        actionValue += probability * (reward + _gamma * v[nextState]);
                    }

                    if (!(actionValue > bestValue)) continue;
                    bestValue = actionValue;
                    bestAction = action;
                }

                policy[state] = bestAction;

                if (oldAction != bestAction)
                {
                    policyStable = false;
                }
            }

        } while (!policyStable); // Continue until policy is stable

        return (v, policy);
    }

    private void PolicyEvaluation(
        List<string> states,
        Dictionary<string, string> policy,
        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics,
        Dictionary<string, double> v)
    {
        double delta;
        do
        {
            delta = 0;

            foreach (var state in states)
            {
                var oldValue = v[state];
                var newValue = 0.0;

                var action = policy[state];
                foreach (var (probability, nextState, reward) in transitionDynamics(state, action))
                {
                    newValue += probability * (reward + _gamma * v[nextState]);
                }

                v[state] = newValue;
                delta = Math.Max(delta, Math.Abs(oldValue - newValue));
            }

        } while (delta >= _theta/100); // Continue until V(s) converges
    }
}