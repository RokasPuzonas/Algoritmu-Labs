using ImGuiNET;

namespace Lab4
{
    internal class ScoreHistWindow
    {
        public ImFontPtr font1;
        MainWindow win;

        float[] dataPoints;

        float timer = 0;
        float lastSampleTime = -1;
        float sampleInterval = 0.1f;

        public ScoreHistWindow(MainWindow win)
        {
            this.win = win;
            dataPoints = new float[100];
        }

        public void Reset()
        {
            dataPoints = new float[100];
            timer = 0;
            lastSampleTime = -1;
        }

        public unsafe void Update(float dt)
        {
            if (win.isIterationRunning && !win.iterationPaused)
            {
                timer += dt;
                if (timer - lastSampleTime > sampleInterval)
                {
                    for (int i = 0; i < dataPoints.Length-1; i++)
                    {
                        dataPoints[i] = dataPoints[i + 1];
                    }
                
                    dataPoints[dataPoints.Length - 1] = (float)win.bestScore;

                    lastSampleTime = timer;
                }
            }

            ImGui.PushFont(font1);

            if (ImGui.Begin("Score histogram", ImGuiWindowFlags.None))
            {
                ImGui.PlotLines("", ref dataPoints[0], dataPoints.Length, 0, null, 10_000f, 50_000f, new System.Numerics.Vector2(0, 180.0f));
                ImGui.Spacing();
                ImGui.End();
            }

            ImGui.PopFont();
        }
    }
}
