using System;

namespace BalancingSimulation;

public class UnstableState : State
{
    public override string Name => "Unstable";

    public override State Transition(double angle, double angleThreshold)
    {
        if (Math.Abs(angle) > 2 * angleThreshold)
            return new FallenState();
        if (Math.Abs(angle) <= angleThreshold)
            return new StableState();
        return this;
    }
}