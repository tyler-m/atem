using System.Collections.Generic;
using System.Linq;
using System.IO;
using ImGuiNET;
using Atem.IO;
using Atem.Saving;

namespace Atem.Views.MonoGame.UI
{
    internal class MenuBar
    {
        private int _height = 19;
        private bool _enableStates;
        private readonly ISaveStateService _saveStateService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly IRecentFilesService _recentFilesService;

        public delegate void ExitEvent();
        public event ExitEvent OnExit;
        public delegate void DebugEvent();
        public event DebugEvent OnDebug;
        public delegate void OnOpenEvent();
        public event OnOpenEvent OnOpen;
        public delegate void OnOptionsEvent();
        public event OnOptionsEvent OnOptions;
        public delegate void OnSelectRecentFileEvent(FileInfo fileInfo);
        public event OnSelectRecentFileEvent OnSelectRecentFile;

        public int Height { get => _height; }

        public bool EnableStates { get => _enableStates; set => _enableStates = value; }

        public MenuBar(ISaveStateService saveStateService, ICartridgeLoader cartridgeLoader, IRecentFilesService recentFilesService)
        {
            _saveStateService = saveStateService;
            _cartridgeLoader = cartridgeLoader;
            _recentFilesService = recentFilesService;
        }

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

                    ImGui.Separator();

                    List<FileInfo> recentFiles = _recentFilesService.RecentFilesInfo.ToList();
                    if (ImGui.BeginMenu("Recent Files", recentFiles.Count != 0))
                    {
                        for (int i = 0; i < recentFiles.Count; i++)
                        {
                            if (ImGui.MenuItem(recentFiles[i].Name.Truncate(20) + "##" + i))
                            {
                                OnSelectRecentFile?.Invoke(recentFiles[i]);
                            }
                        }
                        ImGui.EndMenu();
                    }

                    ImGui.Separator();

                    if (ImGui.BeginMenu("Save State", _enableStates))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (ImGui.MenuItem("Slot " + i))
                            {
                                _saveStateService.Save(i, _cartridgeLoader.Context);
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
                                _saveStateService.Load(i, _cartridgeLoader.Context);
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
                    if (ImGui.MenuItem("Debug"))
                    {
                        OnDebug?.Invoke();
                    }

                    ImGui.Separator();

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
