﻿using FluentAssertions;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TorchSharp;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestModels;

public class MonteCarloControlTests(ITestOutputHelper testOutputHelper)
{
    const string FinalState = "3";
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
       

        static (string nextState, int step, double reward) TransitionDynamics(string state, int step, string action)
        {
            var nextState = action switch
            {
                "up" => state switch
                {
                    "0" or "1" or "2" or "3" => state,
                    _ => (int.Parse(state) - 4).ToString()
                },
                "right" => state switch
                {
                    "3" or "7" or "11" or "15" => state,
                    _ => (int.Parse(state) + 1).ToString()
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
                "stay" => state switch
                {
                    _ => (int.Parse(state) ).ToString()
                },
                _ => state
            };

            step++;

            var reward = nextState is FinalState ? 10 : -1;

            if (action == "stay" &&  nextState is FinalState)
            {
                reward+=10;
            }
            else
            {
                reward-=1;
            }

            if (action == "stay" && nextState is not FinalState)
            {
                reward -= 10;
            }
            
            return (nextState, step, reward);
        }

        

        var actions = new List<string> { "up", "down", "left", "right", "stay" };

        var monteCarlo = new MonteCarloControl(gamma: 0.9);

        // Act
        var (Q, policy) = monteCarlo.Control(states, actions, 
                                                                        TransitionDynamics, maxEpisodes: 200000);

        // Assert: Check if the policy converges to the optimal actions
        for (var row = 0; row < 4; row++)
        {
            // Initialize a list to hold the values for the current row
            var rowValues = new List<string>();

            for (var col = 0; col < 4; col++)
            {
                // Construct the key as a string and get the value from the dictionary
                var key = (row * 4 + col).ToString();
                // Get the value from the dictionary or use "N/A" if the key is missing
                var value = policy.GetValueOrDefault(key, "N/A");

                // Format the value to a fixed width of 8 characters
                rowValues.Add(value.PadRight(8));
            }

            // Join the row values without extra tab spacing (already fixed width)
            var rowOutput = string.Join(string.Empty, rowValues);

            // Output the row
            testOutputHelper.WriteLine(rowOutput);
        }

    }
}
