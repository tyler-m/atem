using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Atem.Core;
using Atem.Views.MonoGame.UI.Window;
using Atem.Input;
using Atem.Saving;
using Atem.Views.MonoGame.Graphics;

namespace Atem.Views.MonoGame.UI
{
    public class ViewUIManager
    {
        private readonly ImGuiRenderer _imGui;
        private readonly AtemRunner _atem;
        private readonly IBatterySaveService _batterySaveService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly Screen _screen;

        private GameDisplayWindow _gameDisplayWindow;
        private readonly FileBrowserWindow _fileBrowserWindow;
        private readonly MemoryWindow _memoryWindow;
        private readonly BreakpointWindow _breakpointWindow;
        private readonly ProcessorRegistersWindow _processorRegistersWindow;
        private readonly MenuBar _menuBar;
        private readonly OptionsWindow _optionsWindow;
        
        private bool _debug;

        public delegate void UpdateWindowSizeEvent();
        public event UpdateWindowSizeEvent OnUpdateWindowSize;

        public delegate void ExitRequestEvent();
        public event ExitRequestEvent OnExitRequest;

        public bool Debug { get => _debug; }

        public ViewUIManager(ImGuiRenderer imGui, AtemRunner atem, ISaveStateService saveStateService, IBatterySaveService batterySaveService, ICartridgeLoader cartridgeLoader, Screen screen, InputManager inputManager)
        {
            _imGui = imGui;
            _atem = atem;
            _batterySaveService = batterySaveService;
            _cartridgeLoader = cartridgeLoader;
            _screen = screen;

            _fileBrowserWindow = new FileBrowserWindow();
            _fileBrowserWindow.OnSelectFile += LoadFile;
            _memoryWindow = new MemoryWindow(_atem.Bus);
            _menuBar = new MenuBar(saveStateService, cartridgeLoader);
            _menuBar.OnExit += () => OnExitRequest?.Invoke();
            _menuBar.OnDebug += ToggleDebug;
            _menuBar.OnOpen += OnOpen;
            _menuBar.OnOptions += OnOptions;
            _breakpointWindow = new BreakpointWindow(_atem.Debugger);
            _processorRegistersWindow = new ProcessorRegistersWindow(_atem.Bus.Processor);
            _optionsWindow = new OptionsWindow(screen, _atem.Bus.Audio, inputManager);
            _screen.OnScreenTextureCreated += OnScreenTextureCreated;
        }

        private void OnScreenTextureCreated(Texture2D texture)
        {
            nint screenTextureId = _imGui.BindTexture(_screen.Texture);
            _gameDisplayWindow = new GameDisplayWindow(_screen, screenTextureId, _screen.Width, _screen.Height);
        }

        public int GetMenuBarHeight()
        {
            return _menuBar.Height;
        }

        private void LoadFile(FileInfo fileInfo)
        {
            _cartridgeLoader.Context.Id = fileInfo.FullName;

            if (_cartridgeLoader.Load())
            {
                _batterySaveService.Load(_cartridgeLoader.Context);
                _atem.Paused = false;
                _fileBrowserWindow.Active = false;
                _menuBar.EnableStates = true;
            }
        }

        private void OnOpen()
        {
            _fileBrowserWindow.Active = true;
        }

        private void OnOptions()
        {
            _optionsWindow.Active = true;
        }

        private void ToggleDebug()
        {
            _debug = !_debug;
            _atem.Debugger.Active = _debug;
            OnUpdateWindowSize?.Invoke();
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

            if (_fileBrowserWindow.Active)
            {
                _fileBrowserWindow.Draw();
            }

            if (_optionsWindow.Active)
            {
                _optionsWindow.Draw();
            }

            _imGui.EndDraw();
        }
    }
}
