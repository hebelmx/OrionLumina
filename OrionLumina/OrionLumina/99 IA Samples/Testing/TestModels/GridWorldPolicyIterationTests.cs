using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using FluentAssertions;

namespace TestModels;


public class GridWorldPolicyIterationTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void PolicyIteration_ShouldConvergeToOptimalValuesAndPolicy()
    {
        // Arrange
        var states = new List<string>
        {
            "0", "1",  "2",  "3",
            "4", "5",  "6",  "7",
            "8", "9",  "10", "11",
            "12", "13", "14", "15"
        };

        var actions = new List<string> { "up", "down", "left", "right" };
        var gamma = 0.9; // Discount factor
        var theta = 0.1; // Convergence threshold

        IEnumerable<(double probability, string nextState, double reward)> TransitionDynamics(string state, string action)
        {
            // Determine next state and reward
            var transitions = new List<(double probability, string nextState, double reward)>();
            var nextState = action switch
            {
                "up" => state switch
                {
                    "0" or "1" or "2" or "3" => state,
                    _ => (int.Parse(state) - 4).ToString()
                },
                "down" => state switch
                {
                    "12" or "13" or "14" or "15" => state,
                    _ => (int.Parse(state) + 4).ToString()
                },
                "left" => state switch
                {
                    "0" or "4" or "8" or "12" => state,
                    _ => (int.Parse(state) - 1).ToString()
                },
                "right" => state switch
                {
                    "3" or "7" or "11" or "15" => state,
                    _ => (int.Parse(state) + 1).ToString()
                },
                _ => state
            };

            var reward = nextState is "3" ? 3 : -1;
            transitions.Add((1.0, nextState, reward));
            return transitions;
        }

        var policyIteration = new PolicyIteration(gamma, theta);

        // Act
        var (values, policy) = policyIteration.Iterate(states, actions, TransitionDynamics);

        // Expected values based on the gridworld rules
        var expectedValues = new[]
        {
            -9, -9, -9, -8,
            -9, -9, -8, -8,
            -9, -9, -7, -5,
            -9, -7, -5,  0
        };

        // Output the resulting values as a 4x4 matrix
        for (int row = 0; row < 4; row++)
        {
            var rowValues = values.Values.Skip(row * 4).Take(4);
            var formattedRowValues = rowValues.Select(v => v.ToString("F2"));
            var rowOutput = string.Join("\t", formattedRowValues);

            var rowPolicies = policy.Values.Skip(row * 4).Take(4);
            var rowPolicy = string.Join("\t", rowPolicies);
            testOutputHelper.WriteLine(rowPolicy);
        }

        // Assert that values are correct
        values.Values.Zip(expectedValues, (actual, expected) => new { actual, expected })
            .Should()
            .AllSatisfy(pair => pair.actual.Should().BeApproximately(pair.expected, theta * 1000));

        // Assert that the policy is optimal
        
    }
}
