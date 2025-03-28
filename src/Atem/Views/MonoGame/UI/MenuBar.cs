using ImGuiNET;

namespace Atem.Views.MonoGame.UI
{
    internal class MenuBar
    {
        private int _height = 19;

        public delegate void ExitEvent();
        public event ExitEvent OnExit;
        public delegate void DebugEvent();
        public event DebugEvent OnDebug;
        public delegate void SaveStateEvent();
        public event SaveStateEvent OnSaveState;
        public delegate void LoadStateEvent();
        public event LoadStateEvent OnLoadState;

        public int Height { get => _height; }

        public void Draw()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Debug"))
                    {
                        OnDebug?.Invoke();
                    }

                    if (ImGui.MenuItem("Save State"))
                    {
                        OnSaveState?.Invoke();
                    }

                    if (ImGui.MenuItem("Load State"))
                    {
                        OnLoadState?.Invoke();
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Exit"))
                    {
                        OnExit?.Invoke();
                    }

                    ImGui.EndMenu();
                }
            }

            ImGui.EndMainMenuBar();
        }
    }
}
