using System.IO;
using ImGuiNET;
using Atem.IO;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class FileBrowserWindow
    {
        private readonly FileBrowser _fileBrowser;
        private readonly string[] _extensions = [".gb", ".gbc"];
        private bool _active;

        public delegate void SelectFileEvent(FileInfo fileInfo);
        public event SelectFileEvent OnSelectFile;

        public bool Active { get => _active; set => _active = value; }

        public FileBrowserWindow()
        {
            _fileBrowser = new FileBrowser(Directory.GetCurrentDirectory());
        }

        public void Draw()
        {
            ImGui.SetNextWindowDockID(ImGui.GetID("Root"));

            ImGui.Begin("File Browser", ref _active, ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoCollapse);

            if (ImGui.ArrowButton("NavigateUp", ImGuiDir.Up))
            {
                _fileBrowser.NavigateUp();
            }

            ImGui.SameLine();
            ImGui.TextDisabled(_fileBrowser.CurrentDirectory.FullName);

            foreach (DirectoryInfo directory in _fileBrowser.GetDirectories())
            {
                if (ImGui.Selectable("> " + directory.Name))
                {
                    _fileBrowser.NavigateTo(directory);
                }
            }

            foreach (FileInfo file in _fileBrowser.GetFiles(_extensions))
            {
                if (ImGui.Selectable(file.Name))
                {
                    OnSelectFile?.Invoke(file);
                }
            }

            ImGui.End();
        }
    }
}
