using System;
using System.Collections.Generic;
using ImGuiNET;
using Atem.Core.Audio;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.UI.Window
{
    public class OptionsWindow
    {
        private readonly IScreen _screen;
        private readonly IAudioManager _audioManager;
        private readonly InputManager _inputManager;
        private bool _active;
        private float _volume;
        private int _selectedScreenSizeFactor;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                if(_active)
                {
                    UpdateOptionsValues();
                }
            }
        }

        private void UpdateOptionsValues()
        {
            _volume = _audioManager.VolumeFactor * 100;
            _selectedScreenSizeFactor = Math.Clamp((int)_screen.SizeFactor - 1, 0, 5);
        }

        public OptionsWindow(IScreen screen, IAudioManager audioManager, InputManager inputManager)
        {
            _screen = screen;
            _inputManager = inputManager;
            _audioManager = audioManager;
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
                foreach ((CommandType type, List<Keybind> keybindList) in _inputManager.Keybinds)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(type.ToString());
                    ImGui.TableNextColumn();

                    if (keybindList.Count == 0)
                    {
                        if (_inputManager.Binding && _inputManager.BindingType == type)
                        {
                            ImGui.Button("~Press A Key~##" + buttonIndex);
                        }
                        else
                        {
                            if (ImGui.Button("Unbound##" + buttonIndex))
                            {
                                _inputManager.Binding = true;
                                _inputManager.BindingType = type;
                            }
                        }
                    }
                    else
                    {
                        foreach (Keybind keybind in keybindList)
                        {
                            if (keybind == _inputManager.Rebinding)
                            {
                                ImGui.Button("~Press A Key~##" + buttonIndex);
                            }
                            else
                            {
                                string label = "";
                                label += keybind.Shift ? "Shift+" : "";
                                label += keybind.Control ? "Control+" : "";
                                label += keybind.Alt ? "Alt+" : "";
                                label += keybind.Key.ToString() + "##" + buttonIndex;

                                if (ImGui.Button(label))
                                {
                                    _inputManager.Rebinding = keybind;
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
                    _audioManager.VolumeFactor = _volume / 100;
                    UpdateOptionsValues();
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
                    _screen.SizeFactor = _selectedScreenSizeFactor + 1;
                    UpdateOptionsValues();
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();

            ImGui.End();
        }
    }
}
