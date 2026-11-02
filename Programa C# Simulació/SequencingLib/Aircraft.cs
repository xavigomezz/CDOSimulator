using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequencingLib
{
    public class Aircraft
    {
        // DECLAREM ATRIBUTS
        public string Model;
        public double MLW; // tonnes
        public double S; // m²
        public double CD0App;
        public double CD2App;
        public double CD0Clean;
        public double CD2Clean;
        public double HpDesc; // ft
        public double CTDescHigh;
        public double CTDescLow;
        public double CTDescApp;
        public double CT1; // N
        public double CT2; // ft
        public double CT3; // 1/ft²
        public double CF1; // kg / min*kN
        public double CF2; // kts

        // CONSTRUCTOR
        public Aircraft(string model, double mlw, double s, double cd0App, double cd2App,
            double cd0Clean, double cd2Clean, double hpDesc, double ctDescHigh,
            double ctDescApp, double ct1, double ct2, double ct3, double cf1, double cf2)
        {
            Model = model;
            MLW = mlw;
            S = s;
            CD0App = cd0App;
            CD2App = cd2App;
            CD0Clean = cd0Clean;
            CD2Clean = cd2Clean;
            HpDesc = hpDesc;
            CTDescHigh = ctDescHigh;
            CTDescApp = ctDescApp;
            CT1 = ct1;
            CT2 = ct2;
            CT3 = ct3;
            CF1 = cf1;
            CF2 = cf2;
        }


    }

}
    

