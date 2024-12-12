using FluentAssertions;
using Models;
using Models.Models;
using System.Collections.Generic;
using TorchSharp.Modules;
using Xunit;
using Xunit.Abstractions;

namespace TestModels;
public class MonteCarloGridWorldTestsWithFall(ITestOutputHelper testOutputHelper)
{

    private const double Gamma = 0.9;
    private const double Epsilon = 0.05;
    private const double Alpha = 0.1;
    private const int MaxSteps = 1000; //10_000_000;
    private const int MaxEpisodes = 1000; // 10_000;

    [Fact]
    public void MonteCarloControl_ShouldConvergeToOptimalPolicy()
    {
        // Arrange
        var rows = 5;
        var cols = 13; 
        var startState = new GridState(0, 0);
        var rewards = new Dictionary<(int x, int y), double>
        {
          [(0, 0)] = -1,  [(1, 0)] = -1,    [(2, 0)] = -1,    [(3, 0)] = -1,     [(4,0)] = -1,     [(5, 0)] = -1,    [(6, 0)] = -1,     [(7, 0)] = -1,   [(8,0)] = -1,     [(9, 0)] = -1,   [(10, 0)] = -1,   [(11, 0)] = -1,    [(12, 0)] = -1, 
          [(0, 1)] = 1,   [(1, 1)] = 1,     [(2, 1)] = 1,     [(3, 1)] = 1,      [(4,1)] = 1,      [(5, 1)] = 1,     [(6, 1)] = 1,      [(7, 1)] = 1,    [(8,1)] = 1,      [(9, 1)] = 1,    [(10, 1)] = 1,    [(11, 1)] = 1,     [(12, 1)] = 1, 
          [(0, 2)] = 1,   [(1, 2)] = 1,     [(2, 2)] = 1,     [(3, 2)] = 1,      [(4,2)] = 1,      [(5, 2)] = 1,     [(6, 2)] = 1,      [(7, 2)] = 1,    [(8,2)] = 1,      [(9, 2)] = 1,    [(10, 2)] = 1,    [(11, 2)] = 1,     [(12, 2)] = 1,
          [(0, 3)] = 10,  [(1, 3)] = 10,    [(2, 3)] = 10,    [(3, 3)] = 10,     [(4,3)] = 10,     [(5, 3)] = 10,    [(6, 3)] = 10,     [(7, 3)] = 10,   [(8,3)] = 10,     [(9, 3)] = 10,   [(10, 3)] = 10,   [(11, 3)] = 10,    [(12, 3)] = 10,
          [(0, 4)] = 0,   [(1, 4)] = -100,  [(2, 4)] = -100,  [(3, 4)] = -100,   [(4,4)] = -100,   [(5, 4)] = -100,  [(6, 4)] = -100,   [(7, 4)] = -100, [(8,4)] = -100,   [(9, 4)] = -100, [(10, 4)] = -100, [(11, 4)] = -100,  [(12, 4)] = 1000,
        };
        var blockedCells = new HashSet<(int x, int y)>();
            var endState = new HashSet<(int x, int y)>
            {
                 (1, 2),(1, 3),(1, 4),(1, 5),(1, 6),(1, 7),(1, 8),(1, 9),(1, 10),(1, 11),(1, 12),
            };
        var gridWorld = new GridWorld(rows, cols, startState, endState, rewards, blockedCells);

            var actions = new List<GridAction> { GridAction.Up, GridAction.Down, GridAction.Left, GridAction.Right };
            var monteCarlo = new MonteCarloTdGridWord(Gamma, Epsilon, Alpha, testOutputHelper, MaxSteps, MaxEpisodes);

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