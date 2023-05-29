using Microsoft.VisualBasic;
using Raylib_cs;
using System.Diagnostics;

namespace Lab4
{
    internal class Program
    {
        static unsafe void Main(string[] args)
        {
            var places = Algorithm.ReadPlacesFromTSV("places.tsv");

            var win = new MainWindow(1280, 720, places);
            win.Run();

            //var algo = new Algorithm(places);
            //algo.IterateSolutionFor(1);
            //Console.WriteLine("Best after {0} iterations is {1}", algo.iteration, algo.GetBestPath().Item1);
            //Console.WriteLine("Seed: {0}", algo.seed);

            // Best after 484012 iterations is 30826.444176719036
            // Time taken: 60000ms
            // Seed: -1477536140
        }
    }
    
}