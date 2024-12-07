namespace Models;

/// <summary>
/// Represents an agent that interacts with an environment.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Chooses an action based on the current state.
    /// </summary>
    /// <param name="state">The current state of the environment.</param>
    /// <returns>An action chosen by the agent.</returns>
    IAction ChooseAction(IState state);

    /// <summary>
    /// Updates the agent's policy based on experience.
    /// </summary>
    /// <param name="state">The state before the action.</param>
    /// <param name="action">The action taken.</param>
    /// <param name="reward">The reward received.</param>
    /// <param name="nextState">The state after the action.</param>
    /// <param name="done">Whether the episode has ended.</param>
    void Learn(IState state, IAction action, double reward, IState nextState, bool done);
}