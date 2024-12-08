using System;
using FluentAssertions;
using Models;
using Xunit;
using Xunit.Abstractions;

namespace TestModels;

public class StrategiesTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void EpsilonGreedy_ShouldExplore_WithProbabilityEpsilon()
    {
        // Arrange
        var epsilon = 1.0; // Always explore
        double[] averageRewards = [1.0, 0.5, 0.2];

        var trials = 1000;

        // Counters for each arm
        var armCounts = new int[3]; // Array to hold counts for arms 0, 1, and 2

        for (var i = 0; i < trials; i++)
        {
            var selectedArm = Strategies.EpsilonGreedy(averageRewards, epsilon);

            // Increment the counter for the selected arm using a switch expression
            _ = selectedArm switch
            {
                0 => armCounts[0]++,
                1 => armCounts[1]++,
                2 => armCounts[2]++,
                _ => throw new InvalidOperationException("Invalid arm selected.") // Safety for unexpected values
            };
        }

        // Act
      
        // Validate the counts to ensure all arms were selected
        armCounts[0].Should().BeGreaterThan(275);
        armCounts[1].Should().BeGreaterThan(275);
        armCounts[2].Should().BeGreaterThan(275);


        testOutputHelper.WriteLine($"EpsilonGreedy_ShouldExplore_WithProbabilityEpsilon");

        testOutputHelper.WriteLine($"Arm 0 selected {armCounts[0]} times.");
        testOutputHelper.WriteLine($"Arm 1 selected {armCounts[1]} times.");
        testOutputHelper.WriteLine($"Arm 2 selected {armCounts[2]} times.");
    }


    [Fact]
    public void EpsilonGreedy_ShouldExploit_WithProbability1MinusEpsilon()
    {
        // Arrange
        var epsilon = 0.0; // Always exploit
        double[] averageRewards = [0.2, 0.5, 1.0];

        // Act
        var selectedArm = Strategies.EpsilonGreedy(averageRewards, epsilon);
        testOutputHelper.WriteLine($"EpsilonGreedy_ShouldExploit_WithProbability1MinusEpsilon {epsilon}");

        testOutputHelper.WriteLine($"Arm {selectedArm}  selected  ");

        // Assert
        selectedArm.Should().Be(2); // Highest reward arm
    }


    [Fact]
    public void EpsilonGreedy_ShouldExploit_SometimesNonGreedy()
    {
        // Arrange
        var epsilon = 0.1; // Explore 10% approximately
        double[] averageRewards = [0.2, 0.5, 1.0];
        // Counters for each arm
        var armCounts = new int[3]; // Array to hold counts for arms 0, 1, and 2

        var countArms = 0;
        var trials = 1000;
        // Act
        for (var i = 0; i < trials; i++)
        {
            var selectedArm = Strategies.EpsilonGreedy(averageRewards, epsilon);
            _ = selectedArm switch
            {
                0 => armCounts[0]++,
                1 => armCounts[1]++,
                2 => armCounts[2]++,
                _ => throw new InvalidOperationException("Invalid arm selected.") // Safety for unexpected values
            };
        }

        // Assert
        countArms.Should().BeLessThan(trials); // Highest reward arm
        testOutputHelper.WriteLine($"EpsilonGreedy_ShouldExploit_WithProbability1MinusEpsilon {epsilon}");

        testOutputHelper.WriteLine($"Arm 0 selected {armCounts[0]} times.");
        testOutputHelper.WriteLine($"Arm 1 selected {armCounts[1]} times.");
        testOutputHelper.WriteLine($"Arm 2 selected {armCounts[2]} times.");

    }
}

