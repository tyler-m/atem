using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atem.Core;
using Atem.Core.Graphics.Palettes;
using Atem.Graphics;

namespace Atem.Views.MonoGame.Graphics
{
    public class Screen : IScreen
    {
        private int _width, _height;
        private float _sizeFactor;
        private Texture2D _texture;
        private Color[] _screenData;
        private SpriteBatch _spriteBatch;

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Texture2D Texture { get => _texture; }
        public float SizeFactor
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

        public Screen(AtemRunner atem)
        {
            atem.OnVerticalBlank += OnVerticalBlank;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, _width, _height);
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _screenData = new Color[_width * _height];
            OnScreenTextureCreated?.Invoke(_texture);
        }

        public void Draw()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_texture,
                new Rectangle(0, 19, (int)(_width * _sizeFactor), (int)(_height * _sizeFactor)),
                new Rectangle(0, 0, _width, _height), Color.White);
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
