using System;

namespace BalancingSimulation
{
    public class Pole(double length, double mass, double angleThreshold)
    {
        public double Angle { get; private set; }
        public double AngularVelocity { get; private set; }
        public State CurrentState { get; private set; } = new StableState();

        public void Update(double torque, double dt)
        {
            // Simplified physics update
            var gravity = 9.8;
            var angularAcceleration = (torque - gravity * mass * length * Math.Sin(Angle)) / (mass * length);
            AngularVelocity += angularAcceleration * dt;
            Angle += AngularVelocity * dt;

            // Update state
            CurrentState = CurrentState.Transition(Angle, angleThreshold);
        }
    }
}