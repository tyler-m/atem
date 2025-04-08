using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Atem.Views.MonoGame.UI.Window;

namespace Atem.Views.MonoGame.UI
{
    public class ViewUIManager
    {
        private readonly View _view;
        private bool _debug;
        private ImGuiRenderer _imGui;

        private GameDisplayWindow _gameDisplayWindow;
        private readonly FileExplorerWindow _fileExplorerWindow;
        private readonly MemoryWindow _memoryWindow;
        private readonly BreakpointWindow _breakpointWindow;
        private readonly ProcessorRegistersWindow _processorRegistersWindow;
        private readonly MenuBar _menuBar;
        private readonly OptionsWindow _optionsWindow;

        public bool Debug { get { return _debug; } }

        public ViewUIManager(View view)
        {
            _view = view;

            _fileExplorerWindow = new FileExplorerWindow();
            _fileExplorerWindow.OnSelectFile += LoadFile;

            _memoryWindow = new MemoryWindow(_view);

            _menuBar = new MenuBar();
            _menuBar.OnExit += _view.Exit;
            _menuBar.OnDebug += ToggleDebug;
            _menuBar.OnLoadState += _view.LoadStateData;
            _menuBar.OnSaveState += _view.SaveStateData;
            _menuBar.OnOpen += OnOpen;
            _menuBar.OnOptions += OnOptions;

            _breakpointWindow = new BreakpointWindow(_view);
            _processorRegistersWindow = new ProcessorRegistersWindow(_view);
            _optionsWindow = new OptionsWindow(_view);

            _view.OnInitialize += OnViewInitialize;
            _view.Screen.OnScreenTextureCreated += OnScreenTextureCreated;
        }

        private void OnViewInitialize()
        {
            _imGui = new ImGuiRenderer(_view);
        }

        private void OnScreenTextureCreated(Texture2D texture)
        {
            nint screenTextureId = _imGui.BindTexture(_view.Screen.Texture);
            _gameDisplayWindow = new GameDisplayWindow(_view, screenTextureId, _view.Screen.Width, _view.Screen.Height);
        }

        public int GetMenuBarHeight()
        {
            return _menuBar.Height;
        }

        private void LoadFile(FileInfo fileInfo)
        {
            _view.LoadFile(fileInfo);

            if (_view.Atem.Bus.Cartridge.Loaded)
            {
                _fileExplorerWindow.Active = false;
                _menuBar.EnableStates = true;
            }
        }

        private void OnOpen()
        {
            _fileExplorerWindow.Active = true;
        }

        private void OnOptions()
        {
            _optionsWindow.Active = true;
        }

        private void ToggleDebug()
        {
            _debug = !_debug;
            _view.Atem.Debugger.Active = _debug;
            _view.UpdateWindowSize();
        }

        public void Draw(GameTime gameTime)
        {
            _imGui.BeginDraw(gameTime);

            ImGui.DockSpaceOverViewport(ImGui.GetID("Root"), ImGui.GetWindowViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

            _menuBar.Draw();

            if (_debug)
            {
                _memoryWindow.Draw();
                _gameDisplayWindow.Draw();
                _breakpointWindow.Draw();
                _processorRegistersWindow.Draw();
            }

            if (_fileExplorerWindow.Active)
            {
                _fileExplorerWindow.Draw();
            }

            if (_optionsWindow.Active)
            {
                _optionsWindow.Draw();
            }

            _imGui.EndDraw();
        }
    }
}
