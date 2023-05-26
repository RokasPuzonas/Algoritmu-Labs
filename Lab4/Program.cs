using System.Diagnostics;
using System.IO;

namespace Lab4
{
    class Place
    {
        public string name;
        public float x;
        public float y;
        public Place(string name, float x, float y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }
    }

    internal class Program
    {
        public static double GetDistance(Place A, Place B)
        {
            float dx = A.x - B.x;
            float dy = A.y - B.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetTravalCost(Place A, Place B)
        {
            return Math.Sqrt(GetDistance(A, B));
        }

        public static List<Place> ReadPlacesFromTSV(string filename)
        {
            List<Place> cities = new List<Place>();
            foreach (var line in File.ReadLines(filename))
            {
                string[] parts = line.Split("\t");

                string name = parts[0];
                float x = float.Parse(parts[2]);
                float y = float.Parse(parts[3]);
                cities.Add(new Place(name, x, y));
            }
            return cities;
        }

        // Warning 'PickUnqiueNode' could get stuck in an infinite loop, if there are not enough places
        public static int PickUnqiueNode(Random rand, List<int> path, int placesCount)
        {
            while (true)
            {
                int num = rand.Next(0, placesCount);
                if (!path.Contains(num))
                {
                    return num;
                }
            }
        }

        public static List<int> GenerateRandomPath(Random rand, int size, int placesCount)
        {
            List<int> path = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                path.Add(PickUnqiueNode(rand, path, placesCount));
            }
            return path;
        }

        public static double GetPathCost(List<int> path, List<Place> places)
        {
            double cost = 0;
            for (int i = 0; i < path.Count-1; i++)
            {
                int from = path[i];
                int to = path[i+1];
                cost += GetTravalCost(places[from], places[to]);
            }
            cost += GetTravalCost(places[path.First()], places[path.Last()]);
            return cost;
        }

        public static List<int> SplicePaths(Random rand, List<int> A, List<int> B)
        {
            Debug.Assert(A.Count == B.Count);

            List<int> spliced = new List<int>(A.Count);
            for (int i = 0; i < A.Count-1; i++)
            {
                spliced[i] = rand.Next(0, 2) == 0 ? A[i] : B[i];
            }
            return spliced;
        }

        public static void MutatePath(Random rand, List<int> path, float mutationPickUniqueChance, float mutationSwapChance, int placesCount)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (rand.NextSingle() < mutationPickUniqueChance) {
                    path[i] = PickUnqiueNode(rand, path, placesCount);
                }
                if (rand.NextSingle() < mutationSwapChance) {
                    int idx1 = rand.Next(0, path.Count);
                    int idx2 = rand.Next(0, path.Count);

                    (path[idx2], path[idx1]) = (path[idx1], path[idx2]);
                }
            }
        }

        static void Main(string[] args)
        {
            /*
                Faile places_data.xlsx pateikta informacija apie lankytinas vietas (1 lentelė). Tikslas: kaip galima pigesnio maršruto sudarymas kai,
                
                * priimama, kad kelionės tarp vietų kaina lygi kvadratinei šakniai iš kelionės atstumo;
                * kelionės pradžios ir pabaigos vieta sutampa (su grįžimu atgal);
                * ta pati vieta negali būti aplankyta daugiau nei vieną;
                * Reikia aplankyti bet kurias 150 vietų iš pateikto sąrašo.
            */

            int pathLength = 150;
            int batchSize = 50;
            int timeLimitMs = 60 * 1000;
            float mutationPickUniqueChance = 0.05f;
            float mutationSwapChance = 0.05f;

            int seed = (int)DateTime.Now.Ticks;
            Random rand = new Random(seed);

            var places = ReadPlacesFromTSV("places.tsv");
            
            var bestPaths = new SortedList<double, List<int>>();
            for (int i = 0; i < batchSize; i++)
            {
                var path = GenerateRandomPath(rand, pathLength, places.Count);
                var cost = GetPathCost(path, places);
                bestPaths.Add(cost, path);
            }

            var stopwatch = new Stopwatch();
            int iteration = 0;
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeLimitMs)
            {
                double bestScore = bestPaths.First().Key;
                double worstScore = bestPaths.Last().Key;

                if (iteration % 200 == 0)
                {
                    //Console.WriteLine("At {0}, best '{1}', worst '{2}'", iteration, bestScore, worstScore);
                }

                var removedPaths = new List<double>();
                var parentPool = new List<List<int>>();
                foreach (var entry in bestPaths)
                {
                    double probability = (entry.Key - bestScore) / (worstScore - bestScore);
                    if (rand.NextSingle() >= probability) {
                        parentPool.Add(entry.Value);
                    } else {
                        removedPaths.Add(entry.Key);
                    }
                }

                foreach (var key in removedPaths)
                {
                    bestPaths.Remove(key);
                }

                while (batchSize > bestPaths.Count)
                {
                    var parentIdx = rand.Next(0, parentPool.Count);
                    var parent = parentPool[parentIdx];
                    var child = new List<int>(parent);
                    MutatePath(rand, child, mutationPickUniqueChance, mutationSwapChance, places.Count);
                    var cost = GetPathCost(child, places);
                    if (!bestPaths.ContainsKey(cost)) {
                        bestPaths.Add(cost, child);
                    }
                }
                
                iteration++;
            }
            stopwatch.Stop();

            Console.WriteLine("Best after {0} iterations is {1}", iteration, bestPaths.First().Key);
            Console.WriteLine("Time taken: {0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Seed: {0}", seed);

            // Best after 484012 iterations is 30826.444176719036
            // Time taken: 60000ms
            // Seed: -1477536140
        }
    }
}