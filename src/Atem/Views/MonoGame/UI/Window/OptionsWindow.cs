using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using Atem.Core.Audio;
using Atem.Graphics;
using Atem.Input.Command;
using Atem.Input;
using Atem.IO;

namespace Atem.Views.MonoGame.UI.Window
{
    public class OptionsWindow
    {
        private readonly IScreen _screen;
        private readonly IAudioManager _audioManager;
        private readonly InputManager _inputManager;
        private readonly TCPSerialLink _serialLink;
        private bool _active;
        private bool _screenSizeLocked;
        private float _volume;
        private int _selectedScreenSizeFactor;
        private readonly bool[] _enableChannel;

        private byte[] _hostname = new byte[128];
        private int _port;

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
            _screenSizeLocked = _screen.SizeLocked;
            _selectedScreenSizeFactor = Math.Clamp(_screen.SizeFactor - 1, 0, 5);
            
            for (int i = 0; i < _enableChannel.Length; i++)
            {
                _enableChannel[i] = !_audioManager.Channels[i].UserMute;
            }
        }

        public OptionsWindow(IScreen screen, IAudioManager audioManager, InputManager inputManager, TCPSerialLink serialLink)
        {
            _screen = screen;
            _inputManager = inputManager;
            _audioManager = audioManager;
            _enableChannel = new bool[_audioManager.Channels.Count];
            _serialLink = serialLink;
            _port = _serialLink.Port;
            Encoding.ASCII.GetBytes(_serialLink.Hostname).CopyTo(_hostname, 0);
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
                                label += _inputManager.KeyProvider.GetKeyString(keybind.Key) + "##" + buttonIndex;

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

                for (int i = 0; i < _enableChannel.Length; i++)
                {
                    if (ImGui.Checkbox("##EnableChannel" + i, ref _enableChannel[i]))
                    {
                        _audioManager.Channels[i].UserMute = !_enableChannel[i];
                    }
                    ImGui.SameLine();
                    ImGui.Text("Channel " + (i + 1));
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Video"))
            {
                ImGui.BeginChild("VideoChild");

                ImGui.Text("Screen Size Locked");
                ImGui.SameLine();
                if (ImGui.Checkbox("##ScreenSizeLocked", ref _screenSizeLocked))
                {
                    _screen.SizeLocked = _screenSizeLocked;
                    UpdateOptionsValues();
                }

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

            if (ImGui.BeginTabItem("Link Cable"))
            {
                ImGui.BeginChild("LinkCableChild");

                if (!_serialLink.IsConnected)
                {
                    if (ImGui.Button("Start Hosting"))
                    {
                        _ = _serialLink.StartAsync(_port);
                    }

                    ImGui.InputText("##Address", _hostname, (uint)_hostname.Length);
                    ImGui.InputInt("##Port", ref _port);

                    if (ImGui.Button("Connect To Host"))
                    {
                        int nullTerminatorIndex = Array.IndexOf(_hostname, (byte)0);
                        string address = Encoding.ASCII.GetString(_hostname, 0, nullTerminatorIndex);
                        _ = _serialLink.ConnectAsync(address, _port);
                    }
                }
                else
                {
                    if (_serialLink.IsHost)
                    {
                        ImGui.Text($"Hosting at {_serialLink.Hostname} on port {_serialLink.Port}");
                    }
                    else
                    {
                        ImGui.Text($"Connected to {_serialLink.Hostname} on port {_serialLink.Port}");
                    }
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();

            ImGui.End();
        }
    }
}
