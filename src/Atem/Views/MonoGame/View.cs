using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atem.Audio;
using Atem.Core;
using Atem.Core.Graphics.Screen;
using Atem.Graphics;
using Atem.Input;
using Atem.Shutdown;
using Atem.Views.MonoGame.UI;
using Atem.Views.MonoGame.Graphics;

namespace Atem.Views.MonoGame
{
    public class View : Game, IAtemView
    {
        private readonly GraphicsDeviceManager _graphics;

        private readonly Emulator _emulator;
        private readonly ISoundService _soundService;
        private readonly IShutdownService _shutdownService;
        private readonly Screen _screen;
        private readonly Window _window;
        private readonly InputManager _inputManager;
        private readonly ViewUIManager _viewUIManager;

        public delegate void ViewInitializeEvent();
        public event ViewInitializeEvent OnInitialize;

        public View(ViewUIManager viewUIManager, Emulator emulator, Screen screen, Window window, ISoundService soundService, InputManager inputManager, IShutdownService shutdownService)
        {
            _graphics = new GraphicsDeviceManager(this);

            _viewUIManager = viewUIManager;
            _emulator = emulator;
            _soundService = soundService;
            _shutdownService = shutdownService;
            _inputManager = inputManager;

            _window = window;
            _screen = screen;
            _screen.OnScreenSizeChange += UpdateWindowSize;

            Window.AllowUserResizing = true;

            _emulator.Paused = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1 / Emulator.ScreenRefreshRate);

            Exiting += OnExit;

            _viewUIManager.OnUpdateWindowSize += UpdateWindowSize;
            _viewUIManager.OnExitRequest += Exit;

            Window.ClientSizeChanged += Window_ClientSizeChanged;

            InactiveSleepTime = TimeSpan.Zero;

            UpdateWindowSize();
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            _window.SetSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        public void UpdateWindowSize()
        {
            if (_viewUIManager == null)
            {
                return;
            }

            if (_screen.SizeLocked && !_viewUIManager.Debug)
            {
                Window.AllowUserResizing = false;
                _graphics.PreferredBackBufferWidth = ScreenManager.ScreenWidth * _screen.SizeFactor;
                _graphics.PreferredBackBufferHeight = (ScreenManager.ScreenHeight * _screen.SizeFactor) + _viewUIManager.GetMenuBarHeight();
                _graphics.ApplyChanges();
            }
            else
            {
                Window.AllowUserResizing = true;
                _graphics.PreferredBackBufferWidth = _window.Width;
                _graphics.PreferredBackBufferHeight = _window.Height;
                _graphics.ApplyChanges();
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            _shutdownService.Shutdown();
        }

        protected override void Initialize()
        {
            OnInitialize?.Invoke();
            _soundService.Play();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _screen.LoadContent(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update();
            _emulator.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (!_viewUIManager.Debug)
            {
                _screen.Draw();
            }

            _viewUIManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
