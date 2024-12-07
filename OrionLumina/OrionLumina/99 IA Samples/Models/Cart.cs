namespace BalancingSimulation
{
    public class Cart(double mass)
    {
        public double Position { get; private set; }
        public double Velocity { get; private set; }

        public void ApplyForce(double force, double dt)
        {
            var acceleration = force / mass;
            Velocity += acceleration * dt;
            Position += Velocity * dt;
        }
    }
}