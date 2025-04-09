using System;
using System.Numerics;
using ImGuiNET;
using Atem.Core.Processing;

namespace Atem.Views.MonoGame.UI.Window
{
    public class ProcessorRegistersWindow
    {
        private IProcessor _processor;

        public ProcessorRegistersWindow(IProcessor processor)
        {
            _processor = processor;
        }

        public void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(171, 118), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            ImGui.Begin("CPU Registers", ImGuiWindowFlags.NoResize);

            ImGui.Text("PC: " + _processor.Registers.PC.ToString("X4"));
            ImGui.SameLine();
            ImGui.Text(" SP: " + _processor.Registers.SP.ToString("X4"));
            ImGui.Text(" A: " + _processor.Registers.A.ToString("X4"));
            ImGui.SameLine();
            ImGui.Text("  F: " + Convert.ToString(_processor.Registers.AF & 0x00FF, 2).PadLeft(8, '0'));
            ImGui.Text(" B: " + _processor.Registers.B.ToString("X4"));
            ImGui.SameLine();
            ImGui.Text("  C: " + _processor.Registers.C.ToString("X4"));
            ImGui.Text(" D: " + _processor.Registers.D.ToString("X4"));
            ImGui.SameLine();
            ImGui.Text("  E: " + _processor.Registers.E.ToString("X4"));
            ImGui.Text(" H: " + _processor.Registers.H.ToString("X4"));
            ImGui.SameLine();
            ImGui.Text("  L: " + _processor.Registers.L.ToString("X4"));

            ImGui.End();
        }
    }
}
