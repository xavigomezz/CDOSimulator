using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SequencingLib;

namespace CDOSimulator
{
    public class Simulator
    {
        // 1. CALCULEM LA DENSITAT DE L'AIRE
        public double CalculateDensityISA(double altitudeMeters)
        {
            double T0 = 288.15;
            double P0 = 101325;
            double L = 0.0065;
            double R = 287.05;
            double g0 = 9.80665;

            double T = T0 - L * altitudeMeters;
            double p = P0 * Math.Pow(T / T0, g0 / (R * L));
            double rho = p / (R * T);
            return rho;
        }

        // 2. CALCULEM LA VELOCITAT QUE MINIMITZA EL DESCENS
        public double CalculateMinRoDSpeed(double thrust, double CD0, double CD2, double rho, double S, double mass)
        {
            double g = 9.80665;
            double A = (rho * S * CD0) / (2 * mass * g);
            double B = (2 * CD2 * mass * g) / (rho * S);
            double T = thrust / (mass * g);
            double speed = Math.Sqrt((T + Math.Sqrt(T * T + 12 * A * B)) / (6 * A));
            return speed;
        }

        // 3. OBTENIM ELS CD0 i CD2
        public double GetCD0(Aircraft aircraft, double h)
        {
            if (h < aircraft.HpDesc)
                return aircraft.CD0App;
            else
                return aircraft.CD0Clean;
        }
        public double GetCD2(Aircraft aircraft, double h)
        {
            if (h < aircraft.HpDesc)
                return aircraft.CD2App;
            else
                return aircraft.CD2Clean;
        }
        
        // 4. THRUST DEL MOTOR
        public double CalculateThrust(Aircraft aircraft, double h)
        {
            double ctFactor;

            if (h < aircraft.HpDesc)
                ctFactor = aircraft.CTDescApp;
            else
                ctFactor = aircraft.CTDescHigh;

            double maxThrust = aircraft.CT1 * (1 - (h / aircraft.CT2) + aircraft.CT3*h*h);
            return ctFactor * maxThrust;
        }

        // 5. VELOCITAT DE DESCENS
        public double CalculateRoD(double speed, double drag, double thrust, double mass)
        {
            double g = 9.80665; // ISA.doc
            double RoD = ((drag - thrust) * speed) / (mass * g);
            return RoD;
        }

        // 6. COMBUSTIBLE CREMAT PER SEGON
        public double CalculateFuelBurn(Aircraft aircraft, double thrust, double speed, double deltaT)
        {
            double fuelFlow = aircraft.CF1 * (1 - (speed / aircraft.CF2)); // Els CF estan en minuts llavors està en kg/min
            double fuelFlowKgPerSec = fuelFlow / 60.0;

            double fuelBurned = fuelFlowKgPerSec * Math.Abs(deltaT); // Assegurem que surti una quantitat de fuel cremat positiva
            return fuelBurned;
        }


        // LA SIMULACIÓ PRINCIPAL
        public Trajectory GetCDO(Aircraft aircraft, double mlwPercent)
        {
            Trajectory trajectory = new Trajectory();

            double x = 0.0;        // metres
            double h = 6000.0;     // feet
            double deltaT = -1.0;  // segons

            // Massa final a l'arribada (IAF)
            double mass = aircraft.MLW * (mlwPercent / 100.0) * 1000.0;

            while (h < 40000.0)
            {
                trajectory.X.Add(x);
                trajectory.H.Add(h);

                double hMeters = h * 0.3048; // Per passar de ft a m
                double rho = CalculateDensityISA(hMeters);

                // Obtenir CD0 i CD2
                double CD0 = GetCD0(aircraft, h);
                double CD2 = GetCD2(aircraft, h);

                // Thrust
                double thrust = CalculateThrust(aircraft, h);

                // Velocitat de Descens
                double speed = CalculateMinRoDSpeed(thrust, CD0, CD2, rho, aircraft.S, mass);

                // CL
                double g = 9.80665;
                double CL = (2.0 * mass * g) / (rho * speed * speed * aircraft.S);

                // Drag i CD (total)
                double CD = CD0 + CD2 * CL * CL;
                double drag = 0.5 * CD * rho * speed * speed * aircraft.S;

                // Taxa de Descens
                double rateOfDescent = CalculateRoD(speed, drag, thrust, mass);

                // MOVIMENT DE L'AERONAU
                double horizontalSpeed = Math.Sqrt(Math.Max(0.0, speed * speed - rateOfDescent * rateOfDescent)); // Pitàgores
                x = x - horizontalSpeed * deltaT; // MRU
                hMeters = hMeters - rateOfDescent * deltaT;
                h = hMeters / 0.3048;

                //Recuperar el combustible que tenia abans de començar el descens
                double fuelBurned = CalculateFuelBurn(aircraft, thrust, speed, deltaT);
                mass -= fuelBurned;
            }

            return trajectory;
        }
    }
}