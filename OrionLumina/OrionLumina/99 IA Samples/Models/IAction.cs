namespace Models;

/// <summary>
/// Represents an action in the environment.
/// </summary>
public interface IAction
{
    /// <summary>
    /// Gets the action's identifier.
    /// </summary>
    int ActionId { get; }
}