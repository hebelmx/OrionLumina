namespace Models.Models;

public class GridAction : IAction
{
    public static readonly GridAction Up = new("Up");
    public static readonly GridAction Down = new("Down");
    public static readonly GridAction Left = new("Left");
    public static readonly GridAction Right = new("Right");

    public string Name { get; }

    private GridAction(string name) => Name = name;

    public override string ToString() => Name;
}