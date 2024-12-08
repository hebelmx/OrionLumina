using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using FluentAssertions;
using Models;
using Xunit.Abstractions;
using Xunit.Sdk;

public class GridWorldPolicyEvaluationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GridWorldPolicyEvaluationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void PolicyEvaluation_ShouldConvergeToCorrectValues()
    {
        // Arrange
        var states = new List<string>
        {
                 "0", "1",   "2",   "3", 
                 "4", "5",   "6",   "7", 
                 "8", "9",   "10",  "11",
                 "12", "13", "14" , "15" 
        };

        var actions = new List<string> { "up", "down", "left", "right" };
        var gamma = 0.9; // Undiscounted task
        var theta = 0.1; // Stopping threshold

        var policy = new Dictionary<string, Dictionary<string, double>>();
        foreach (var state in states)
        {
            policy[state] = new Dictionary<string, double>
            {
                { "up", 0.25 },
                { "down", 0.25 },
                { "left", 0.25 },
                { "right", 0.25 }
            };
        }

        IEnumerable<(double probability, string nextState, double reward)> TransitionDynamics(string state, string action)
        {
            // Terminal state
            //if (state == "15") return Array.Empty<(double, string, double)>();
            // Terminal state
            //if (state == "0") return Array.Empty<(double, string, double)>();
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

            transitions.Add((1.0, nextState, nextState is "12" or "3" /*or "5" or "10" or "15"*/ ? 3 : -1));
            return transitions;
        }

        var evaluator = new PolicyEvaluation(gamma, theta);
        

        //for (int i = 0; i < 100; i++)
        //{
            var values = evaluator.EvaluatePolicy(states, actions, policy, TransitionDynamics);

        //    foreach (var (key ,value) in values)
        //    {
        //        policy[key] = value;
        //    }

        //}

        // Act
       

        // Arrange
        var expectedValues = new[]
        {
            -9, -9, -9, -8, 
            -9, -9, -8, -8, 
            -9, -9, -7, -5,
            -9, -7, -5, -0

        };




        for (int row = 0; row < 4; row++)
        {
            // Extract the current row (4 elements)
            var rowValues = values.Values.Skip(row * 4).Take(4);

            // Format each value to two decimal places
            var formattedRowValues = rowValues.Select(v => v.ToString("F2"));

            // Join the formatted values with tabs
            var rowOutput = string.Join("\t", formattedRowValues);

            // Output the formatted row
            _testOutputHelper.WriteLine(rowOutput);
        }
        // Assert
        values.Values.Zip(expectedValues, (actual, expected) => new { actual, expected })
            .Should()
            .AllSatisfy(pair => pair.actual.Should().BeApproximately(pair.expected, theta*1000));
    }
}
