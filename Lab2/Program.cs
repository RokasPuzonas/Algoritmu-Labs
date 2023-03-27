using System;
using System.Reflection;

namespace Lab2
{
    internal class Program
    {
        public static uint counter = 0;

        public static long methodToAnalysis1(int[] arr)
        {
            long n = arr.Length;
            long k = n;
            for (int i = 0; i < n; i++)
            {
                counter += 1;
                if (arr[i] > 0)
                {
                    for (int j = 0; j < n * n / 2; j++)
                    {
                        k -= 2;
                        counter += 1;
                    }
                    for (int j = 0; j < n * n / 2; j++)
                    {
                        k += 3;
                        counter += 1;
                    }
                }
            }
            return k;
        }

        public static long methodToAnalysis2(int n, int[] arr)
        {
            long k = 0;
            for (int i = 0; i < n; i++)
            {
                counter += 1;
                k += arr[i] + FF4(i, arr);
            }

            return k;
        }
        public static long methodToAnalysis2(int[] arr)
        {
            return methodToAnalysis2(arr.Length, arr);
        }

        public static long FF4(int n, int[] arr)
        {
            if (n > 0 && arr.Length > n && arr[n] < 0)
            {
                counter += 1;
                return FF4(n / 2, arr) + FF4(n / 3, arr);
            }
            return n;
        }

        public static double TestFunc(Func<int[], long> T, uint n, int value)
        {
            int[] data = new int[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = value;
            }

            var before = DateTime.Now;
            counter = 0;
            T(data);
            var duration = DateTime.Now - before;
            return duration.TotalSeconds;
        }

        static void TestMethod(Func<int[], long> F, string format, int bestValue, uint[] best, int worstValue, uint[] worst)
        {
            Console.WriteLine("- Best:");
            foreach (var n in best)
            {
                double time = TestFunc(F, n, bestValue);
                Console.WriteLine(format, n, time, counter);
            }
            Console.WriteLine("- Worst:");
            foreach (var n in worst)
            {
                double time = TestFunc(F, n, worstValue);
                Console.WriteLine(format, n, time, counter);
            }
        }


        static void Main(string[] args)
        {
            string format = "  F({0}) => {1}s {2}i";
            //string format = "{1} {2}";
            
            /*
            Console.WriteLine("methodToAnalysis1:");
            TestMethod(
                methodToAnalysis1,
                format,
                -1, new uint[] { 10000, 100000000, 300000000, 600000000, 900000000 },
                 1, new uint[] { 500, 600, 700, 800, 1000 }
            );
            */

            Console.WriteLine("methodToAnalysis2:");
            TestMethod(
                methodToAnalysis2,
                format,
                 1, new uint[] { 10000, 100000000, 300000000, 600000000, 900000000 },
                -1, new uint[] { 10000, 20000, 30000, 50000, 100000 }
            );
        }
    }
}
