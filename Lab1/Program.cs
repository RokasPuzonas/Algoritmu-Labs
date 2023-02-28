using System;
using System.Collections.Generic;
using System.IO;

namespace Lab1
{
    class Program
    {
        public static int T1(int[] n, int size)
        {
            if (size < 1)
                return 0;

            int sum = 0;
            for (int i = 0; i < Math.Pow(size, 5); i++)
            {
                sum++;
            }
            return T1(n, size / 9) + T1(n, size / 9) + sum;
        }

        public static int T1(int[] n)
        {
            return T1(n, n.Length);
        }

        public static int T2(int[] n, int size)
        {
            if (size < 1)
                return 0;

            int sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum++;
            }
            return T2(n, size / 6) + T2(n, size / 7) + sum;
        }

        public static int T2(int[] n)
        {
            return T2(n, n.Length);
        }

        public static int T3(int[] n, int size)
        {
            if (size < 1)
                return 0;

            int sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum++;
            }
            return T3(n, size - 8) + T3(n, size - 6) + sum;
        }

        public static int T3(int[] n)
        {
            return T3(n, n.Length);
        }

        public static void TrianglesRecursive(BMPImage image, int x, int y, uint width, uint height)
        {
            if (width <= 3 || height <= 3) return;

            image.Triangle(
                x + (int)(width / 3 * 1  ), (y + (int)height / 3 * 2),
                x + (int)(width / 3 * 2  ), (y + (int)height / 3 * 2),
                x + (int)(width / 3 * 1.5), (y + (int)height / 3 * 1),
                new Color(0, 0, 0)
            );

            TrianglesRecursive(
                image, x, y, width/3, height/3
            );
            TrianglesRecursive(
                image, x + (int)width/3*2, y, width / 3, height / 3
            );
            TrianglesRecursive(
                image, x, y + (int)height/3*2, width / 3, height / 3
            );
            TrianglesRecursive(
                image, x + (int)width / 3 * 2, y + (int)height / 3 * 2, width / 3, height / 3
            );
        }

        public static void Triangles()
        {
            uint size = 40_000;
            BMPImage image = new BMPImage(size, size);
            image.Fill(new Color(255, 255, 255));
            var before = DateTime.Now;
            TrianglesRecursive(image, 0, 0, image.width, image.height);
            var duration = DateTime.Now - before;
            Console.WriteLine("Triangles render duration: {0}", duration.TotalSeconds);
            image.Write("result.bmp");
        }

        public static double TestFunc(Func<int[], int> T, uint n)
        {
            int[] data = new int[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = i;
            }

            var before = DateTime.Now;
            T(data);
            var duration = DateTime.Now - before;
            return duration.TotalSeconds;
        }

        static void Main()
        {
            /*
            uint[] N1 = { 10, 15, 20, 25, 30, 35, 40 };
            Console.WriteLine("T1, s");
            foreach (uint n in N1)
            {
                Console.WriteLine("{0}, {1}", n, TestFunc(T1, n));
            }
            */

            /*
            uint[] N2 = { 100, 1_000_000, 250_000_000, 500_000_000, 1_000_000_000, 1_500_000_000 };
            Console.WriteLine("T2, s");
            foreach (uint n in N2)
            {
                Console.WriteLine("{0}, {1}", n, TestFunc(T2, n));
            }
            */

            /*
            uint[] N3 = { 100, 150, 160, 170, 180, 185 };
            Console.WriteLine("T3, s");
            foreach (uint n in N3)
            {
                Console.WriteLine("{0}, {1}", n, TestFunc(T3, n));
            }
            */

            Triangles();
        }
    }
}
