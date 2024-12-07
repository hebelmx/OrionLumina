using System;
using FluentAssertions;
using Models.NArmBandit;
using Xunit;
namespace TestModels
{
    namespace NArmBandit.Tests
    {
        public class BanditArmTests
        {
            [Fact]
            public void Constructor_ShouldThrow_WhenProbabilityIsOutOfRange()
            {
                // Act
                Action act = () => new BanditArm(-0.1);

                // Assert
                act.Should().Throw<ArgumentOutOfRangeException>();
            }

            [Fact]
            public void Constructor_ShouldInitialize_WithValidProbability()
            {
                // Act
                var arm = new BanditArm(0.5);

                // Assert
                arm.SuccessProbability.Should().Be(0.5);
            }

            [Fact]
            public void Pull_ShouldReturnValidRewards()
            {
                // Arrange
                var arm = new BanditArm(1.0);

                // Act
                var reward = arm.Pull();

                // Assert
                reward.Should().Be(1); // Probability of success is 100%
            }
        }
    }

}
