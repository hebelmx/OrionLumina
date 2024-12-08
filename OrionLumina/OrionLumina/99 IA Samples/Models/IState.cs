namespace Models;

/// <summary>
/// Represents a state in the environment.
/// </summary>
public interface IState
{
    /// <summary>
    /// Retrieves the features or observations of the state.
    /// </summary>

    public int X { get;  set; }
    public int Y { get;  set; }
}