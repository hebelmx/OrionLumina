namespace Models;

/// <summary>
/// Represents a state in the environment.
/// </summary>
public interface IState
{
    /// <summary>
    /// Retrieves the features or observations of the state.
    /// </summary>
    double[] Features { get; }
}