using System.Collections.Generic;
using System.Linq;

namespace BalancingSimulation
{
    public class MultiPoleSystem
    {
        public Cart Cart { get; }
        public List<Pole> Poles { get; }

        public MultiPoleSystem(double cartMass, List<(double Length, double Mass, double AngleThreshold)> poleSpecifications)
        {
            Cart = new Cart(cartMass);
            Poles = [];

            foreach (var spec in poleSpecifications)
            {
                Poles.Add(new Pole(spec.Length, spec.Mass, spec.AngleThreshold));
            }
        }

        public void Update(double force, double dt)
        {
            Cart.ApplyForce(force, dt);

            foreach (var pole in Poles)
            {
                pole.Update(force, dt); // Torque here approximated by force applied
            }
        }

        public List<string> GetStates()
        {
            return Poles.Select(pole => pole.CurrentState.Name).ToList();
        }
    }
}