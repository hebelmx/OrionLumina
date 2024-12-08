using FluentAssertions;
using Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace TestModels;

public class MonteCarloControlTests
{
    [Fact]
    public void MonteCarloControl_ShouldConvergeToOptimalPolicy()
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

        var monteCarlo = new MonteCarloControl(gamma: 0.9);

        // Act
        var (Q, policy) = monteCarlo.Control(states, actions, 
            TransitionDynamics, maxEpisodes: 20);

        // Assert: Check if the policy converges to the optimal actions
        foreach (var state in states)
        {
            if (state is "0" or "15" ) continue; // Terminal states
            var expectedAction = state switch
            {
                "1" or "2" => "left",
                "4" or "8" => "up",
                "13" or "14" => "right",
                "7" or "11" => "down",
                _ => string.Empty
            };
            policy[state].Should().Contain(expectedAction);
        }

        return;

        static (string nextState, double reward) TransitionDynamics(string state, string action)
        {
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

            var reward = nextState is "0" or "15" ? 3 : -1;
            return (nextState, reward);
        }
    }
}
