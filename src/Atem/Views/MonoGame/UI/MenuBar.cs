using ImGuiNET;

namespace Atem.Views.MonoGame.UI
{
    internal class MenuBar
    {
        private bool _visible = true;

        public delegate void ExitEvent();
        public event ExitEvent OnExit;

        public int Height { get; private set; }

        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                if (!_visible)
                {
                    Height = 0;
                }
            }
        }

        public void Draw()
        {
            if (Visible)
            {
                if (ImGui.BeginMainMenuBar())
                {
                    Height = (int)ImGui.GetFrameHeight();

                    if (ImGui.BeginMenu("File", false))
                    {
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
}
