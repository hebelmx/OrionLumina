using System;

namespace Models.Models;

public class RandomStart : IState
{

    public int X { get;  set; } 
    public int Y { get;  set; }


    public RandomStart(IState sizeState)

    {
        Random random = new Random(); 
        X = random.Next(sizeState.X);
        Y = random.Next(sizeState.Y);

    }

    public override string ToString() => $"({X}, {Y})";

    // Explicit conversion to GridState
    public static explicit operator GridState(RandomStart randomStart)
    {
        return new GridState(randomStart.X, randomStart.Y);
    }
}