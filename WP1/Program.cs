using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOSimulator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Simulator sim = new Simulator();

            // Creem cada avió
            Aircraft b767 = new Aircraft("B767-300ER", 0.145150e+03, 0.28350e+03, 0.14000e-01, 0.49000e-01, 0.17400e-01, 0.45900e-01, 26418.0, 0.64359e-01, 0.12475, 0.35167e+06, 0.44673e+05, 0.10129e-09, 0.54005e+00, 0.55782e+03);
            Aircraft b777 = new Aircraft("B777-300", 0.237680e+03, 0.42804e+03, 0.17300e-01, 0.48400e-01, 0.15700e-01, 0.42000e-01, 36122.0, 0.44239e-01, 0.092921, 0.42577e+06, 0.48987e+05, 0.66146e-10, 0.87843e+00, 0.36897e+04);
            Aircraft b737 = new Aircraft("B737", 0.51710e+02, 0.12465e+03, 0.27000e-01, 0.44100e-01, 0.23500e-01, 0.44500e-01, 30152.0, 0.36336e-01, 0.16440, 0.14573e+06, 0.55638e+05, 0.14200e-10, 0.94680e+00, 0.10000e+15);
            Aircraft a320 = new Aircraft("A320-212", 0.64500e+02, 0.12260e+03, 0.24200e-01, 0.46900e-01, 0.24000e-01, 0.37500e-01, 12398.0, 0.45711e-01, 0.13981, 0.13605e+06, 0.52238e+05, 0.26637e-10, 0.94000e+00, 0.10000e+06);
            Aircraft a319 = new Aircraft("A319-131", 0.61000e+02, 0.12260e+03, 0.28400e-01, 0.37600e-01, 0.28000e-01, 0.31000e-01, 27726.0, 0.83084e-01, 0.14767, 0.13900e+06, 0.58900e+05, 0.57200e-14, 0.68800e+00, 0.16700e+04);

            // Els fiquem en una llista per crear un bucle
            Aircraft[] flota = new Aircraft[5];

            flota[0] = b767;
            flota[1] = b777;
            flota[2] = b737;
            flota[3] = a320;
            flota[4] = a319;
            
            Console.WriteLine("==================================================");
            Console.WriteLine("    COMPROVACIÓ DE DESCENSOS (80% i 100% MLW)    ");
            Console.WriteLine("==================================================");

            for (int i = 0; i < flota.Length; i++)
            {
                Aircraft avion = flota[i];

                // Cas al 80% MLW
                Trajectory traj80 = sim.GetCDO(avion, 80);
                int puntos80 = traj80.X.Count;
                double distKm80 = traj80.X[puntos80 - 1] / 1000.0;
                double altFt80 = traj80.H[puntos80 - 1];

                Console.WriteLine("Avió: " + avion.Model + " | 80% MLW");
                Console.WriteLine("  Segons: " + puntos80 + " | Distància: " + Math.Round(distKm80, 2) + " km | Altitud: " + Math.Round(altFt80, 0) + " ft");

                // Cas al 100% MLW
                Trajectory traj100 = sim.GetCDO(avion, 100);
                int puntos100 = traj100.X.Count;
                double distKm100 = traj100.X[puntos100 - 1] / 1000.0;
                double altFt100 = traj100.H[puntos100 - 1];

                Console.WriteLine("Avió: " + avion.Model + " | 100% MLW");
                Console.WriteLine("  Segons: " + puntos100 + " | Distància: " + Math.Round(distKm100, 2) + " km | Altitud: " + Math.Round(altFt100, 0) + " ft");

                Console.WriteLine("--------------------------------------------------");
            }

            Console.WriteLine("\nProves finalitzades. Prem Enter per tancar.");
            Console.ReadLine();
        }

    }
}