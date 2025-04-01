﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ImGuiNET;
using Atem.Core.Graphics;
using Atem.Core;
using Atem.Views.MonoGame.UI.Window;
using Atem.Views.MonoGame.UI;
using Atem.Views.MonoGame.Input;

namespace Atem.Views.MonoGame
{
    public class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly AtemRunner _atem;
        private readonly Color[] _screenData;
        private Texture2D _screenTexture;
        private DynamicSoundEffectInstance _soundInstance;

        private readonly Config _config;
        private string _loadedFilePath;

        private bool _debug = false;
        private ImGuiRenderer _imGui;
        private readonly FileExplorerWindow _fileExplorerWindow;
        private GameDisplayWindow _gameDisplayWindow;
        private readonly MemoryWindow _memoryWindow;
        private readonly BreakpointWindow _breakpointWindow;
        private readonly ProcessorRegistersWindow _processorRegistersWindow;
        private readonly MenuBar _menuBar;
        private readonly InputManager _inputManager;

        public AtemRunner Atem { get => _atem; }

        public View(AtemRunner atem, Config config)
        {
            Window.AllowUserResizing = false;

            _atem = atem;
            _atem.Paused = true;
            _atem.OnVerticalBlank += OnVerticalBlank;
            _atem.OnFullAudioBuffer += OnFullAudioBuffer;
            _config = config;
            _screenData = new Color[config.ScreenWidth * _config.ScreenHeight];

            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            Exiting += OnExit;

            _fileExplorerWindow = new FileExplorerWindow(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _fileExplorerWindow.OnSelectFile += LoadFile;
            _memoryWindow = new MemoryWindow(_atem);
            _menuBar = new MenuBar();
            _menuBar.OnExit += Exit;
            _menuBar.OnDebug += ToggleDebug;
            _menuBar.OnLoadState += LoadStateData;
            _menuBar.OnSaveState += SaveStateData;
            _menuBar.OnOpen += OnOpen;
            _breakpointWindow = new BreakpointWindow(_atem.Debugger);
            _processorRegistersWindow = new ProcessorRegistersWindow(_atem.Bus.Processor);
            _inputManager = new InputManager();

            UpdateWindowSize();
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

        private void UpdateWindowSize()
        {
            if (_debug)
            {
                _graphics.PreferredBackBufferWidth = 960;
                _graphics.PreferredBackBufferHeight = 720;
                Window.AllowUserResizing = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = (int)(_config.ScreenWidth * _config.ScreenSizeFactor);
                _graphics.PreferredBackBufferHeight = (int)(_config.ScreenHeight * _config.ScreenSizeFactor) + _menuBar.Height;
                Window.AllowUserResizing = false;
            }

            _fileExplorerWindow.Width = _graphics.PreferredBackBufferWidth;
            _fileExplorerWindow.Height = _graphics.PreferredBackBufferHeight;
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
        }

        private void OnFullAudioBuffer(byte[] buffer)
        {
            if (_soundInstance.PendingBufferCount > 10)
            {
                return;
            }
            else
            {
                _soundInstance.SubmitBuffer(buffer);
            }
        }

        protected override void Initialize()
        {
            _imGui = new ImGuiRenderer(this);

            base.Initialize();

            _soundInstance = new DynamicSoundEffectInstance(Core.Audio.AudioManager.SAMPLE_RATE, AudioChannels.Stereo);
            _soundInstance.Play();

            Texture2D highlightTexture = new(GraphicsDevice, 1, 1);
            highlightTexture.SetData([Color.White]);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenTexture = new Texture2D(GraphicsDevice, _config.ScreenWidth, _config.ScreenHeight);
            nint screenTextureId = _imGui.BindTexture(_screenTexture);
            _gameDisplayWindow = new GameDisplayWindow(screenTextureId, (int)(_config.ScreenWidth * _config.ScreenSizeFactor), (int)(_config.ScreenHeight * _config.ScreenSizeFactor));
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(this);
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
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(_screenTexture,
                    new Rectangle(0, _menuBar.Height, (int)(_config.ScreenWidth * _config.ScreenSizeFactor), (int)(_config.ScreenHeight * _config.ScreenSizeFactor)),
                    new Rectangle(0, 0, _config.ScreenWidth, _config.ScreenHeight), Color.White);
                _spriteBatch.End();
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

            _imGui.EndDraw();

            base.Draw(gameTime);
        }

        private static Color GBColorToColor(GBColor gbColor)
        {
            float r = gbColor.Red / 31.0f;
            float g = gbColor.Green / 31.0f;
            float b = gbColor.Blue / 31.0f;
            return new Color(r, g, b);
        }

        private void OnVerticalBlank(GBColor[] screen)
        {
            for (int i = 0; i < _screenData.Length; i++)
            {
                _screenData[i] = GBColorToColor(screen[i]);
            }

            _screenTexture.SetData(_screenData);
        }
    }
}
