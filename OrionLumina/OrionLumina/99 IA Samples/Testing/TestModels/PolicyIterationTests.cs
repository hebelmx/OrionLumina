using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Models;
using Xunit;

namespace TestModels;

public class PolicyIterationTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenGammaIsOutOfBounds()
    {
        // Act
        Action act1 = () => new PolicyIteration(-0.1, 0.01);
        Action act2 = () => new PolicyIteration(1.1, 0.01);

        // Assert
        act1.Should().Throw<ArgumentException>().WithMessage("*Gamma*");
        act2.Should().Throw<ArgumentException>().WithMessage("*Gamma*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenThetaIsNonPositive()
    {
        // Act
        Action act = () => new PolicyIteration(0.9, -0.01);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Theta*");
    }

    [Fact]
    public void PolicyIteration_ShouldConvergeToOptimalPolicy_WhenDeterministicTransitions()
    {
        // Arrange
        var states = new List<string> { "A", "B", "C" };
        var actions = new List<string> { "left", "right" };

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

        var policyIteration = new PolicyIteration(0.9, 0.01);

        // Act
        var (values, policy) = policyIteration.Iterate(states, actions, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(0.81, 1e-2);
        values["B"].Should().BeApproximately(1.0, 1e-2);
        values["C"].Should().BeApproximately(0.729, 1e-2);

        policy["A"].Should().Be("left");
        policy["B"].Should().Be("left");
        policy["C"].Should().Be("right");
    }

    [Fact]
    public void PolicyIteration_ShouldConvergeToOptimalPolicy_WhenUniformTransitions()
    {
        // Arrange
        var states = new List<string> { "1", "2", "3", "T" };
        var actions = new List<string> { "left", "right" };

        static IEnumerable<(double probability, string nextState, double reward)> transitionDynamics(string state, string action)
        {
            if (state == "T") return Array.Empty<(double, string, double)>(); // Terminal state

            return state switch
            {
                "1" => action == "right" ? [(1.0, "2", -1)] : [(1.0, "1", -1)],
                "2" => action == "right" ? [(1.0, "3", -1)] : [(1.0, "1", -1)],
                "3" => action == "right" ? [(1.0, "T", 0)] : [(1.0, "2", -1)],
                _ => Array.Empty<(double, string, double)>()
            };
        }

        var policyIteration = new PolicyIteration(1.0, 0.01);

        // Act
        var (values, policy) = policyIteration.Iterate(states, actions, transitionDynamics);

        // Assert
        values["1"].Should().BeApproximately(-2.0, 1e-2);
        values["2"].Should().BeApproximately(-1.0, 1e-2);
        values["3"].Should().BeApproximately(0.0, 1e-2);
        values["T"].Should().Be(0.0);

        policy["1"].Should().Be("right");
        policy["2"].Should().Be("right");
        policy["3"].Should().Be("right");
    }

    [Fact]
    public void PolicyIteration_ShouldHandleNoTransitions()
    {
        // Arrange
        var states = new List<string> { "A" };
        var actions = new List<string> { "noop" };

        Func<string, string, IEnumerable<(double probability, string nextState, double reward)>> transitionDynamics =
            (state, action) => Array.Empty<(double, string, double)>();

        var policyIteration = new PolicyIteration(0.9, 0.01);

        // Act
        var (values, policy) = policyIteration.Iterate(states, actions, transitionDynamics);

        // Assert
        values["A"].Should().BeApproximately(0.0, 1e-6);
        policy["A"].Should().Be("noop");
    }
}
