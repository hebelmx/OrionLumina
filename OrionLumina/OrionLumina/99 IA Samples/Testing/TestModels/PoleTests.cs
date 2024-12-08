using BalancingSimulation;
using FluentAssertions;
using Xunit;

namespace TestModels.Tests
{
    public class PoleTests
    {
        [Fact]
        public void Pole_ShouldRemainStable_WhenTorqueBalancesGravity()
        {
            // Arrange
            var length = 1.0;
            var mass = 0.1;
            var angleThreshold = 0.5;
            var pole = new Pole(length, mass, angleThreshold);

            // Act
            var torque = 0.0; // No external force
            pole.Update(torque, 0.1); // Small time step

            // Assert
            pole.CurrentState.Should().BeOfType<StableState>();
            pole.Angle.Should().BeApproximately(0.0, 0.01);
            pole.AngularVelocity.Should().BeApproximately(0.0, 0.01);
        }

        [Fact]
        public void Pole_ShouldTransitionToUnstable_WhenTorqueIncreasesAngleBeyondThreshold()
        {
            // Arrange
            var length = 1.0;
            var mass = 0.1;
            var angleThreshold = 0.5;
            var pole = new Pole(length, mass, angleThreshold);

            // Act
            var torque = 10.0; // High torque
            pole.Update(torque, 0.1); // Small time step

            // Assert
            pole.CurrentState.Should().BeOfType<UnstableState>();
            pole.Angle.Should().BeGreaterThan(angleThreshold);
        }
    }
}