using System.Collections.Generic;
using BalancingSimulation;
using FluentAssertions;
using Xunit;

namespace TestModels.Tests
{
    public class MultiPoleSystemTests
    {
        [Fact]
        public void MultiPoleSystem_ShouldStartAllPolesInStableState()
        {
            // Arrange
            var poles = new List<(double Length, double Mass, double AngleThreshold)>
            {
                (1.0, 0.1, 0.5), // Pole 1
                (1.0, 0.1, 0.5)  // Pole 2
            };
            var system = new MultiPoleSystem(1.0, poles);

            // Act
            var states = system.GetStates();

            // Assert
            states.Should().AllBeEquivalentTo("Stable");
        }

        [Fact]
        public void MultiPoleSystem_ShouldUpdatePolesIndependently()
        {
            // Arrange
            var poles = new List<(double Length, double Mass, double AngleThreshold)>
            {
                (1.0, 0.1, 0.5), // Pole 1
                (1.0, 0.1, 0.5)  // Pole 2
            };
            var system = new MultiPoleSystem(1.0, poles);

            // Act
            system.Update(10.0, 0.1); // Apply force
            var states = system.GetStates();

            // Assert
            states.Should().Contain(state => state == "Unstable");
        }

        [Fact]
        public void MultiPoleSystem_ShouldTransitionToFallen_WhenAllPolesExceedDoubleThreshold()
        {
            // Arrange
            var poles = new List<(double Length, double Mass, double AngleThreshold)>
            {
                (1.0, 0.1, 0.5), // Pole 1
                (1.0, 0.1, 0.5)  // Pole 2
            };
            var system = new MultiPoleSystem(1.0, poles);

            // Act
            system.Update(100.0, 0.5); // Apply large force
            var states = system.GetStates();

            // Assert
            states.Should().AllBeEquivalentTo("Fallen");
        }
    }
}