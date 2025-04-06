using ImGuiNET;
using System.Numerics;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class MemoryWindow
    {
        private View _view;
        private int _bytesPerRow = 16;
        private int _memorySize = 65536; // maybe should be a const somewhere in Atem.Core
        private int _rowCount;

        public MemoryWindow(View view)
        {
            _view = view;
            _rowCount = _memorySize / _bytesPerRow;
        }

        public void Draw()
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(-1, 400), new Vector2(-1, 800));
            ImGui.Begin("Memory");
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

            float rowHeight = ImGui.GetTextLineHeight();
            ImGuiListClipper clipper;
            ImGuiListClipperPtr clipperPointer;

            unsafe
            {
                clipperPointer = &clipper;
                ImGuiNative.ImGuiListClipper_Begin(clipperPointer, _rowCount, rowHeight);
            }

            unsafe
            {
                while (ImGuiNative.ImGuiListClipper_Step(clipperPointer) == 1)
                {
                    for (int line = clipper.DisplayStart; line < clipper.DisplayEnd; line++)
                    {
                        int baseAddress = line * _bytesPerRow;
                        ImGui.Text(baseAddress.ToString($"X{4}") + ":  ");

                        for (int column = 0; column < _bytesPerRow; column++)
                        {
                            int value = _view.Atem.Bus.Read((ushort)(baseAddress + column));
                            ImGui.SameLine();
                            if (value != 0)
                            {
                                ImGui.Text(value.ToString($"X{2}") + " ");
                            }
                            else
                            {
                                ImGui.TextDisabled(value.ToString($"X{2}") + " ");
                            }
                        }

                        ImGui.SameLine();
                        ImGui.Text("  ");

                        for (int column = 0; column < _bytesPerRow; column++)
                        {
                            int value = _view.Atem.Bus.Read((ushort)(baseAddress + column));
                            ImGui.SameLine();
                            if (value >= 32 && value <= 126)
                            {
                                ImGui.Text(((char)value).ToString());
                            }
                            else
                            {
                                ImGui.TextDisabled(".");
                            }
                        }
                    }
                }
            }

            ImGui.PopStyleVar(1);
            ImGui.End();
        }
    }
}
