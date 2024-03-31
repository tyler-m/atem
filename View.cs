using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private Atem _atem;
        private Color[] _screenData = new Color[ScreenWidth * ScreenHeight];
        private Texture2D _screenTexture;
        private bool _pauseAtem = true;

        public View(Atem atem)
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int)(ScreenWidth * ScreenSizeFactor);
            _graphics.PreferredBackBufferHeight = (int)(ScreenHeight * ScreenSizeFactor);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1/ScreenRefreshRate);

            _atem = atem;
            _atem.OnVerticalBlank += OnVerticalBlank;
            _atem.ClockCPUOneOp();
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
        }

        protected override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _atem.Clock();
            }
            else if (_currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
            {
                _pauseAtem = !_pauseAtem;
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
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
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
                    color = Color.LightGray;
                }
                else if (bColor == 2)
                {
                    color = Color.DarkGray;
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
