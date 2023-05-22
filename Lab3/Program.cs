using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace Lab3
{
    internal class Program
    {
        public static long methodToAnalysis1(int[] arr)
        {
            long n = arr.Length;
            long k = n;

            for (int i = 0; i < n; i++)
            {
                if (arr[i] / 7 == 0)
                {
                    k -= 2;
                }
                else
                {
                    k += 3;
                }

            }

            if (arr[0] > 0)
            {
                for (int i = 0; i < n * n; i++)
                {
                    if (arr[0] > 0)
                    {
                        k += 3;
                    }
                }
            }

            return k;
        }

        public static long methodToAnalysisParallel1(int[] arr)
        {
            int coreCount = Environment.ProcessorCount;

            int n = arr.Length;
            if (n % coreCount != 0)
            {
                throw new Exception("Array size is not divisable by the core count");
            }

            long nPerCore = n / coreCount;
            long nnPerCore = n * n / coreCount;

            var tasks = new List<Task<long>>();
            for (int coreIdx = 0; coreIdx < coreCount; coreIdx++)
            {
                int localCoreIdx = coreIdx;
                var task = Task<long>.Factory.StartNew(() =>
                {
                    long subk = 0;
                    for (long i = 0; i < nPerCore; i++)
                    { 
                        if (arr[localCoreIdx * nPerCore + i] / 7 == 0)
                        {
                            subk -= 2;
                        }
                        else
                        {
                            subk += 3;
                        }
                    }

                    if (arr[0] > 0)
                    {
                        for (long i = 0; i < nnPerCore; i++)
                        {
                            if (arr[0] > 0)
                            {
                                subk += 3;
                            }
                        }
                    }

                    return subk;
                });
                tasks.Add(task);
            }

            long k = n;
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                k += task.Result;
            }

            return k;
        }

        public static long methodToAnalysis2(int[] arr)
        {
            long k = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                k += arr[i] + FF4(i, arr);
            }

            return k;
        }

        public static long methodToAnalysisParallel2(int[] arr)
        {
            int coreCount = Environment.ProcessorCount;

            int n = arr.Length;
            if (n % coreCount != 0)
            {
                throw new Exception("Array size is not divisable by the core count");
            }

            var tasks = new List<Task<long>>();
            for (int coreIdx = 0; coreIdx < coreCount; coreIdx++)
            {
                int localCoreIdx = coreIdx;
                var task = Task<long>.Factory.StartNew(() =>
                {
                    long subk = 0;
                    for (int i = 0; i < n / coreCount; i++)
                    {
                        int j = localCoreIdx * (n / coreCount) + i;
                        subk += arr[j] + FF4(j, arr);
                    }
                    return subk;
                });
                tasks.Add(task);
            }

            long k = 0;
            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                k += task.Result;
            }

            return k;
        }

        public static long FF4(int n, int[] arr)
        {
            if (n > 0 && arr.Length > n && arr[n] < 0)
            {
                return FF4(n / 2, arr) + FF4(n / 3, arr);
            }

            return n;
        }

        public static int knapsack(int weightLimit, int n, int[] weights, int[] values)
        {
            if (n == 1)
            {
                return weights[0] <= weightLimit ? values[0] : 0;
            }
            if (weights[n - 1] > weightLimit)
            {
                return knapsack(weightLimit, n - 1, weights, values);
            }

            int case1 = knapsack(weightLimit - weights[n - 1], n - 1, weights, values) + values[n - 1];
            int case2 = knapsack(weightLimit, n - 1, weights, values);
            return Math.Max(case1, case2);
        }

        public static int knapsackDP(int weightLimit, int n, int[] weights, int[] values)
        {
            int[,] memo = new int[n + 1, weightLimit +1];
            for (int i = 1; i <= n; i++)
            {
                for (int w = 1; w <= weightLimit; w++)
                {
                    if (weights[i-1] <= w)
                    {
                        memo[i, w] = Math.Max(values[i - 1] + memo[i - 1, w - weights[i - 1]], memo[i - 1, w]);
                    }
                    else
                    {
                        memo[i, w] = memo[i - 1, w];
                    }
                }
            }

            return memo[n, weightLimit];
        }

        public static int[] createTestArray(int n)
        {
            int[] arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                arr[i] = -(i+1);
            }
            return arr;
        }

        public static void benchmarkKnapsack(Func<int, int, int[], int[], int> knapsackFunc, int[] sizes, bool silent = false)
        {
            var stopWatch = new Stopwatch();
            foreach (int size in sizes)
            {
                int weightLimit = size*10;
                int[] weights = createTestArray(size);
                int[] values = createTestArray(size);
                int n = weights.Length;

                if (silent) {
                    knapsackFunc(weightLimit, n, weights, values);
                } else {
                    stopWatch.Reset();
                    stopWatch.Start();
                    knapsackFunc(weightLimit, n, weights, values);
                    stopWatch.Stop();
                    Console.WriteLine("{0}, {1}", size, Math.Round(stopWatch.Elapsed.TotalSeconds * 1000, 5));
                }
            }
        }

        public static void benchmarkSimple(Func<int[], long> func, int[] sizes, bool silent = false)
        {
            var stopWatch = new Stopwatch();
            foreach (int size in sizes)
            {
                int[] arr = createTestArray(size);

                if (silent)
                {
                    func(arr);
                }
                else
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    func(arr);
                    stopWatch.Stop();
                    Console.WriteLine("{0}, {1}", size, Math.Round(stopWatch.Elapsed.TotalSeconds * 1000, 5));
                }
            }
        }

        static void Main(string[] args)
        {
            int cores = Environment.ProcessorCount;
            Console.WriteLine("The number of processors on this computer is {0}", Environment.ProcessorCount);

            /*
            Console.WriteLine();

            int[] knapsackSizes = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28 };
            Console.WriteLine("Knapsack Recursive (ms):");
            benchmarkKnapsack(knapsack, new int[] { 10 }, true);
            benchmarkKnapsack(knapsack, knapsackSizes);
            Console.WriteLine("Knapsack DP (ms):");
            benchmarkKnapsack(knapsackDP, new int[] { 10 }, true);
            benchmarkKnapsack(knapsackDP, knapsackSizes);
            */

            /*
            Console.WriteLine();

            int[] method1Sizes = new int[] { 10, 50, 100, 250, 500, 1000, 2000 }
                .Select(size => size * cores)
                .ToArray();
            Console.WriteLine("methodToAnalysis1 (ms):");
            benchmarkSimple(methodToAnalysis1, new int[] { cores }, true);
            benchmarkSimple(methodToAnalysis1, method1Sizes);
            Console.WriteLine("methodToAnalysisParallel1 (ms):");
            benchmarkSimple(methodToAnalysisParallel1, new int[] { cores }, true);
            benchmarkSimple(methodToAnalysisParallel1, method1Sizes);
            */

            
            Console.WriteLine();

            int[] method2Sizes = new int[] { 10, 50, 100, 500, 1000, 3500 }
                .Select(size => size * cores)
                .ToArray();
            Console.WriteLine("methodToAnalysis2 (ms):");
            benchmarkSimple(methodToAnalysis2, new int[] { cores }, true);
            benchmarkSimple(methodToAnalysis2, method2Sizes);
            Console.WriteLine("methodToAnalysisParallel2 (ms):");
            benchmarkSimple(methodToAnalysisParallel2, new int[] { cores }, true);
            benchmarkSimple(methodToAnalysisParallel2, method2Sizes);
            
        }
    }
}