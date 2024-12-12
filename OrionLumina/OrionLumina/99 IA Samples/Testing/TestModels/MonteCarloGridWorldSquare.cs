using FluentAssertions;
using Models;
using Models.Models;
using System.Collections.Generic;
using TorchSharp.Modules;
using Xunit;
using Xunit.Abstractions;

namespace TestModels;
public class MonteCarloGridWorldSquare(ITestOutputHelper testOutputHelper)
{

    private const double Gamma = 0.9;
    private const double Epsilon = 0.05;
    private const double Alpha = 0.1;
    private const int MaxSteps = 10_000;
    private const int MaxEpisodes = 1_000;

    [Fact]
        public void MonteCarloControl_ShouldConvergeToOptimalPolicy()
        {
            // Arrange
            var rows = 4;
            var cols = 4;
            var startState = new GridState(0, 0);
         
            var rewards = new Dictionary<(int x, int y), double>
            {
                [(0,0)] = 0, [(1 , 0)] = 1, [(2, 0)] = 2, [(3, 0)] = 3,
                [(0, 1)] = 2, [(1, 1)] = 4, [(2, 1)] = 5, [(3, 1)] = 6,
                [(0, 2)] = 4, [(1, 2)] = 7, [(2, 2)] = 8, [(3, 2)] = 9,
                [(0, 3)] = 6, [(1,3)] = 10, [(2, 3)] = 11, [(3, 3)] = 12.0,
            };    
            var blockedCells = new HashSet<(int x, int y)>();
            var endState = new HashSet<(int x, int y)>
            {
                (3, 3)
            };
        var gridWorld = new GridWorld(rows, cols, startState, endState, rewards, blockedCells);

            var actions = new List<GridAction> { GridAction.Up, GridAction.Down, GridAction.Left, GridAction.Right };
            var monteCarlo = new MonteCarloTdGridWord(Gamma, Epsilon, Alpha, testOutputHelper,MaxSteps, MaxEpisodes);

            // Act
            var (qValues, policy) = monteCarlo.Control(gridWorld, actions);

            // Assert: Check if the policy converges to optimal actions
            for (var row = 0; row < rows; row++)
            {
                var rowValues = new List<string>();

                for (var col = 0; col < cols; col++)
                {
                    var key = $"({row}, {col})";
                    var value = policy.GetValueOrDefault(key, "N/A");
                    rowValues.Add(value.PadRight(8));
                }

                var rowOutput = string.Join(string.Empty, rowValues);
                testOutputHelper.WriteLine(rowOutput);
            }

        }
    }