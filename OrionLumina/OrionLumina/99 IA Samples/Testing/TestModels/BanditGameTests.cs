using FluentAssertions;
using Models;
using Xunit;
namespace TestModels
{
    public class BanditGameTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRewardsAndPullCounts()
        {
            // Arrange
            var probabilities = new[] { 0.1, 0.5, 0.7 };

            // Act
            var game = new BanditGame(probabilities);

            // Assert
            game.PullCounts.Should().AllBeEquivalentTo(0);
            game.Rewards.Should().AllBeEquivalentTo(0.0);
        }

        [Fact]
        public void PullArm_ShouldUpdateCountsAndRewards()
        {
            // Arrange
            var game = new BanditGame([1.0]); // Always rewards

            // Act
            var reward = game.PullArm(0);

            // Assert
            game.PullCounts[0].Should().Be(1);
            game.Rewards[0].Should().Be(1.0);
            reward.Should().Be(1);
        }

        [Fact]
        public void GetAverageRewards_ShouldReturnCorrectAverages()
        {
            // Arrange
            var game = new BanditGame([1.0, 0.5]);
            game.PullArm(0); // Always rewards

            for (var i = 0; i < 1000; i++)
            {
                game.PullArm(0); // Always rewards
                game.PullArm(1); // Random, assume reward = 0
            }
            
            // Act
            var averages = game.GetAverageRewards();

            // Assert
            averages[0].Should().Be(1.0); // Arm 0 average is 1
            averages[1].Should().BeApproximately(0.5, 0.05);
        }
    }
}