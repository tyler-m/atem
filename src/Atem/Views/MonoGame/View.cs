﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Atem.Core;
using Atem.Core.Input;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.UI;
using Atem.Views.MonoGame.UI.Window;
using Atem.Views.MonoGame.Input.Command;
using Atem.Views.MonoGame.Audio;

namespace Atem.Views.MonoGame
{
    public class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;

        private readonly GraphicsDeviceManager _graphics;

        private readonly AtemRunner _atem;
        private readonly ISoundService _soundService;
        private readonly IViewConfigService _configService;
        private readonly Screen _screen;
        private readonly InputManager _inputManager;

        private string _loadedFilePath;

        private bool _debug = false;
        private ImGuiRenderer _imGui;
        private readonly FileExplorerWindow _fileExplorerWindow;
        private GameDisplayWindow _gameDisplayWindow;
        private readonly MemoryWindow _memoryWindow;
        private readonly BreakpointWindow _breakpointWindow;
        private readonly ProcessorRegistersWindow _processorRegistersWindow;
        private readonly MenuBar _menuBar;
        private readonly OptionsWindow _optionsWindow;

        public AtemRunner Atem { get => _atem; }
        public InputManager InputManager { get => _inputManager; }
        public Screen Screen { get => _screen; }

        public View(AtemRunner atem, IViewConfigService configService, ISoundService soundService, InputManager inputManager)
        {
            _atem = atem;
            _soundService = soundService;
            _configService = configService;
            _inputManager = inputManager;

            AddInputCommands();

            _screen = new Screen(_atem);

            _configService.Load(this);

            Window.AllowUserResizing = false;

            _atem.Paused = true;

            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            Exiting += OnExit;

            _fileExplorerWindow = new FileExplorerWindow();
            _fileExplorerWindow.OnSelectFile += LoadFile;
            _memoryWindow = new MemoryWindow(this);
            _menuBar = new MenuBar();
            _menuBar.OnExit += Exit;
            _menuBar.OnDebug += ToggleDebug;
            _menuBar.OnLoadState += LoadStateData;
            _menuBar.OnSaveState += SaveStateData;
            _menuBar.OnOpen += OnOpen;
            _menuBar.OnOptions += OnOptions;
            _breakpointWindow = new BreakpointWindow(this);
            _processorRegistersWindow = new ProcessorRegistersWindow(this);
            _optionsWindow = new OptionsWindow(this);

            UpdateWindowSize();
        }

        private void AddInputCommands()
        {
            _inputManager.AddCommand(new ExitCommand(this));
            _inputManager.AddCommand(new ContinueCommand(this));
            _inputManager.AddCommand(new PauseCommand(this));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Up, CommandType.Up));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Down, CommandType.Down));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Left, CommandType.Left));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Right, CommandType.Right));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.B, CommandType.B));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.A, CommandType.A));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Select, CommandType.Select));
            _inputManager.AddCommand(new JoypadCommand(this, JoypadButton.Start, CommandType.Start));
        }

        private void OnOptions()
        {
            _optionsWindow.Active = true;
        }

        private void OnOpen()
        {
            _fileExplorerWindow.Active = true;
        }

        private void SaveStateData(int slot)
        {
            File.WriteAllBytes(_loadedFilePath + ".ss" + slot, _atem.GetState());
        }

        private void LoadStateData(int slot)
        {
            string stateSavePath = _loadedFilePath + ".ss" + slot;
            
            if (File.Exists(stateSavePath))
            {
                byte[] saveStateData = File.ReadAllBytes(stateSavePath);
                _atem.SetState(saveStateData);
            }
        }

        public void UpdateWindowSize()
        {
            if (_debug)
            {
                _graphics.PreferredBackBufferWidth = 960;
                _graphics.PreferredBackBufferHeight = 720;
                Window.AllowUserResizing = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = (int)(_screen.Width * _screen.SizeFactor);
                _graphics.PreferredBackBufferHeight = (int)(_screen.Height * _screen.SizeFactor) + _menuBar.Height;
                Window.AllowUserResizing = false;
            }

            _graphics.ApplyChanges();
        }

        private void ToggleDebug()
        {
            _debug = !_debug;
            _atem.Debugger.Active = _debug;
            UpdateWindowSize();
        }

        public void LoadFile(FileInfo fileInfo)
        {
            string filePath = fileInfo.FullName;

            if (_atem.Load(filePath))
            {
                string savePath = filePath + ".sav";

                if (File.Exists(savePath))
                {
                    byte[] saveData = File.ReadAllBytes(savePath);
                    _atem.Bus.Cartridge.LoadBatterySave(saveData);
                }

                _atem.Paused = false;
                _fileExplorerWindow.Active = false;
                _menuBar.EnableStates = true;
                _loadedFilePath = fileInfo.FullName;
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (_atem.Bus.Cartridge.Loaded)
            {
                File.WriteAllBytes(_loadedFilePath + ".sav", _atem.Bus.Cartridge.GetBatterySave());
            }

            _configService.Save(this);
        }

        protected override void Initialize()
        {
            _imGui = new ImGuiRenderer(this);

            base.Initialize();

            _soundService.Play();
        }

        protected override void LoadContent()
        {
            _screen.LoadContent(GraphicsDevice);
            nint screenTextureId = _imGui.BindTexture(_screen.Texture);
            _gameDisplayWindow = new GameDisplayWindow(this, screenTextureId, _screen.Width, _screen.Height);
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update();
            _atem.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _imGui.BeginDraw(gameTime);

            ImGui.DockSpaceOverViewport(ImGui.GetID("Root"), ImGui.GetWindowViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

            _menuBar.Draw();

            if (!_debug)
            {
                _screen.Draw();
            }
            else
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

            base.Draw(gameTime);
        }
    }
}
