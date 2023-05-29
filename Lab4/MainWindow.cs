using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Lab4.Algorithm;

namespace Lab4
{
    public class MainWindow
    {
        public Algorithm algo;
        List<Place> places;
        public int screenWidth;
        public int screenHeight;

        public float interationTime = 0;
        Task? iterationTask;

        public List<int>? bestPath;
        public double bestScore = 0;
        public bool isIterationRunning = false;

        public bool iterationPaused = false;

        RaylibDrawContext ctx;

        ScoreHistWindow scoreHist;

        public MainWindow(int screenWidth, int screenHeight, List<Place> places)
        {
            this.places = places;
            algo = new Algorithm(places);
            scoreHist = new ScoreHistWindow(this);

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            {
                ctx = new RaylibDrawContext();

                const int screenMargin = 50;

                var (minX, maxX, minY, maxY) = GetCoordinateBounds(places);
                var rangeX = maxX - minX;
                var rangeY = maxY - minY;
                float scale = Math.Min((screenWidth - screenMargin * 2) / rangeX, (screenHeight - screenMargin * 2) / rangeY);
                ctx.Scale(scale, scale);
                ctx.Translate(-screenMargin, -screenMargin);
                ctx.Translate(minX * scale, minY * scale);
            }
        }

        public void Start()
        {
            algo.Reset();
            scoreHist.Reset();
            isIterationRunning = true;
            iterationPaused = false;
            interationTime = 0;
            iterationTask = new Task(() => {
                while (isIterationRunning)
                {
                    if (!iterationPaused)
                    {
                        algo.IterateSolution();
                        var best = algo.GetBestPath();
                        bestScore = best.Item1;
                        bestPath = best.Item2;
                    }
                }
            });
            iterationTask.Start();
        }

        public void Stop()
        {
            isIterationRunning = false;
        }

        public static Tuple<float, float, float, float> GetCoordinateBounds(List<Place> places)
        {
            float minX = places[0].x;
            float maxX = places[0].x;
            float minY = places[0].y;
            float maxY = places[0].y;

            for (int i = 1; i < places.Count; i++)
            {
                minX = Math.Min(minX, places[i].x);
                maxX = Math.Max(maxX, places[i].x);
                minY = Math.Min(minY, places[i].y);
                maxY = Math.Max(maxY, places[i].y);
            }

            return Tuple.Create(minX, maxX, minY, maxY);
        }

        public static void DrawPath(RaylibDrawContext ctx, List<int> path, List<Place> places)
        {
            Color color = Color.GREEN;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var place0 = places[path[i + 0]];
                var place1 = places[path[i + 1]];
                ctx.DrawLine(place0.x, place0.y, place1.x, place1.y, color);
            }
        }

        public Vector2 GetMousePosition()
        {
            return ctx.TransformToLocalSpace(Raylib.GetMousePosition());
        }

        public Place? GetNearestPlaceToMouse()
        {
            if (places.Count == 0) return null;

            var mouse = GetMousePosition();
            var mearest = places.First();
            double nearestDistance = GetDistance(mearest.x, mearest.y, mouse.X, mouse.Y);
            foreach (var place in places)
            {
                var distance = GetDistance(place.x, place.y, mouse.X, mouse.Y);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    mearest = place;
                }
            }
            return mearest;
        }

        public unsafe void Run()
        {
            Raylib.SetTraceLogCallback(&Logging.LogConsole);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(this.screenWidth, this.screenHeight, "IP Užduotis");
            Raylib.SetTargetFPS(60);

            Raylib.InitAudioDevice();

            ImguiController controller = new ImguiController();
            EditorWindow editor = new EditorWindow(this);

            controller.Load(screenWidth, screenHeight);
            while (!Raylib.WindowShouldClose())
            {
                // Update
                float dt = Raylib.GetFrameTime();
                controller.Update(dt);
                editor.Update(dt);
                scoreHist.Update(dt);
                if (isIterationRunning && !iterationPaused)
                {
                    interationTime += dt;
                }
                

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);

                foreach (var place in places)
                {
                    ctx.DrawCircle(place.x, place.y, 2000, Color.RED);
                }

                if (bestPath != null)
                {
                    DrawPath(ctx, bestPath, places);
                }

                /*
                var nearest = GetNearestPlaceToMouse();
                if (nearest != null)
                {
                    var mouse = GetMousePosition();
                    ctx.DrawLine(nearest.x, nearest.y, mouse.X, mouse.Y, Color.BLUE);
                    ctx.DrawCircleLines(nearest.x, nearest.y, algo.localAreaRadius, Color.BLUE);
                }
                */

                controller.Draw();

                Raylib.EndDrawing();
            }

            // De-Initialization
            controller.Dispose();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
            Stop();
        }
    }
}
