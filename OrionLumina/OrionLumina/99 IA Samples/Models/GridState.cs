namespace Models.Models;

public class GridState(int x, int y) : IState
{

    public int X { get;  set; } = x;
    public int Y { get;  set; } = y;


    public override string ToString() => $"({X}, {Y})";
}