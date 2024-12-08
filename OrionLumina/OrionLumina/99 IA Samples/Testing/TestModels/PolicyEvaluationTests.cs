using System;
using System.Collections.Generic;
using FluentAssertions;
using Models;
using Xunit;

namespace TestModels;

public class PolicyEvaluationTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenGammaIsOutOfBounds()
    {
        // Act
        Action act1 = () => new PolicyEvaluation(-0.1, 0.01);
        Action act2 = () => new PolicyEvaluation(1.0, 0.01);

        // Assert
        act1.Should().Throw<ArgumentException>().WithMessage("*Gamma*");
        act2.Should().Throw<ArgumentException>().WithMessage("*Gamma*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenThetaIsNonPositive()
    {
        // Act
        Action act = () => new PolicyEvaluation(0.9, -0.01);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Theta*");
    }

    [Fact]
    public void EvaluatePolicy_ShouldReturnZeroValues_WhenNoRewardsOrTransitions()
    {
        // Arrange
        var states = new List<string> { "A", "B" };
        var actions = new List<string> { "action1", "action2" };
        var policy = new Dictionary<string, Dictionary<string, double>>
        {
            { "A", new Dictionary<string, double> { { "action1", 1.0 } } },
            { "B", new Dictionary<string, double> { { "action2", 1.0 } } }
        };

        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics =
            (state, action) => Array.Empty<(double, string, double)>();

        var evaluator = new PolicyEvaluation(0.9, 0.01);

        // Act
        var values = evaluator.EvaluatePolicy(states, actions, policy, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(0.0, 1e-6);
        values["B"].Should().BeApproximately(0.0, 1e-6);
    }

    [Fact]
    public void EvaluatePolicy_ShouldComputeCorrectValues_WhenDeterministicTransitions()
    {
        // Arrange
        var states = new List<string> { "A", "B", "C" };
        var actions = new List<string> { "left", "right" };
        var policy = new Dictionary<string, Dictionary<string, double>>
        {
            { "A", new Dictionary<string, double> { { "left", 1.0 } } },
            { "B", new Dictionary<string, double> { { "left", 1.0 } } },
            { "C", new Dictionary<string, double> { { "right", 1.0 } } }
        };

        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics =
            (state, action) =>
            {
                return state switch
                {
                    "A" when action == "left" => new[] { (1.0, "B", 0.0) },
                    "B" when action == "left" => new[] { (1.0, "C", 1.0) },
                    "C" when action == "right" => new[] { (1.0, "A", 0.0) },
                    _ => Array.Empty<(double, string, double)>()
                };
            };

        var evaluator = new PolicyEvaluation(0.9, 0.01);

        // Act
        var values = evaluator.EvaluatePolicy(states, actions, policy, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(0.81, 1e-2);
        values["B"].Should().BeApproximately(1.0, 1e-2);
        values["C"].Should().BeApproximately(0.729, 1e-2);
    }

    [Fact]
    public void EvaluatePolicy_ShouldConverge_WhenPolicyIsUniformAndTransitionIsRandom()
    {
        // Arrange
        var states = new List<string> { "A", "B" };
        var actions = new List<string> { "left", "right" };
        var policy = new Dictionary<string, Dictionary<string, double>>
        {
            { "A", new Dictionary<string, double> { { "left", 0.5 }, { "right", 0.5 } } },
            { "B", new Dictionary<string, double> { { "left", 0.5 }, { "right", 0.5 } } }
        };

        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics =
            (state, action) =>
            {
                return state switch
                {
                    "A" when action == "left" => new[] { (0.5, "A", 0.0), (0.5, "B", 1.0) },
                    "A" when action == "right" => new[] { (1.0, "B", 0.0) },
                    "B" when action == "left" => new[] { (1.0, "A", 1.0) },
                    "B" when action == "right" => new[] { (1.0, "A", 0.0) },
                    _ => Array.Empty<(double, string, double)>()
                };
            };

        var evaluator = new PolicyEvaluation(0.9, 0.01);

        // Act
        var values = evaluator.EvaluatePolicy(states, actions, policy, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(4.3, 1e-1); // Expected value for state A
        values["B"].Should().BeApproximately(4.87, 1e-1); // Expected value for state B
    }

    [Fact]
    public void EvaluatePolicy_ShouldHandleEdgeCase_WhenNoActionsAvailable()
    {
        // Arrange
        var states = new List<string> { "A" };
        var actions = new List<string>();
        var policy = new Dictionary<string, Dictionary<string, double>>(); // No actions defined

        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics =
            (state, action) => Array.Empty<(double, string, double)>();

        var evaluator = new PolicyEvaluation(0.9, 0.01);

        // Act
        var values = evaluator.EvaluatePolicy(states, actions, policy, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(0.0, 1e-6);
    }
}