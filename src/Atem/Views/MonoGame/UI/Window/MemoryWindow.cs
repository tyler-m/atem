using System.Numerics;
using ImGuiNET;
using Atem.Core;
using Atem.Core.Memory;

namespace Atem.Views.MonoGame.UI.Window
{
    /// <summary>
    /// A UI window that displays a hexadecimal memory viewer using ImGui.
    /// </summary>
    public class MemoryWindow
    {
        private const int BYTES_PER_ROW = 16; // number of memory bytes to show per row
        private readonly IMemoryProvider _memoryProvider;
        private readonly int _rowCount;
        private readonly int[] _valueCache = new int[BYTES_PER_ROW];

        public MemoryWindow(IMemoryProvider memoryProvider)
        {
            _memoryProvider = memoryProvider;
            _rowCount = Bus.Size / BYTES_PER_ROW;
        }

        /// <summary>
        /// Draws the memory window with hexadecimal and ASCII views of memory content.
        /// </summary>
        public void Draw()
        {
            ImGui.SetNextWindowSizeConstraints(new Vector2(-1, 400), new Vector2(-1, 800));
            ImGui.Begin("Memory");
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

            float rowHeight = ImGui.GetTextLineHeight();

            // use ImGuiListClipper to efficiently render only the visible portion of the memory
            ImGuiListClipper clipper;
            ImGuiListClipperPtr clipperPointer;

            unsafe
            {
                clipperPointer = &clipper;
                ImGuiNative.ImGuiListClipper_Begin(clipperPointer, _rowCount, rowHeight);

                while (ImGuiNative.ImGuiListClipper_Step(clipperPointer) == 1)
                {
                    for (int line = clipper.DisplayStart; line < clipper.DisplayEnd; line++)
                    {
                        int baseAddress = line * BYTES_PER_ROW;

                        // print base address of the row (e.g., 0x0042:)
                        ImGui.Text($"{baseAddress:X4}:  ");

                        // print each byte of the line in hex format
                        for (int column = 0; column < BYTES_PER_ROW; column++)
                        {
                            _valueCache[column] = _memoryProvider.Read((ushort)(baseAddress + column));
                            int value = _valueCache[column];

                            ImGui.SameLine();
                            if (value != 0x00)
                            {
                                ImGui.Text($"{value:X2} ");
                            }
                            else
                            {
                                // 0x00 bytes are shown as disabled
                                ImGui.TextDisabled($"{value:X2} ");
                            }
                        }

                        ImGui.SameLine();
                        ImGui.Text("  ");

                        // print an ASCII representation of the row's bytes
                        for (int column = 0; column < BYTES_PER_ROW; column++)
                        {
                            int value = _valueCache[column];
                            ImGui.SameLine();

                            if (value >= 32 && value <= 126)
                            {
                                ImGui.Text($"{(char)value}");
                            }
                            else
                            {
                                ImGui.TextDisabled(".");
                            }
                        }
                    }
                }

                ImGuiNative.ImGuiListClipper_End(clipperPointer);
            }

            // restore previous style
            ImGui.PopStyleVar(1);
            ImGui.End();
        }
    }
}
