using Atem.Core.State;
using System.IO;

namespace Atem.Core.Graphics
{
    public class ScreenManager : IStateful
    {
        private readonly IBus _bus;
        private GBColor[] _screen = new GBColor[160 * 144];

        public GBColor[] Screen { get => _screen; set => _screen = value; }

        public ScreenManager(IBus bus)
        {
            _bus = bus;
        }

        private GBColor GetColorOfScreenPixel(byte pixelX, byte pixelY)
        {
            bool window = _bus.Graphics.WindowEnabled && pixelX > _bus.Graphics.WindowX - 8 && _bus.Graphics.WindowY <= pixelY;
            _bus.Graphics.WindowWasTriggeredThisFrame |= window;

            (GBColor tileColor, int tileId, bool tilePriority) = _bus.Graphics.TileManager.GetTileInfo(pixelX, window ? _bus.Graphics.CurrentWindowLine : pixelY, window);

            if (!_bus.ColorMode && !_bus.Graphics.BackgroundAndWindowEnabledOrPriority)
            {
                tileColor = new GBColor(0xFFFF);
            }

            (GBColor spriteColor, int spriteId, Sprite sprite) = _bus.Graphics.ObjectManager.GetSpritePixelInfo(pixelX, pixelY, _bus.Graphics.DMGPalettes);

            GBColor pixelColor = tileColor;
            if (sprite != null && spriteId != 0 && _bus.Graphics.ObjectManager.ObjectsEnabled)
            {
                if (!sprite.Priority || tileId == 0)
                {
                    pixelColor = spriteColor;
                }
            }

            if (_bus.ColorMode)
            {
                if (tilePriority && tileId != 0 && _bus.Graphics.BackgroundAndWindowEnabledOrPriority)
                {
                    pixelColor = tileColor;
                }
            }

            return pixelColor;
        }

        public void Clock()
        {
            for (int i = 0; i < 4; i++)
            {
                _screen[_bus.Graphics.CurrentLine * 160 + _bus.Graphics.LinePixel] = GetColorOfScreenPixel(_bus.Graphics.LinePixel, _bus.Graphics.CurrentLine);
                _bus.Graphics.LinePixel++;
            }
        }

        public void GetState(BinaryWriter writer)
        {
            foreach (GBColor pixel in _screen)
            {
                pixel.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            foreach (GBColor pixel in _screen)
            {
                pixel.SetState(reader);
            }
        }
    }
}
