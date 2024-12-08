using System;
using System.Collections.Generic;
using FluentAssertions;
using Models;
using Models.Models;
using Xunit;

namespace TestModels;

public class GridWorldTests
{
    [Fact]
    public void Reset_ShouldReturnStartState()
    {
        // Arrange
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState);

        // Act
        var initialState = gridWorld.Reset();

        // Assert
        initialState.Should().BeEquivalentTo(startState);
    }

    [Fact]
    public void Step_ShouldMoveToNextState_WhenActionIsValid()
    {
        // Arrange
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState);

        gridWorld.Reset();

        // Act
        var (state, reward, done) = gridWorld.Step(GridAction.Right);

        // Assert
        state.Should().BeEquivalentTo(new GridState(0, 1));
        reward.Should().Be(0.0);
        done.Should().BeFalse();
    }

    [Fact]
    public void Step_ShouldStayInPlace_WhenBlockedCell()
    {
        // Arrange
        var blockedCells = new HashSet<(int x, int y)> { (0, 1) };
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState, blockedCells: blockedCells);

        gridWorld.Reset();

        // Act
        var (state, reward, done) = gridWorld.Step(GridAction.Right);

        // Assert
        state.Should().BeEquivalentTo(startState); // Remains in the same state
        reward.Should().Be(-1.0); // Penalty for hitting blocked cell
        done.Should().BeFalse();
    }

    [Fact]
    public void Step_ShouldStayInPlace_WhenOutOfBounds()
    {
        // Arrange
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState);

        gridWorld.Reset();

        // Act
        var (state, reward, done) = gridWorld.Step(GridAction.Up);

        // Assert
        state.Should().BeEquivalentTo(startState); // Remains in the same state
        reward.Should().Be(-1.0); // Penalty for moving out of bounds
        done.Should().BeFalse();
    }

    [Fact]
    public void Step_ShouldReturnEndState_WhenGoalIsReached()
    {
        // Arrange
        var startState = new GridState(0, 0);
        var endState = new GridState(1, 1);
        var gridWorld = new GridWorld(3, 3, startState, endState);

        gridWorld.Reset();

        // Act
        gridWorld.Step(GridAction.Right);
        var (state, reward, done) = gridWorld.Step(GridAction.Down);

        // Assert
        state.Should().BeEquivalentTo(endState); // Reached end state
        reward.Should().Be(0.0); // No specific reward defined for end state
        done.Should().BeTrue(); // Episode ends
    }

    [Fact]
    public void Step_ShouldReturnCorrectReward_WhenEnteringRewardCell()
    {
        // Arrange
        var rewards = new Dictionary<(int x, int y), double>
        {
            { (1, 1), 1.0 }
        };
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState, rewards: rewards);

        gridWorld.Reset();

        // Act
        gridWorld.Step(GridAction.Right);
        var (state, reward, done) = gridWorld.Step(GridAction.Down);

        // Assert
        state.Should().BeEquivalentTo(new GridState(1, 1));
        reward.Should().Be(1.0); // Reward for entering (1, 1)
        done.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenGridDimensionsAreInvalid()
    {
        // Act
        Action act = () => new GridWorld(-1, 3, new GridState(0, 0), new GridState(2, 2));

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Grid dimensions must be positive.");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenStartOrEndStateIsNull()
    {
        // Act
        Action actStartNull = () => new GridWorld(3, 3, null, new GridState(2, 2));
        Action actEndNull = () => new GridWorld(3, 3, new GridState(0, 0), null);

        // Assert
        actStartNull.Should().Throw<ArgumentNullException>().WithMessage("*startState*");
        actEndNull.Should().Throw<ArgumentNullException>().WithMessage("*endState*");
    }

    [Fact]
    public void Reset_ShouldReinitializeToStartState()
    {
        // Arrange
        var startState = new GridState(0, 0);
        var endState = new GridState(2, 2);
        var gridWorld = new GridWorld(3, 3, startState, endState);

        gridWorld.Reset();
        gridWorld.Step(GridAction.Right); // Move from start state

        // Act
        var resetState = gridWorld.Reset();

        // Assert
        resetState.Should().BeEquivalentTo(startState); // Should reset to start state
    }
}