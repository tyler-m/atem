using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atem.Core;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Graphics;

namespace Atem.Views.MonoGame.Graphics
{
    public class Screen : IScreen
    {
        private const float AspectRatio = (float)ScreenManager.ScreenWidth / ScreenManager.ScreenHeight;

        private readonly IWindow _window;
        private int _displayOffsetX, _displayOffsetY;
        private int _displayWidth, _displayHeight;
        private bool _sizeLocked;
        private int _sizeFactor;
        private Texture2D _texture;
        private Color[] _screenData;
        private SpriteBatch _spriteBatch;

        public Texture2D Texture { get => _texture; }
        public bool SizeLocked
        {
            get => _sizeLocked;
            set
            {
                _sizeLocked = value;
                OnScreenSizeChange?.Invoke();
            }
        }
        public int SizeFactor
        {
            get => _sizeFactor;
            set
            {
                _sizeFactor = value;
                OnScreenSizeChange?.Invoke();
            }
        }

        public delegate void OnScreenTextureCreatedEvent(Texture2D texture);
        public event OnScreenTextureCreatedEvent OnScreenTextureCreated;

        public delegate void OnScreenSizeChangeEvent();
        public event OnScreenSizeChangeEvent OnScreenSizeChange;

        public Screen(AtemRunner atem, IWindow window)
        {
            atem.OnVerticalBlank += OnVerticalBlank;
            _window = window;
            _window.WindowSizeChanged += OnWindowSizeChanged;
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            int windowRenderHeight = _window.Height - 19;
            int scaledHeight = (int)(_window.Width / AspectRatio);

            if (scaledHeight <= windowRenderHeight)
            {
                // scale by width
                _displayWidth = _window.Width;
                _displayHeight = scaledHeight;

                _displayOffsetX = 0;
                _displayOffsetY = (int)((windowRenderHeight - scaledHeight) / 2.0f);
            }
            else
            {
                // scale by height
                int scaledWidth = (int)(_window.Height * AspectRatio);
                _displayWidth = scaledWidth;
                _displayHeight = windowRenderHeight;

                _displayOffsetX = (int)((_window.Width - scaledWidth) / 2.0f);
                _displayOffsetY = 0;
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, ScreenManager.ScreenWidth, ScreenManager.ScreenHeight);
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _screenData = new Color[ScreenManager.ScreenWidth * ScreenManager.ScreenHeight];
            OnScreenTextureCreated?.Invoke(_texture);
        }

        public void Draw()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (_sizeLocked)
            {
                _spriteBatch.Draw(_texture,
                    new Rectangle(0, 19, ScreenManager.ScreenWidth * SizeFactor, ScreenManager.ScreenHeight * SizeFactor),
                    new Rectangle(0, 0, _texture.Width, _texture.Height), Color.White);
            }
            else
            {
                _spriteBatch.Draw(_texture,
                    new Rectangle(_displayOffsetX, _displayOffsetY + 19, _displayWidth, _displayHeight),
                    new Rectangle(0, 0, _texture.Width, _texture.Height), Color.White);
            }

            _spriteBatch.End();
        }

        public void OnVerticalBlank(GBColor[] screen)
        {
            for (int i = 0; i < _screenData.Length; i++)
            {
                _screenData[i] = GBColorToColor(screen[i]);
            }

            _texture.SetData(_screenData);
        }

        private static Color GBColorToColor(GBColor gbColor)
        {
            float r = gbColor.Red / 31.0f;
            float g = gbColor.Green / 31.0f;
            float b = gbColor.Blue / 31.0f;
            return new Color(r, g, b);
        }
    }
}
