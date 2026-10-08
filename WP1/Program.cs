using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SequencingLib;

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

            
            // SETMANA 3 -----------***SEQUENCING***----------------

            // 1. Definim els 6 vols
            Aircraft[] avions = new Aircraft[6];
            avions[0] = b767;
            avions[1] = b737;
            avions[2] = b777;
            avions[3] = b767;
            avions[4] = a319;
            avions[5] = a320;

            string[] stars = new string[6] { "ALBER1Z", "PUMAL1Z", "MARTA3Z", "MATEX3Z", "LOBAR2W", "CASPE2W" };
            double[] mlwPercents = new double[6] { 80, 100, 100, 80, 80, 100 };
            double[] distanciesNM = new double[6] { 69.7, 51.0, 96.3, 102.6, 81.7, 88.6 };

            // Vectors per guardar els resultats calculats
            double[] distanciesKm = new double[6];
            double[] altitudsEntradaFt = new double[6];
            int[] tempsVolSegons = new int[6];
            TimeSpan[] toasIAF = new TimeSpan[6];

            TimeSpan horaEntrada = new TimeSpan(11, 45, 0);

            // 2. Càlcul per a cada vol
            for (int i = 0; i < 6; i++)
            {
                // Calculem el perfil CDO de l'avió
                Trajectory traj = sim.GetCDO(avions[i], mlwPercents[i]);

                double distMetres = distanciesNM[i] * 1852.0;
                distanciesKm[i] = distMetres / 1000.0;
                
                // Ens quedem amb el segon de després
                int s = 0;
                while (s < traj.X.Count && traj.X[s] < distMetres)
                    s++;
                
                tempsVolSegons[i] = s;
                altitudsEntradaFt[i] = traj.H[s]; // Directament en peus (ft)
                toasIAF[i] = horaEntrada.Add(TimeSpan.FromSeconds(s));
            }

            // 3. Mostrem les dades per pantalla
            Console.WriteLine("==========================================================================================");
            Console.WriteLine("***SEQUENCING***  ESCENARI 1: RESULTATS PREVIS A L'ORDENACIO");
            Console.WriteLine("==========================================================================================");

            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine("Avio: " + avions[i].Model.PadRight(12) +
                                  " | STAR: " + stars[i].PadRight(9) +
                                  " | Dist: " + Math.Round(distanciesKm[i], 2) + " km" +
                                  " | Alt Entrada: " + Math.Round(altitudsEntradaFt[i], 0) + " ft" +
                                  " | Temps: " + tempsVolSegons[i] + " s" +
                                  " | TOA IAF: " + toasIAF[i].ToString(@"hh\:mm\:ss"));
            }
            // 4. Ordenació clàssica per hora d'arribada a l'IAF (de primer a últim)
            for (int i = 0; i < 6 - 1; i++)
            {
                for (int j = 0; j < 6 - i - 1; j++)
                {
                    if (toasIAF[j] > toasIAF[j + 1])
                    {
                        // Intercanvi de TOA
                        TimeSpan tempToa = toasIAF[j];
                        toasIAF[j] = toasIAF[j + 1];
                        toasIAF[j + 1] = tempToa;

                        // Intercanvi de nom de STAR
                        string tempStar = stars[j];
                        stars[j] = stars[j + 1];
                        stars[j + 1] = tempStar;

                        // Intercanvi d'avió
                        Aircraft tempAvio = avions[j];
                        avions[j] = avions[j + 1];
                        avions[j + 1] = tempAvio;

                        // Intercanvi de percentatge MLW
                        double tempMlw = mlwPercents[j];
                        mlwPercents[j] = mlwPercents[j + 1];
                        mlwPercents[j + 1] = tempMlw;

                        // Intercanvi de distància
                        double tempDist = distanciesKm[j];
                        distanciesKm[j] = distanciesKm[j + 1];
                        distanciesKm[j + 1] = tempDist;

                        // Intercanvi d'altitud d'entrada
                        double tempAlt = altitudsEntradaFt[j];
                        altitudsEntradaFt[j] = altitudsEntradaFt[j + 1];
                        altitudsEntradaFt[j + 1] = tempAlt;

                        // Intercanvi de temps de vol
                        int tempTemps = tempsVolSegons[j];
                        tempsVolSegons[j] = tempsVolSegons[j + 1];
                        tempsVolSegons[j + 1] = tempTemps;
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("--- ESCENARI 1: TAULA ORDENADA ---");

            for (int i = 0; i < 6; i++)
            {
                int ordre = i + 1; // 1r, 2n, 3r, etc.

                if (i == 0)
                {
                    // El primer avió no té ningú al davant
                    Console.WriteLine("Ordre " + ordre + ": " + avions[i].Model + " (" + stars[i] + ") - TOA: " + toasIAF[i] + " - Separacio: Primer avio");
                }
                else
                {
                    // Als seguents, restem el temps d'aquest menys el de l'anterior
                    int separacio = tempsVolSegons[i] - tempsVolSegons[i - 1];

                    Console.WriteLine("Ordre " + ordre + ": " + avions[i].Model + " (" + stars[i] + ") - TOA: " + toasIAF[i] + " - Separacio: " + separacio + " segons");
                }
            }
        }

    }
}