using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SequencingLib
{
    public class Waypoint
    {
        public string Name;
        public double DistanceToIAF; // en metres
        public double MinAlt; // en ft (0 si no hi ha restricció)
        public double MaxAlt; // en ft (40000 si no hi ha restricció)

        public Waypoint(string name, double distanceToIAF, double minAlt = 0.0, double maxAlt = 40000.0)
        {
            Name = name;
            DistanceToIAF = distanceToIAF;
            MinAlt = minAlt;
            MaxAlt = maxAlt;
        }
    }
    public class STARRoute
    {
        public string Name;
        public Waypoint EntryPoint;
        public List<Waypoint> Waypoints;
        
        public STARRoute(string name, Waypoint entryPoint)
        {
            Name = name;
            EntryPoint = entryPoint;

        }
    }
}
