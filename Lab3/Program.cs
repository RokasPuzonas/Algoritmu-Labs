using System.Reflection.Metadata.Ecma335;

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

        public static long methodToAnalysis2(int n, int[] arr)
        {
            long k = 0;

            for (int i = 0; i < n; i++)
            {
                k += arr[i] + FF4(i, arr);
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

        static void Main(string[] args)
        {
            int weightLimit = 15;
            int[] weights = new int[]{ 7, 2, 1, 9 };
            int[] values  = new int[]{ 5, 4, 7, 2 };
            int n = weights.Length;
            Console.WriteLine(knapsackDP(weightLimit, n, weights, values));
            Console.WriteLine(knapsack(weightLimit, n, weights, values));
        }
    }
}