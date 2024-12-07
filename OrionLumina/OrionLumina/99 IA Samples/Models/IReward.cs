namespace Models;

/// <summary>
/// Represents a reward signal from the environment.
/// </summary>
public interface IReward
{
    /// <summary>
    /// Gets the value of the reward.
    /// </summary>
    double Value { get; }
}