using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atem.Core;
using Atem.Views.MonoGame.Audio;
using Atem.Views.MonoGame.Config;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.UI;
using Atem.Views.MonoGame.Saving;

namespace Atem.Views.MonoGame
{
    public class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;

        private readonly GraphicsDeviceManager _graphics;

        private readonly AtemRunner _atem;
        private readonly ISoundService _soundService;
        private readonly IViewConfigService _configService;
        private readonly ISaveStateService _saveStateService;
        private readonly ICartridgeLoader _cartridgeLoader;
        private readonly IBatterySaveService _batterySaveService;
        private readonly Screen _screen;
        private readonly InputManager _inputManager;
        private readonly ViewUIManager _viewUIManager;

        public AtemRunner Atem { get => _atem; }
        public InputManager InputManager { get => _inputManager; }
        public Screen Screen { get => _screen; }
        public ICartridgeLoader CartridgeLoader { get => _cartridgeLoader; }
        public ISaveStateService SaveStateService { get => _saveStateService; }
        public IBatterySaveService BatterySaveService { get => _batterySaveService; }

        public delegate void OnInitializeEvent();
        public event OnInitializeEvent OnInitialize;

        public View(AtemRunner atem, IViewConfigService configService, ISoundService soundService, ISaveStateService saveStateService, ICartridgeLoader cartridgeLoader, IBatterySaveService batterySaveService, InputManager inputManager)
        {
            _atem = atem;
            _soundService = soundService;
            _configService = configService;
            _saveStateService = saveStateService;
            _cartridgeLoader = cartridgeLoader;
            _batterySaveService = batterySaveService;
            _inputManager = inputManager;

            _screen = new Screen(_atem);

            _configService.Load(this);

            Window.AllowUserResizing = false;

            _atem.Paused = true;

            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            Exiting += OnExit;

            _viewUIManager = new ViewUIManager(this);

            UpdateWindowSize();
        }

        public void UpdateWindowSize()
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

        private void OnExit(object sender, EventArgs e)
        {
            if (_atem.Bus.Cartridge.Loaded)
            {
                _batterySaveService.Save(_cartridgeLoader.Context);
            }

            _configService.Save(this);
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
