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
            Waypoints = new List<Waypoint>();
            Waypoints.Add(entryPoint);
        }
        public void AddWaypoint(Waypoint wp)
        { Waypoints.Add(wp);}
        public double GetTotalDistance()
        { return EntryPoint.DistanceToIAF; }
    }
    public class FlightArrival
    {
        public string AircraftModel;
        public string STARName;
        public double MLWPercent;
        public double DistanceToIAF;     // en km
        public double EntryAltitude;      // en ft
        public double TimeToIAF;          // en s
        public TimeSpan TOA_IAF;          // hora calculada d'arribada a l'IAF
        public int SequenceOrder;         // lloc en la cua (1r, 2n, etc.)
        public double SeparationToPrev;   // separació amb l'anterior en segons

        public FlightArrival(string aircraftModel, string starName, double mlwPercent, double distanceToIAF)
        {
            AircraftModel = aircraftModel;
            STARName = starName;
            MLWPercent = mlwPercent;
            DistanceToIAF = distanceToIAF;
        }
    }
}
