using System.Numerics;
using System.IO;
using ImGuiNET;

namespace Atem.Views.MonoGame.UI.Window
{
    internal class FileExplorerWindow
    {
        private DirectoryInfo _cwd;
        private DirectoryInfo _directory;

        public delegate void SelectFileEvent(string filePath);
        public event SelectFileEvent OnSelectFile;

        public bool Active { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public FileExplorerWindow( int width, int height, bool active = true)
        {
            Width = width;
            Height = height;
            Active = active;

            _cwd = new DirectoryInfo(Directory.GetCurrentDirectory());
            _directory = new DirectoryInfo(_cwd.FullName);
        }

        private void SelectFile(FileInfo file)
        {
            OnSelectFile?.Invoke(file.FullName);
        }

        private void SelectDirectory(DirectoryInfo directory)
        {
            _directory = directory;
        }

        public void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(Width, Height), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always);

            ImGui.Begin("File Explorer", ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoDocking
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoSavedSettings
                | ImGuiWindowFlags.NoCollapse);

            if (ImGui.ArrowButton("UpDirectory", ImGuiDir.Up))
            {
                if (_directory.Parent != null)
                {
                    _directory = _directory.Parent;
                }
            }

            ImGui.SameLine();
            ImGui.TextDisabled(_directory.FullName);

            foreach (DirectoryInfo directory in _directory.EnumerateDirectories())
            {
                if (ImGui.Selectable("> " + directory.Name))
                {
                    SelectDirectory(directory);
                }
            }

            foreach (FileInfo file in _directory.EnumerateFiles())
            {
                if (ImGui.Selectable(file.Name))
                {
                    SelectFile(file);
                }
            }

            ImGui.End();
        }
    }
}
