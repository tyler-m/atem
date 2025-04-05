using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using ImGuiNET;
using Atem.Views.MonoGame.Input;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class RebindWindow
    {
        private readonly InputManager _inputManager;

        public RebindWindow(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public void Draw()
        {
            ImGui.Begin("Rebind");
            ImGui.BeginTable("Rebind Table", 2);

            foreach (KeyValuePair<ICommand, HashSet<Keys>> command in _inputManager.Commands)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text(command.Key.Name);

                foreach (Keys value in command.Value)
                {
                    ImGui.TableNextColumn();
                    if (command.Key == _inputManager.Rebinding)
                    {
                        ImGui.Button("~Press A Key~");
                    }
                    else
                    {
                        if (ImGui.Button(value.ToString()))
                        {
                            _inputManager.Rebinding = command.Key;
                        }
                    }
                }
            }

            ImGui.EndTable();
            ImGui.End();
        }
    }
}
