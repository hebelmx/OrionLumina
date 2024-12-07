using System;

namespace BalancingSimulation;

public class StableState : State
{
    public override string Name => "Stable";

    public override State Transition(double angle, double angleThreshold)
    {
        if (Math.Abs(angle) > angleThreshold)
            return new UnstableState();
        return this;
    }
}