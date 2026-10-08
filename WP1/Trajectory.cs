using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOSimulator
{
    public class Trajectory
    {
        public List<Double> X { get; set; }
        public List<Double> H { get; set; }
        
        public Trajectory()
        {
            X = new List<double>();
            H = new List<double>();
        }
        
    }
}
