using Atem.Core.Debugging;
using ImGuiNET;
using System.Collections.Generic;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class BreakpointWindow
    {
        private byte[] _inputAddressText = new byte[5];
        private Debugger _debugger;
        private Dictionary<Breakpoint, bool> _selected = [];

        public BreakpointWindow(Debugger debugger)
        {
            _debugger = debugger;
        }

        public void Draw()
        {
            ImGui.Begin("Breakpoints");

            ImGui.SetNextItemWidth(60);
            ImGui.InputText("##Address", _inputAddressText, 5, ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.CharsUppercase);
            ImGui.SameLine();

            if (ImGui.Button("Add"))
            {
                int nullTerminatorIndex = System.Array.IndexOf(_inputAddressText, (byte)0);
                ushort value = ushort.Parse(System.Text.Encoding.ASCII.GetString(_inputAddressText, 0, nullTerminatorIndex), System.Globalization.NumberStyles.HexNumber);
                Breakpoint breakpoint = _debugger.AddBreakpoint(value);

                if (breakpoint != null)
                {
                    _selected.Add(breakpoint, false);
                }
            }

            ImGui.Separator();
            ImGui.BeginChild("BreakpointList");

            if (ImGui.BeginTable("BreakpointTable", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn("Address");
                ImGui.TableSetupColumn("Hit Count");
                ImGui.TableHeadersRow();

                for (int i = 0; i < _debugger.BreakpointCount; i++)
                {
                    Breakpoint breakpoint = _debugger.GetBreakpoint(i);
                    ushort address = breakpoint.Address;

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Checkbox("##Breakpoint" + i, ref breakpoint.Enabled);
                    ImGui.SameLine();
                    if (ImGui.Selectable(address.ToString($"X{4}"), _selected[breakpoint], ImGuiSelectableFlags.SpanAllColumns))
                    {
                        _selected[breakpoint] = !_selected[breakpoint];
                    }
                    ImGui.TableNextColumn();
                    ImGui.Text(breakpoint.HitCount.ToString());
                }

                ImGui.EndTable();

                if (ImGui.BeginPopupContextItem("BreakpointContextMenu"))
                {
                    if (ImGui.MenuItem("Remove"))
                    {
                        foreach ((Breakpoint breakpoint, bool selected) in _selected)
                        {
                            if (selected && _debugger.RemoveBreakpoint(breakpoint))
                            {
                                _selected.Remove(breakpoint);
                            }
                        }
                    }
                    ImGui.EndPopup();
                }
            }

            ImGui.EndChild();
            ImGui.End();
        }
    }
}
