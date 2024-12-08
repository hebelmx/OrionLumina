using System;
using System.Collections.Generic;
using Models.Models;

namespace Models;

public class GridWorld : IEnvironment
{
    private readonly int _rows;
    private readonly int _cols;
    private readonly GridState _startState;
    private readonly GridState _endState;
    private readonly Dictionary<(int x, int y), double> _rewards;
    private readonly HashSet<(int x, int y)> _blockedCells;
    private GridState _currentState;

    /// <summary>
    /// Initializes a new instance of the GridWorld environment.
    /// </summary>
    public GridWorld(int rows, int cols, GridState startState, GridState endState,
        Dictionary<(int x, int y), double> rewards = null,
        HashSet<(int x, int y)> blockedCells = null)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException("Grid dimensions must be positive.");

        _rows = rows;
        _cols = cols;
        _startState = startState ?? throw new ArgumentNullException(nameof(startState));
        _endState = endState ?? throw new ArgumentNullException(nameof(endState));
        _rewards = rewards ?? new Dictionary<(int x, int y), double>();
        _blockedCells = blockedCells ?? new HashSet<(int x, int y)>();
        _currentState = _startState;
    }

    public IState Reset()
    {
        _currentState = _startState;
        return _currentState;
    }

    public (IState state, double reward, bool done) Step(IAction action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        var (newX, newY) = GetNextPosition(_currentState, action);

        if (_blockedCells.Contains((newX, newY)))
        {
            // Negative reward for hitting a blocked cell or staying in place
            return (_currentState, -1.0, false);
        }

        if (!IsInBounds(newX, newY))
        {
            // Negative reward for trying to leave the grid
            return (_currentState, -1.0, false);
        }

        _currentState = new GridState(newX, newY);

        // Check if we reached the end state
        var done = _currentState.X == _endState.X && _currentState.Y == _endState.Y;
        var reward = _rewards.GetValueOrDefault((newX, newY), 0.0);

        return (_currentState, reward, done);
    }

    private (int x, int y) GetNextPosition(IState currentState, IAction action)
    {
        int x = currentState.X, y = currentState.Y;

        return action switch
        {
            { Name: "Up" } => (x - 1, y),
            { Name: "Down" } => (x + 1, y),
            { Name: "Left" } => (x, y - 1),
            { Name: "Right" } => (x, y + 1),
            _ => throw new ArgumentException("Invalid action.")
        };
    }

    private bool IsInBounds(int x, int y) => x >= 0 && x < _rows && y >= 0 && y < _cols;
}