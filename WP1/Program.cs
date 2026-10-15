using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SequencingLib;

namespace CDOSimulator
{
    public class Program
    {
        static void ComprovarRestriccions(STARRoute ruta, List<double> x, List<double> h)
        {
            Console.WriteLine("Comprovacio de restriccions per a: " + ruta.Name);

            for (int w = 0; w < ruta.Waypoints.Count; w++)
            {
                Waypoint wp = ruta.Waypoints[w];

                // Busquem el segon en què arriba a la distància d'aquell waypoint
                int s = 0;
                while (s < x.Count && x[s] < wp.DistanceToIAF)
                {
                    s++;
                }

                double altFt = h[s];
                bool compleix = wp.IsAltitudeValid(altFt);

                if (compleix)
                {
                    Console.WriteLine("  " + wp.Name + ": " + Math.Round(altFt, 0) + " ft [DESCENS CORRECTE - Dins dels limits]");
                }
                else
                {
                    Console.WriteLine("  " + wp.Name + ": " + Math.Round(altFt, 0) + " ft [DESCENS INCORRECTE - Fora dels limits!]");
                }
            }
        }
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
            Console.WriteLine("========================================================================================================================");
            Console.WriteLine("                         ***SEQUENCING***  SCENARIO 1: RESULTATS PREVIS A L'ORDENACIO");
            Console.WriteLine("========================================================================================================================");

            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine("Avio: " + avions[i].Model.PadRight(12) +
                                  " | STAR: " + stars[i].PadRight(8) +
                                  " | Dist: " + Convert.ToString(Math.Round(distanciesKm[i], 2)).PadRight(6) + " km" +
                                  " | Alt Entrada: " + Math.Round(altitudsEntradaFt[i], 0) + " ft" +
                                  " | Temps: " + Convert.ToString(tempsVolSegons[i]).PadRight(4) + " s" +
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
                    Console.WriteLine(ordre + ". " + avions[i].Model.PadRight(10) + " | " + stars[i].PadRight(6) + " | TOA: " + toasIAF[i] + " | Separació: Primer avió");
                }
                else
                {
                    // Als seguents, restem el temps d'aquest menys el de l'anterior
                    int separacio = tempsVolSegons[i] - tempsVolSegons[i - 1];

                    Console.WriteLine(ordre + ". " + avions[i].Model.PadRight(10) + " | " + stars[i].PadRight(6) + " | TOA: " + toasIAF[i] + " | Separació: " + separacio + " segons");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("========================================================================================================================");
            Console.WriteLine("                         ***SEQUENCING***  SCENARIO 2: SEPARACIÓ DE 2 MINUTS A L'IAF");
            Console.WriteLine("========================================================================================================================");
            Console.WriteLine(" ");

            // Arrays para guardar los resultados del Escenario 2
            TimeSpan[] toasIAF2 = new TimeSpan[6];
            TimeSpan[] horaEntrada2 = new TimeSpan[6];
            int[] delaySegons2 = new int[6];

            TimeSpan horaEntradaSTAR = new TimeSpan(11, 45, 0);

            // 1. El primer avión mantiene su llegada al IAF intacta
            toasIAF2[0] = toasIAF[0];
            horaEntrada2[0] = horaEntradaSTAR;
            delaySegons2[0] = 0;

            // 2. Calculamos los siguientes aviones secuencialmente
            for (int i = 1; i < 6; i++)
            {
                // Cada avión llega exactamente 120 segundos (2 minutos) después del anterior al IAF
                toasIAF2[i] = toasIAF2[i - 1].Add(TimeSpan.FromSeconds(120));

                // Su nueva hora de entrada a la STAR es la hora del IAF menos lo que tarda en volar la ruta
                horaEntrada2[i] = toasIAF2[i].Add(TimeSpan.FromSeconds(-tempsVolSegons[i]));

                // El retraso aplicado es la diferencia entre su nueva hora de entrada y las 11:45:00
                delaySegons2[i] = (int)(horaEntrada2[i] - horaEntradaSTAR).TotalSeconds;
            }

            // 3. Mostramos la tabla requerida por la diapositiva 4
            Console.WriteLine("--- TAULA 1. VOLS ORDENATS PER HORA D'ARRIBADA A L'IAF ---");
            Console.WriteLine("Aircraft         | STAR    | %MLW         | TOA IAF            | Applied delay               | TOA STAR First WP");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");

            for (int i = 0; i < 6; i++)
            {
                string delayTexto = delaySegons2[i] + " s";
                if (delaySegons2[i] > 0)
                {
                    delayTexto = "+" + delaySegons2[i] + " s (retard)";
                }
                else if (delaySegons2[i] < 0)
                {
                    delayTexto = delaySegons2[i] + " s (avançament)";
                }

                Console.WriteLine("Avió: " + avions[i].Model.PadRight(10) +
                                  " | " + stars[i].PadRight(6) +
                                  " | MLW %: " + Convert.ToString(mlwPercents[i]).PadRight(4) + "%" +
                                  " | ARRIBADA: " + toasIAF2[i].ToString(@"hh\:mm\:ss") +
                                  " | DELAY: " + delayTexto.PadRight(20) +
                                  " | ENTRADA STAR: " + horaEntrada2[i]);

            }            
            for (int i = 0; i < 6 - 1; i++) 
            {
                for (int j = 0; j < 6 - i - 1; j++)
                {
                    if (horaEntrada2[j] > horaEntrada2[j + 1])
                    {
                        // Intercanviem l'hora d'entrada a la STAR
                        TimeSpan tempHora = horaEntrada2[j];
                        horaEntrada2[j] = horaEntrada2[j + 1];
                        horaEntrada2[j + 1] = tempHora;

                        // Intercanviem l'hora d'arribada a l'IAF
                        TimeSpan tempTOA = toasIAF2[j];
                        toasIAF2[j] = toasIAF2[j + 1];
                        toasIAF2[j + 1] = tempTOA;

                        // Intercanviem el delay
                        int tempDelay = delaySegons2[j];
                        delaySegons2[j] = delaySegons2[j + 1];
                        delaySegons2[j + 1] = tempDelay;

                        // Intercanviem l'avió
                        Aircraft tempAvio = avions[j];
                        avions[j] = avions[j + 1];
                        avions[j + 1] = tempAvio;

                        // Intercanviem nom de l'STAR
                        string tempStar = stars[j];
                        stars[j] = stars[j + 1];
                        stars[j + 1] = tempStar;

                        // Intercanviem el % MLW
                        double tempMLW = mlwPercents[j];
                        mlwPercents[j] = mlwPercents[j + 1];
                        mlwPercents[j + 1] = tempMLW;

                        // Intercanviem temps de vol
                        int tempTemps = tempsVolSegons[j];
                        tempsVolSegons[j] = tempsVolSegons[j + 1];
                        tempsVolSegons[j + 1] = tempTemps;
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("--- TAULA 2. VOLS REORDENATS PER HORA D'ENTRADA A LA STAR ---");
            Console.WriteLine("Aircraft      | STAR    | TOA FIRST WP STAR      | TOA IAF           | Applied delay         | SEPARATION at IAF");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            for (int i = 0; i < 6; i++)
            {
                int orden = i + 1;
                Console.WriteLine(orden + ". " + avions[i].Model.PadRight(10) + " | " + stars[i].PadRight(6) + " | Entrada STAR: " + horaEntrada2[i] + " | TOA IAF: " + toasIAF2[i].ToString(@"hh\:mm\:ss") + " | Applied DELAY: " + delaySegons2[i].ToString().PadRight(4) + " s" + " | SEPARATION: 120s");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("========================================================================================================================");
            Console.WriteLine("                               COMPROVACIO DE RESTRICCIONS D'ALTITUD ALS WAYPOINTS");
            Console.WriteLine("========================================================================================================================");

            for (int i = 0; i < 6; i++)
            {
                STARRoute ruta = Waypoint.GetLEBLStar(stars[i]);
                Trajectory traj = sim.GetCDO(avions[i], mlwPercents[i]);

                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Vol " + (i + 1) + ": " + avions[i].Model + " | STAR: " + ruta.Name + " (" + mlwPercents[i] + "% MLW)");
                Console.WriteLine("Waypoint | Dist. IAF (NM) | Altitud CDO (ft) | Finestra permesa (ft) | Estat");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");

                bool totsValids = true;

                for (int w = 0; w < ruta.Waypoints.Count; w++)
                {
                    Waypoint wp = ruta.Waypoints[w];

                    int s = 0;
                    while (s < traj.X.Count - 1 && traj.X[s] < wp.DistanceToIAF)
                    {
                        s++;
                    }

                    double altAvioFt = traj.H[s];
                    bool esValid = wp.IsAltitudeValid(altAvioFt);

                    if (!esValid)
                    {
                        totsValids = false;
                    }

                    string estat = esValid ? "[OK]" : "[FORA DE LIMITS]";
                    double distNM = Math.Round(wp.DistanceToIAF / 1852.0, 1);
                    string finestra = (wp.MinAlt == 0.0 ? "No min" : wp.MinAlt + " ft") + " - " + (wp.MaxAlt >= 40000.0 ? "No max" : wp.MaxAlt + " ft");

                    Console.WriteLine(wp.Name.PadRight(8) + " | " + distNM.ToString().PadRight(14) + " | " + Math.Round(altAvioFt, 0).ToString().PadRight(16) + " | " + finestra.PadRight(21) + " | " + estat);
                }

                if (totsValids)
                    Console.WriteLine(">> RESULTAT: Totes les restriccions d'aquesta STAR es compleixen.");
                else
                    Console.WriteLine(">> RESULTAT: ATENCIO, hi ha restriccions que NO es compleixen en aquest perfil CDO!");
            }

            Console.ReadLine();

        }

    }
}