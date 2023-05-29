using System.Diagnostics;

namespace Lab4
{
    public class Algorithm
    {
        /*
            Faile places_data.xlsx pateikta informacija apie lankytinas vietas (1 lentelė). Tikslas: kaip galima pigesnio maršruto sudarymas kai,
                
            * priimama, kad kelionės tarp vietų kaina lygi kvadratinei šakniai iš kelionės atstumo;
            * kelionės pradžios ir pabaigos vieta sutampa (su grįžimu atgal);
            * ta pati vieta negali būti aplankyta daugiau nei vieną;
            * Reikia aplankyti bet kurias 150 vietų iš pateikto sąrašo.
        */

        public int pathLength = 150;
        public int batchSize = 40;
        public float mutationPickUniqueChance = 0.005f;
        public float mutationSwapChance = 0.01f;
        public float mutationPickLocalChance = 0.005f;
        public float localAreaRadius = 70000.000f;
        public int seed;
        public int iteration = 0;
        public SortedList<double, List<int>> paths;
        public List<List<int>> localPlaces;

        List<Place> places;
        Random rand;

        public Algorithm(List<Place> places, int seed)
        {
            this.places = places;
            this.seed = seed;
            rand = new Random(seed);
            paths = GenerateInitialPaths(rand, places, pathLength, batchSize);
            localPlaces = ComputeLocalNodes(places, localAreaRadius);
        }

        public Algorithm(List<Place> places) : this(places, (int)DateTime.Now.Ticks) { }

        public void Reset()
        {
            rand = new Random(seed);
            paths = GenerateInitialPaths(rand, places, pathLength, batchSize);
            localPlaces = ComputeLocalNodes(places, localAreaRadius);
            iteration = 0;
        }

        public class Place
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

        public static double GetDistance(float x0, float y0, float x1, float y1)
        {
            float dx = x0 - x1;
            float dy = y0 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistance(Place A, Place B)
        {
            return GetDistance(A.x, A.y, B.x, B.y);
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

        public static List<List<int>> ComputeLocalNodes(List<Place> places, float localRadius)
        {
            var locals = new List<List<int>>(places.Count);
            for (int i = 0; i < places.Count; i++)
            {
                var nodeLocals = new List<int>();
                for (int j = 0; j < places.Count; j++)
                {
                    if (i == j) continue;

                    if (GetDistance(places[i], places[j]) < localRadius)
                    {
                        nodeLocals.Add(j);
                    }
                }
                locals.Add(nodeLocals);
            }
            return locals;
        }

        public static int PickUniqueLocalNode(Random rand, List<int> path, List<int> localPlaces)
        {
            if (localPlaces.Count == 0) return -1;

            int retryCount = 5;
            for (int i = 0; i < retryCount; i++)
            {
                int idx = rand.Next(0, localPlaces.Count);
                int place = localPlaces[idx];
                if (!path.Contains(place))
                {
                    return place;
                }
            }
            return -1;
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
            for (int i = 0; i < path.Count - 1; i++)
            {
                int from = path[i];
                int to = path[i + 1];
                cost += GetTravalCost(places[from], places[to]);
            }
            cost += GetTravalCost(places[path.First()], places[path.Last()]);
            return cost;
        }

        public static List<int> SplicePaths(Random rand, List<int> A, List<int> B)
        {
            Debug.Assert(A.Count == B.Count);

            List<int> spliced = new List<int>(A.Count);
            for (int i = 0; i < A.Count - 1; i++)
            {
                spliced[i] = rand.Next(0, 2) == 0 ? A[i] : B[i];
            }
            return spliced;
        }

        public static SortedList<double, List<int>> GenerateInitialPaths(Random rand, List<Place> places, int pathLength, int pathCount)
        {
            var paths = new SortedList<double, List<int>>();
            for (int i = 0; i < pathCount; i++)
            {
                var path = GenerateRandomPath(rand, pathLength, places.Count);
                var cost = GetPathCost(path, places);
                paths.Add(cost, path);
            }
            return paths;
        }


        public void SetLocalRadius(float radius)
        {
            localAreaRadius = radius;
            localPlaces = ComputeLocalNodes(places, localAreaRadius);
        }

        public void MutatePath(List<int> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (rand.NextSingle() < mutationPickUniqueChance)
                {
                    path[i] = PickUnqiueNode(rand, path, places.Count);
                }
                if (rand.NextSingle() < mutationSwapChance)
                {
                    int idx1 = rand.Next(0, path.Count);
                    int idx2 = rand.Next(0, path.Count);

                    (path[idx2], path[idx1]) = (path[idx1], path[idx2]);
                }
                if (rand.NextSingle() < mutationPickLocalChance)
                {
                    int localNode = PickUniqueLocalNode(rand, path, localPlaces[i]);
                    if (localNode != -1)
                    {
                        path[i] = localNode;
                    }
                }
            }
        }

        public void IterateSolution()
        {
            double bestScore = paths.First().Key;
            double worstScore = paths.Last().Key;

            var removedPaths = new List<double>();
            var parentPool = new List<List<int>>();
            foreach (var entry in paths)
            {
                double probability = (entry.Key - bestScore) / (worstScore - bestScore);
                if (rand.NextSingle() >= probability)
                {
                    parentPool.Add(entry.Value);
                }
                else
                {
                    removedPaths.Add(entry.Key);
                }
            }

            foreach (var key in removedPaths)
            {
                paths.Remove(key);
            }

            while (batchSize > paths.Count)
            {
                var parentIdx = rand.Next(0, parentPool.Count);
                var parent = parentPool[parentIdx];
                var child = new List<int>(parent);
                MutatePath(child);
                var cost = GetPathCost(child, places);
                if (!paths.ContainsKey(cost))
                {
                    paths.Add(cost, child);
                }
            }

            iteration++;
        }

        public void IterateSolutionFor(float seconds)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < seconds * 1000)
            {
                IterateSolution();
            }
            stopwatch.Stop();
        }

        public Tuple<double, List<int>> GetBestPath()
        {
            var entry = paths.First();
            return Tuple.Create(entry.Key, entry.Value);
        }
    }
}
