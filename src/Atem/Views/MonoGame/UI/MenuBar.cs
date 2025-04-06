using ImGuiNET;

namespace Atem.Views.MonoGame.UI
{
    internal class MenuBar
    {
        private int _height = 19;
        private bool _enableStates;

        public delegate void ExitEvent();
        public event ExitEvent OnExit;
        public delegate void DebugEvent();
        public event DebugEvent OnDebug;
        public delegate void SaveStateEvent(int slot);
        public event SaveStateEvent OnSaveState;
        public delegate void LoadStateEvent(int slot);
        public event LoadStateEvent OnLoadState;
        public delegate void OnOpenEvent();
        public event OnOpenEvent OnOpen;
        public delegate void OnOptionsEvent();
        public event OnOptionsEvent OnOptions;

        public int Height { get => _height; }

        public bool EnableStates { get => _enableStates; set => _enableStates = value; }

        public void Draw()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..."))
                    {
                        OnOpen?.Invoke();
                    }

                    if (ImGui.MenuItem("Debug"))
                    {
                        OnDebug?.Invoke();
                    }

                    if (ImGui.BeginMenu("Save State", _enableStates))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (ImGui.MenuItem("Slot " + i))
                            {
                                OnSaveState?.Invoke(i);
                            }
                        }
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Load State", _enableStates))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (ImGui.MenuItem("Slot " + i))
                            {
                                OnLoadState?.Invoke(i);
                            }
                        }
                        ImGui.EndMenu();
                    }

                    ImGui.Separator();

                    if (ImGui.MenuItem("Exit"))
                    {
                        OnExit?.Invoke();
                    }

                    ImGui.EndMenu();
                }
                
                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Options..."))
                    {
                        OnOptions?.Invoke();
                    }

                    ImGui.EndMenu();
                }
            }

            ImGui.EndMainMenuBar();
        }
    }
}
