using BalancingSimulation;
using FluentAssertions;
using Xunit;

namespace TestModels.Tests
{
    public class StateTests
    {
        [Fact]
        public void StableState_ShouldRemainStable_WhenAngleIsWithinThreshold()
        {
            // Arrange
            var stableState = new StableState();
            var angle = 0.1; // Within threshold
            var angleThreshold = 0.5;

            // Act
            var newState = stableState.Transition(angle, angleThreshold);

            // Assert
            newState.Should().BeOfType<StableState>();
        }

        [Fact]
        public void StableState_ShouldTransitionToUnstable_WhenAngleExceedsThreshold()
        {
            // Arrange
            var stableState = new StableState();
            var angle = 0.6; // Exceeds threshold
            var angleThreshold = 0.5;

            // Act
            var newState = stableState.Transition(angle, angleThreshold);

            // Assert
            newState.Should().BeOfType<UnstableState>();
        }

        [Fact]
        public void UnstableState_ShouldTransitionToFallen_WhenAngleExceedsDoubleThreshold()
        {
            // Arrange
            var unstableState = new UnstableState();
            var angle = 1.1; // Exceeds double threshold
            var angleThreshold = 0.5;

            // Act
            var newState = unstableState.Transition(angle, angleThreshold);

            // Assert
            newState.Should().BeOfType<FallenState>();
        }
    }
}