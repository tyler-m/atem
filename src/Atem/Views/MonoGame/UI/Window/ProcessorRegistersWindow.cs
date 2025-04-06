using ImGuiNET;
using System;
using System.Numerics;

namespace Atem.Views.MonoGame.UI.Window
{
    public class ProcessorRegistersWindow
    {
        private readonly View _view;

        public ProcessorRegistersWindow(View view)
        {
            _view = view;
        }

        public void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(171, 118), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(20, 20), ImGuiCond.FirstUseEver);

            ImGui.Begin("CPU Registers", ImGuiWindowFlags.NoResize);

            ImGui.Text("PC: " + _view.Atem.Bus.Processor.Registers.PC.ToString($"X{4}"));
            ImGui.SameLine();
            ImGui.Text(" SP: " + _view.Atem.Bus.Processor.Registers.SP.ToString($"X{4}"));
            ImGui.Text(" A: " + _view.Atem.Bus.Processor.Registers.A.ToString($"X{4}"));
            ImGui.SameLine();
            ImGui.Text("  F: " + Convert.ToString(_view.Atem.Bus.Processor.Registers.AF & 0x00FF, 2).PadLeft(8, '0'));
            ImGui.Text(" B: " + _view.Atem.Bus.Processor.Registers.B.ToString($"X{4}"));
            ImGui.SameLine();
            ImGui.Text("  C: " + _view.Atem.Bus.Processor.Registers.C.ToString($"X{4}"));
            ImGui.Text(" D: " + _view.Atem.Bus.Processor.Registers.D.ToString($"X{4}"));
            ImGui.SameLine();
            ImGui.Text("  E: " + _view.Atem.Bus.Processor.Registers.E.ToString($"X{4}"));
            ImGui.Text(" H: " + _view.Atem.Bus.Processor.Registers.H.ToString($"X{4}"));
            ImGui.SameLine();
            ImGui.Text("  L: " + _view.Atem.Bus.Processor.Registers.L.ToString($"X{4}"));

            ImGui.End();
        }
    }
}
