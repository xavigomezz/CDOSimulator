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
        public double MinAlt;        // en ft (0 si no hi ha restricció)
        public double MaxAlt;        // en ft (40000 si no hi ha restricció)

        public Waypoint(string name, double distanceToIAF_Metres, double minAlt = 0.0, double maxAlt = 40000.0)
        {
            Name = name;
            DistanceToIAF = distanceToIAF_Metres;
            MinAlt = minAlt;
            MaxAlt = maxAlt;
        }
        public bool IsAltitudeValid(double altitudFt)
        {
            return (altitudFt >= MinAlt && altitudFt <= MaxAlt);
        }
        const double NMtoM = 1852.0;
        public static List<STARRoute> GetLEBLStars()
        {
            List<STARRoute> stars = new List<STARRoute>();

            STARRoute alber = new STARRoute("ALBER1Z", new Waypoint("ALBER", 69.7 * NMtoM, 11000, 25000));
            alber.AddWaypoint(new Waypoint("CUTXE", 50.2 * NMtoM));
            alber.AddWaypoint(new Waypoint("UTHAN", 27.8 * NMtoM));
            alber.AddWaypoint(new Waypoint("ENJUC", 17.3 * NMtoM, minAlt: 8000));
            alber.AddWaypoint(new Waypoint("UCREQ", 10.4 * NMtoM, maxAlt: 10000));
            alber.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(alber);

            STARRoute pumal = new STARRoute("PUMAL1Z", new Waypoint("PUMAL", 51.0 * NMtoM, 13000, 25000));
            pumal.AddWaypoint(new Waypoint("BERGA", 39.2 * NMtoM, minAlt: 12000));
            pumal.AddWaypoint(new Waypoint("KOSIT", 25.0 * NMtoM));
            pumal.AddWaypoint(new Waypoint("MAMUK", 19.1 * NMtoM, minAlt: 8000));
            pumal.AddWaypoint(new Waypoint("UCREQ", 10.4 * NMtoM, maxAlt: 10000));
            pumal.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(pumal);

            STARRoute marta = new STARRoute("MARTA3Z", new Waypoint("MARTA", 96.3 * NMtoM, maxAlt: 24000));
            marta.AddWaypoint(new Waypoint("EBROX", 75.0 * NMtoM, minAlt: 10000));
            marta.AddWaypoint(new Waypoint("RES", 48.7 * NMtoM));
            marta.AddWaypoint(new Waypoint("VLA", 27.5 * NMtoM));
            marta.AddWaypoint(new Waypoint("BL463", 10.0 * NMtoM, maxAlt: 10000));
            marta.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(marta);

            STARRoute matex = new STARRoute("MATEX3Z", new Waypoint("MATEX", 102.6 * NMtoM, maxAlt: 28000));
            matex.AddWaypoint(new Waypoint("SENIA", 74.0 * NMtoM, minAlt: 10000));
            matex.AddWaypoint(new Waypoint("RES", 48.7 * NMtoM));
            matex.AddWaypoint(new Waypoint("VLA", 27.5 * NMtoM));
            matex.AddWaypoint(new Waypoint("BL463", 10.0 * NMtoM, maxAlt: 10000));
            matex.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(matex);

            STARRoute lobar = new STARRoute("LOBAR2W", new Waypoint("LOBAR", 81.7 * NMtoM, maxAlt: 28000));
            lobar.AddWaypoint(new Waypoint("PEKIS", 45.0 * NMtoM, 16000, 20000));
            lobar.AddWaypoint(new Waypoint("BL461", 10.0 * NMtoM, maxAlt: 10000));
            lobar.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(lobar);

            STARRoute caspe = new STARRoute("CASPE2W", new Waypoint("CASPE", 88.6 * NMtoM, maxAlt: 28000));
            caspe.AddWaypoint(new Waypoint("MECUH", 47.6 * NMtoM, 16000, 20000));
            caspe.AddWaypoint(new Waypoint("VIBOK", 27.5 * NMtoM, maxAlt: 15000));
            caspe.AddWaypoint(new Waypoint("BL461", 10.0 * NMtoM, maxAlt: 10000));
            caspe.AddWaypoint(new Waypoint("SLL", 0.0, minAlt: 6000));
            stars.Add(caspe);

            return stars;
        }

        public static STARRoute GetLEBLStar(string name)
        {
            foreach (STARRoute star in GetLEBLStars())
                if (star.Name == name)
                    return star;
            return null;
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
