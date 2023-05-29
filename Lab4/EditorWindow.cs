using System.Numerics;
using ImGuiNET;
using Lab4;
using Raylib_cs;

namespace Lab4
{
    class EditorWindow
    {
        private bool showDemoWindow = false;
        public ImFontPtr font1;

        MainWindow win;

        public EditorWindow(MainWindow win)
        {
            this.win = win;
        }

        public void Update(float dt)
        {
            ImGui.PushFont(font1);

            if (showDemoWindow)
            {
                // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                // Here we just want to make the demo initial state a bit more friendly!
                ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref showDemoWindow);
            }

            if (ImGui.Begin("Editor", ImGuiWindowFlags.None))
            {
                var algo = win.algo;

                var mouse = win.GetMousePosition();
                if (win.isIterationRunning)
                {
                    if (ImGui.Button(win.iterationPaused ? "Continue" : "Pause"))
                    {
                        win.iterationPaused = !win.iterationPaused;
                    }
                    if (ImGui.Button("Stop"))
                    {
                        win.Stop();
                    }
                } else
                {
                    if (ImGui.Button("Start"))
                    {
                        win.Start();
                    }
                }
                ImGui.Text($"Mouse: {mouse.X:f3} {mouse.Y:f3}");
                ImGui.Text($"Iteration: {algo.iteration}");
                ImGui.Text($"Runtime: {win.interationTime:f3}s");

                ImGuiInputTextFlags inputFlags = 0;
                if (win.isIterationRunning)
                {
                    uint bg = ImGui.GetColorU32(ImGuiCol.FrameBg);
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x44000000 | (bg & 0x00FFFFFF));
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
                    inputFlags = ImGuiInputTextFlags.ReadOnly;
                }

                ImGui.InputInt("Seed", ref algo.seed, 1, 1, ImGuiInputTextFlags.CharsDecimal | inputFlags);
                ImGui.InputInt("Batch size", ref algo.batchSize, 1, 1, ImGuiInputTextFlags.CharsDecimal | inputFlags);
                
                if (win.isIterationRunning)
                {
                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();
                }

                var localRadius = algo.localAreaRadius;
                if (ImGui.InputFloat("Local area radius", ref localRadius))
                {
                    Console.WriteLine(localRadius);
                    win.algo.SetLocalRadius(localRadius);
                }
                ImGui.SliderFloat("Mutation swap chance", ref algo.mutationSwapChance, 0, 0.3f);
                ImGui.SliderFloat("Mutation repick chance", ref algo.mutationPickUniqueChance, 0, 0.3f);
                ImGui.SliderFloat("Mutation repick local chance", ref algo.mutationPickLocalChance, 0, 0.3f);

                ImGui.Text($"Best path score: {win.bestScore}");

                ImGui.End();
            }

            ImGui.PopFont();
        }
    }
}