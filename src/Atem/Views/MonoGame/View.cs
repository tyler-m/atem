using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.UI;

namespace Atem.Views.MonoGame
{
    public class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;

        private readonly GraphicsDeviceManager _graphics;

        private readonly AtemRunner _atem;
        private readonly ISoundService _soundService;
        private readonly IShutdownService _shutdownService;
        private readonly Screen _screen;
        private readonly InputManager _inputManager;
        private readonly ViewUIManager _viewUIManager;

        public AtemRunner Atem { get => _atem; }
        public InputManager InputManager { get => _inputManager; }
        public Screen Screen { get => _screen; }

        public delegate void ViewInitializeEvent();
        public event ViewInitializeEvent OnInitialize;

        public View(ViewUIManager viewUIManager, AtemRunner atem, Screen screen, ISoundService soundService, InputManager inputManager, IShutdownService shutdownService)
        {
            _graphics = new GraphicsDeviceManager(this);

            _viewUIManager = viewUIManager;
            _atem = atem;
            _soundService = soundService;
            _shutdownService = shutdownService;
            _inputManager = inputManager;

            _screen = screen;
            _screen.OnScreenSizeChange += UpdateWindowSize;

            Window.AllowUserResizing = false;

            _atem.Paused = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            Exiting += OnExit;

            _viewUIManager.OnUpdateWindowSize += UpdateWindowSize;
            _viewUIManager.OnExitRequest += Exit;

            UpdateWindowSize();
        }

        public void UpdateWindowSize()
        {
            if (_viewUIManager != null)
            {
                if (_viewUIManager.Debug)
                {
                    _graphics.PreferredBackBufferWidth = 960;
                    _graphics.PreferredBackBufferHeight = 720;
                    Window.AllowUserResizing = true;
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = (int)(_screen.Width * _screen.SizeFactor);
                    _graphics.PreferredBackBufferHeight = (int)(_screen.Height * _screen.SizeFactor) + _viewUIManager.GetMenuBarHeight();
                    Window.AllowUserResizing = false;
                }

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
            _atem.Update();

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
