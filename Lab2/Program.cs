using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

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

        public static double TimeFunc(Action Func)
        {
            var before = DateTime.Now;
            Func();
            var duration = DateTime.Now - before;
            return duration.TotalSeconds;
        }

        public static double TestFunc<R>(Func<int[], R> T, uint n, int value)
        {
            int[] data = new int[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = value;
            }

            counter = 0;
            return TimeFunc(() => T(data));
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

        static uint Recursive(Span<uint> board)
        {
            if (board.Length <= 1)
                return 0;
            else if (board.Length == 2)
                return board[1];

            uint score1 = board[1] + Recursive(board[1..]);
            uint score2 = 2 * (uint)Math.Abs((int)board[0] - (int)board[2]) + Recursive(board[2..]);

            return Math.Max(score1, score2);
        }

        static uint DP(uint[] board)
        {
            uint[] cache = new uint[board.Length];
            cache[0] = 0;
            cache[1] = board[1];

            for (int i = 2; i < board.Length; i++)
            {
                uint score1 = board[i] + cache[i - 1];
                uint score2 = 2 * (uint)Math.Abs((int)board[i] - (int)board[i - 2]) + cache[i - 2];

                cache[i] = Math.Max(score1, score2);
            }

            return cache[board.Length - 1];
        }


        static void Main(string[] args)
        {

            string format = "  Recursive({0}) => {1}s";
            foreach (var size in new uint[] { 10, 30, 35, 37 })
            {
                uint[] board = new uint[size];
                for (uint i = 0; i < size; i++)
                {
                    board[i] = i;
                }

                double time = TimeFunc(() => Recursive(board));
                Console.WriteLine(format, size, time, counter);
            }
            

            format = "  DP({0}) => {1}s";
            foreach (var size in new uint[] { 1000, 10000000, 30000000, 60000000, 180000000 })
            {
                uint[] board = new uint[size];
                for (uint i = 0; i < size; i++)
                {
                    board[i] = i;
                }

                double time = TimeFunc(() => DP(board));
                Console.WriteLine(format, size, time, counter);
            }



            /*
            
            //string format = "{1} {2}";

            Console.WriteLine("methodToAnalysis1:");
            TestMethod(
                methodToAnalysis1,
                format,
                -1, new uint[] { 10000, 100000000, 300000000, 600000000, 900000000 },
                 1, new uint[] { 500, 600, 700, 800, 1000 }
            );
            

            Console.WriteLine("methodToAnalysis2:");
            TestMethod(
                methodToAnalysis2,
                format,
                    1, new uint[] { 10000, 100000000, 300000000, 600000000, 900000000 },
                -1, new uint[] { 10000, 20000, 30000, 50000, 100000 }
            ); 
            */
        }
    }
}
