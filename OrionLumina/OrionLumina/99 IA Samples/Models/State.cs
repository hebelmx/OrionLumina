using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancingSimulation
{
    public abstract class State
    {
        public abstract string Name { get; }
        public abstract State Transition(double angle, double angleThreshold);
    }
}
