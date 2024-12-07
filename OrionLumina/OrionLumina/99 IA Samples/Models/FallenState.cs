namespace BalancingSimulation;

public class FallenState : State
{
    public override string Name => "Fallen";

    public override State Transition(double angle, double angleThreshold)
    {
        // Fallen is terminal
        return this;
    }
}