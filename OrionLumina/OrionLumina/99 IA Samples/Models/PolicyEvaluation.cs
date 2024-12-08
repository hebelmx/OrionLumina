using System;
using System.Collections.Generic;

namespace Models;

public class PolicyEvaluation
{
    private readonly double _gamma; // Discount factor
    private readonly double _theta; // Threshold for convergence

    public PolicyEvaluation(double gamma, double theta)
    {
        if (gamma is <= 0 or >= 1)
            throw new ArgumentException("Gamma must be between 0 and 1 (exclusive).");
        if (theta <= 0)
            throw new ArgumentException("Theta must be a small positive number.");

        _gamma = gamma;
        _theta = theta;
    }

    /// <summary>
    /// Evaluates the policy and returns the value function V(s).
    /// </summary>
    /// <param name="states">The list of states in the environment.</param>
    /// <param name="actions">The list of actions available.</param>
    /// <param name="policy">A dictionary mapping state to action probabilities.</param>
    /// <param name="transitionDynamics">Function returning (probability, next state, reward) given (state, action).</param>
    /// <returns>Dictionary representing V(s) for each state.</returns>
    public Dictionary<string, double> EvaluatePolicy(
        List<string> states,
        List<string> actions,
        Dictionary<string, Dictionary<string, double>> policy,
        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics)
    {
        // Initialize V(s) = 0 for all states
        var V = new Dictionary<string, double>();
        foreach (var state in states)
        {
            V[state] = 0.0;
        }

        double delta;

        do
        {
            delta = 0;

            // Iterate over each state
            foreach (var state in states)
            {
                var v = V[state]; // Store the current value of the state

                // Compute the new value of V(s) based on the policy
                var newValue = 0.0;
                foreach (var action in actions)
                {
                    // π(a|s) - Probability of taking action `a` in state `s` under the policy
                    if (!policy[state].TryGetValue(action, out var actionProbability))
                        continue; // Skip actions not part of the policy

                    // Transition dynamics: p(s', r | s, a)
                    
                    foreach (var (probability, nextState, reward) in transitionDynamics(state, action))
                    {
                        //if (nextState != "0" || nextState != "15")
                        //{
                            newValue += actionProbability * probability * (reward + _gamma * V[nextState]);
                        //}
                        
                    }
                }

                V[state] = newValue; // Update the value of the state
                delta = Math.Max(delta, Math.Abs(v - V[state])); // Track the maximum change
            }

        } while (delta >= _theta/100); // Repeat until convergence

        return V;
    }
}


