using System;
using System.Collections.Generic;
using ImGuiNET;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.UI.Window
{
    public class OptionsWindow
    {
        private readonly View _view;
        private bool _active;
        private float _volume;
        private int _selectedScreenSizeFactor;

        public bool Active { get => _active; set => _active = value; }

        public delegate void OnSetVolumeEvent(float volume);
        public event OnSetVolumeEvent OnSetVolume;

        public OptionsWindow(View view)
        {
            _view = view;
            _volume = view.Atem.Bus.Audio.UserVolumeFactor * 100;
            _selectedScreenSizeFactor = Math.Clamp((int)_view.Screen.SizeFactor - 1, 0, 5);
        }

        public void Draw()
        {
            ImGui.SetNextWindowDockID(ImGui.GetID("Root"));

            ImGui.Begin("Options", ref _active, ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoCollapse);

            ImGui.BeginTabBar("Options Tab");

            if (ImGui.BeginTabItem("Controls"))
            {
                ImGui.BeginChild("ControlsChild");
                ImGui.BeginTable("RebindTable", 2);

                int buttonIndex = 0;
                foreach ((CommandType type, List<Keybind> keybindList) in _view.InputManager.Keybinds)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(type.ToString());
                    ImGui.TableNextColumn();

                    if (keybindList.Count == 0)
                    {
                        if (_view.InputManager.Binding && _view.InputManager.BindingType == type)
                        {
                            ImGui.Button("~Press A Key~##" + buttonIndex);
                        }
                        else
                        {
                            if (ImGui.Button("Unbound##" + buttonIndex))
                            {
                                _view.InputManager.Binding = true;
                                _view.InputManager.BindingType = type;
                            }
                        }
                    }
                    else
                    {
                        foreach (Keybind keybind in keybindList)
                        {
                            if (keybind == _view.InputManager.Rebinding)
                            {
                                ImGui.Button("~Press A Key~##" + buttonIndex);
                            }
                            else
                            {
                                string label = (keybind.Shift ? "Shift+" : "") + keybind.Key.ToString() + "##" + buttonIndex;

                                if (ImGui.Button(label))
                                {
                                    _view.InputManager.Rebinding = keybind;
                                }
                            }
                        }
                    }

                    buttonIndex++;
                }

                ImGui.EndTable();
                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Audio"))
            {
                ImGui.BeginChild("AudioChild");

                ImGui.Text("Volume");
                ImGui.SameLine();
                if (ImGui.SliderFloat("##UserVolumeFactor", ref _volume, 0.0f, 100.0f, "%.0f"))
                {
                    _view.Atem.Bus.Audio.UserVolumeFactor = _volume / 100;
                    _volume = _view.Atem.Bus.Audio.UserVolumeFactor * 100;
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Video"))
            {
                ImGui.BeginChild("VideoChild");

                ImGui.Text("Screen Size");
                ImGui.SameLine();
                if (ImGui.Combo("##ScreenSizeFactor", ref _selectedScreenSizeFactor, "1x\02x\03x\04x\05x\06x"))
                {
                    _view.Screen.SizeFactor = _selectedScreenSizeFactor + 1;
                    _view.UpdateWindowSize();
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();

            ImGui.End();
        }
    }
}
