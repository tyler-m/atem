using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core;
using System;
using ImGuiNET;

namespace Atem.Views.MonoGame
{
    internal class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private AtemRunner _atem;
        private Color[] _screenData;
        private Texture2D _screenTexture;
        private bool _pauseAtem = true;
        private DynamicSoundEffectInstance _soundInstance;

        private Config _config;
        private ImGuiRenderer _imGui;
        private FileExplorer _fileExplorer;

        public View(AtemRunner atem, Config config)
        {
            _atem = atem;
            _atem.OnVerticalBlank += OnVerticalBlank;
            _atem.OnFullAudioBuffer += OnFullAudioBuffer;
            _config = config;
            _screenData = new Color[config.ScreenWidth * _config.ScreenHeight];

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int)(_config.ScreenWidth * _config.ScreenSizeFactor);
            _graphics.PreferredBackBufferHeight = (int)(_config.ScreenHeight * _config.ScreenSizeFactor);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            Exiting += OnExit;

            _fileExplorer = new FileExplorer(this, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }

        public void LoadFile(string filePath)
        {
            _atem.Load(filePath);
            _pauseAtem = false;
            _fileExplorer.Active = false;
        }

        private void OnExit(object sender, EventArgs e)
        {
            _atem.OnExit();
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

            Texture2D highlightTexture = new Texture2D(GraphicsDevice, 1, 1);
            highlightTexture.SetData(new[] { Color.White });
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenTexture = new Texture2D(GraphicsDevice, _config.ScreenWidth, _config.ScreenHeight);
            _font = Content.Load<SpriteFont>("Default");
        }

        protected override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
            {
                _pauseAtem = !_pauseAtem;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _atem.OnJoypadChange(JoypadButton.Up, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Up) && _previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _atem.OnJoypadChange(JoypadButton.Up, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _atem.OnJoypadChange(JoypadButton.Down, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Down) && _previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _atem.OnJoypadChange(JoypadButton.Down, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _atem.OnJoypadChange(JoypadButton.Left, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Left) && _previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _atem.OnJoypadChange(JoypadButton.Left, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _atem.OnJoypadChange(JoypadButton.Right, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Right) && _previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _atem.OnJoypadChange(JoypadButton.Right, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.X) && !_previousKeyboardState.IsKeyDown(Keys.X))
            {
                _atem.OnJoypadChange(JoypadButton.A, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.X) && _previousKeyboardState.IsKeyDown(Keys.X))
            {
                _atem.OnJoypadChange(JoypadButton.A, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Z) && !_previousKeyboardState.IsKeyDown(Keys.Z))
            {
                _atem.OnJoypadChange(JoypadButton.B, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Z) && _previousKeyboardState.IsKeyDown(Keys.Z))
            {
                _atem.OnJoypadChange(JoypadButton.B, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Back) && !_previousKeyboardState.IsKeyDown(Keys.Back))
            {
                _atem.OnJoypadChange(JoypadButton.Select, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Back) && _previousKeyboardState.IsKeyDown(Keys.Back))
            {
                _atem.OnJoypadChange(JoypadButton.Select, false);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                _atem.OnJoypadChange(JoypadButton.Start, true);
            }
            if (!_currentKeyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                _atem.OnJoypadChange(JoypadButton.Start, false);
            }

            if (!_pauseAtem)
            {
                _atem.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_screenTexture,
                new Rectangle(0, 0, (int)(_config.ScreenWidth * _config.ScreenSizeFactor), (int)(_config.ScreenHeight * _config.ScreenSizeFactor)),
                new Rectangle(0, 0, _config.ScreenWidth, _config.ScreenHeight), Color.White);

            _spriteBatch.End();

            _imGui.BeginDraw(gameTime);

            if (_fileExplorer.Active)
            {
                _fileExplorer.Draw();
            }

            _imGui.EndDraw();

            base.Draw(gameTime);
        }

        private Color GBColorToColor(GBColor gbColor)
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
