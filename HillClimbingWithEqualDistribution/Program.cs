using System;
using System.Text;
using System.IO;
using System.Collections.Generic;


namespace HillClimbingWithEqualDistribution
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            List<double> bricks = new List<double>();
            ReloadBricks(ref bricks);
            int bestSplit = 0;
            int split = 0;

            List<double> best = GenerateRandomSolution(random, bricks, ref bestSplit);
            double bestScore = Evaluate(best, bricks, bestSplit);

            var attempt = 0;
            var mutations = 0;

            while (true)
            {
                attempt++;

                if (bestScore == 0 && bestSplit != 0 || attempt > 10000)
                {
                    Console.WriteLine("-------------------------------------------------\n------------------RESULTS FOUND------------------\n-------------------------------------------------");
                    DisplayDistributionFinal(best, bestSplit, bestScore);
                    Evaluate(best, bricks, bestSplit);
                    break;
                }

                Console.WriteLine("Best fitness so far " + bestScore + " attempt " + attempt + " Mutations " + mutations);
                DisplayDistribution(best, bestSplit);

                List<double> newSolution = GenerateRandomSolution(random, bricks, ref split);
                double score = Evaluate(newSolution, bricks, split);

                //sets the constants to the new solution if it is deemed "better".
                if (bestScore > score)
                {
                    bestSplit = split;
                    best = newSolution;
                    bestScore = score;
                    mutations++;
                }
            }
        }

        public static void ReloadBricks(ref List<double> Bricks)
        {
            var reader = new StreamReader("bricks.csv");

            while (!reader.EndOfStream)
            {
                double line = Convert.ToDouble(reader.ReadLine());

                Bricks.Add(line);
            }
        }

        public static void DisplayDistribution(List<double> CurrentBricks, int split)
        {
            double van1TotalWeight = 0;
            double van2TotalWeight = 0;
            for (int i = 0; i < split; i++)
                van1TotalWeight += CurrentBricks[i];
            for (int i = split; i < 60; i++)
                van2TotalWeight += CurrentBricks[i];
            Console.WriteLine("Van1 Load: %" + (van1TotalWeight / (van1TotalWeight + van2TotalWeight) * 100) + " Van2 Load: %" + (van2TotalWeight / (van1TotalWeight + van2TotalWeight) * 100));
        }

        public static void DisplayDistributionFinal(List<double> CurrentBricks, int split , double fitness)
        {
            double van1TotalWeight = 0;
            double van2TotalWeight = 0;
            for (int i = 0; i < split; i++)
                van1TotalWeight += CurrentBricks[i];
            for (int i = split; i < 60; i++)
                van2TotalWeight += CurrentBricks[i];
            Console.WriteLine("Van1 Load: %" + (van1TotalWeight / (van1TotalWeight + van2TotalWeight) * 100) + " Van2 Load: %" + (van2TotalWeight / (van1TotalWeight + van2TotalWeight) * 100));
            Console.WriteLine("Our final fitness value is: " + fitness);
            Console.WriteLine("\nVan1 final bricks loaded\n------------------------");
            for (int i = 0; i < split; i++)
                Console.Write(CurrentBricks[i] + ", ");
            Console.WriteLine("\n\nVan2 final bricks loaded\n------------------------");
            for (int i = split; i < 60; i++)
                Console.Write(CurrentBricks[i] + ", ");
        }

        private static List<double> GenerateRandomSolution(Random random, List<double> Bricks, ref int split, int length = 60)
        {
            List<double> tmp = new List<double>();

            split = random.Next(29, 31);

            for (int i = 0; i < length; i++)
            {
                tmp.Add(GetRandomBricks(random, ref Bricks));
            }

            ReloadBricks(ref Bricks);

            return tmp;
        }

        //evaluates the constant "score" (this is our fitness) of the target being half the total weight split evenly
        private static double Evaluate(List<double> solution, List<double> Bricks, int split)
        {
            double target = 0;
            double van1TotalWeight = 0;
            double van2TotalWeight = 0;

            foreach (var d in Bricks)
            {
                target += d;
            }
            target = Math.Round( (target / 2), 6);

            for (int i = 0; i < split; i++)
                van1TotalWeight += solution[i];
            for (int i = split; i < 60; i++)
                van2TotalWeight += solution[i];

            return (Math.Abs(target - Math.Round(van1TotalWeight, 2)) + (Math.Abs(target - Math.Round(van2TotalWeight, 2))) / 2);
        }

        private static double GetRandomBricks(Random random, ref List<double> Bricks)
        {
            int rand = random.Next(0, Bricks.Count - 1);

            double result = Bricks[rand];
            Bricks.RemoveAt(rand);
            return result;
        }
    }
}
