using System;
using System.Collections.Generic;
using System.IO;

namespace Lab1
{
    class Program
    {
        // O(n^5)
        public static double T1(int[] n, int size)
        {
            if (size >= 1)
            {
                int sum = 0;
                for (int i = 0; i < Math.Pow(size, 5); i++)
                {
                    sum += i;
                }
                return 2 * T1(n, size / 9) + sum;
            }
            return 0;
        }

        public static double T1(int[] n)
        {
            return T1(n, n.Length);
        }

        // O(n)
        public static double T2(int[] n, int size)
        {
            if (size >= 1)
            {
                int sum = 0;
                for (int i = 0; i < size; i++)
                {
                    sum += i;
                }
                return T2(n, size / 6) + T2(n, size / 7) + sum;
            }
            return 0;
        }

        public static double T2(int[] n)
        {
            return T2(n, n.Length);
        }

        // O(n^2)
        public static double T3(int[] n, int size)
        {
            if (size >= 1)
            {
                int sum = 0;
                for (int i = 0; i < size; i++)
                {
                    sum += i;
                }
                return T3(n, size - 8) + T3(n, size - 6) + sum;
            }
            return 0;
        }

        public static double T3(int[] n)
        {
            return T3(n, n.Length);
        }

        // O(log4(n))
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
            BMPImage image = new BMPImage(1000, 1000);
            image.Fill(new Color(255, 255, 255));
            TrianglesRecursive(image, 0, 0, image.width, image.height);
            image.Write("result.bmp");
        }

        public static double TestFunc(Func<int[], double> T, uint n)
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
            // T1(10) => 0.0056552
            // T1(20) => 0.0699722
            // T1(40) => 2.1500858
            // T1(50) => 6.4564238

            // T2(100)           => 0.0039419
            // T2(1_000_000)     => 0.0048064
            // T2(250_000_000)   => 0.3011757
            // T2(500_000_000)   => 0.6046134
            // T2(1_000_000_000) => 1.1902643

            // T3(100) => 0.0041945
            // T3(150) => 0.0759872
            // T3(180) => 1.4428123
            // T3(190) => 3.9067703

            //Triangles();
        }
    }
}
