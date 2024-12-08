namespace Models;

/// <summary>
/// Represents an environment in which agents can interact.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Initializes the environment and returns the initial state.
    /// </summary>
    /// <returns>Initial state of the environment.</returns>
    IState Reset();

    /// <summary>
    /// Takes an action and returns the resulting state, reward, and if the episode is done.
    /// </summary>
    /// <param name="action">Action to be performed.</param>
    /// <returns>Tuple containing the new state, reward, and done status.</returns>
    (IState state, double reward, bool done) Step(IAction action);
}





