using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Atem.Core.Input;
using System;

namespace Atem
{
    internal class View : Game
    {
        private const float ScreenRefreshRate = 59.73f;
        private const int ScreenWidth = 160;
        private const int ScreenHeight = 144;
        private const float ScreenSizeFactor = 2.0f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private Core.Atem _atem;
        private Color[] _screenData = new Color[ScreenWidth * ScreenHeight];
        private Texture2D _screenTexture;
        private bool _pauseAtem = true;
        private DynamicSoundEffectInstance _soundInstance;

        public View(Core.Atem atem)
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int)(ScreenWidth * ScreenSizeFactor);
            _graphics.PreferredBackBufferHeight = (int)(ScreenHeight * ScreenSizeFactor);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            _atem = atem;
            _atem.OnVerticalBlank += OnVerticalBlank;
            _atem.OnFullAudioBuffer += OnFullAudioBuffer;
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenTexture = new Texture2D(GraphicsDevice, ScreenWidth, ScreenHeight);
            _font = Content.Load<SpriteFont>("Default");

            _soundInstance = new DynamicSoundEffectInstance(Core.Audio.AudioManager.SAMPLE_RATE, AudioChannels.Stereo);
            _soundInstance.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.PageUp) && _previousKeyboardState.IsKeyUp(Keys.PageUp))
            {
                _atem.ClockOneOperation();
                _pauseAtem = true;
            }
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
            GraphicsDevice.Clear(new Color(33, 36, 39));

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_screenTexture,
                new Rectangle(0, 0, (int)(ScreenWidth * ScreenSizeFactor), (int)(ScreenHeight * ScreenSizeFactor)),
                new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void OnVerticalBlank(byte[] screen)
        {
            for (int i = 0; i < _screenData.Length; i++)
            {
                byte bColor = screen[i];
                Color color = Color.White;

                if (bColor == 1)
                {
                    color = Color.LightGray * 0.9f;
                }
                else if (bColor == 2)
                {
                    color = Color.DarkGray * 0.5f;
                }
                else if (bColor == 3)
                {
                    color = Color.Black;
                }

                _screenData[i] = color;
            }

            _screenTexture.SetData(_screenData);
        }
    }
}
