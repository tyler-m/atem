using System.IO;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Screen
{
    public class ScreenManager : IStateful
    {
        private readonly IBus _bus;
        private GBColor[] _screen = new GBColor[160 * 144];
        private byte _linePixel;

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
            if (_bus.Graphics.RenderModeScheduler.Mode == RenderMode.Draw)
            {
                for (int i = 0; i < 4; i++)
                {
                    _screen[_bus.Graphics.RenderModeScheduler.CurrentLine * 160 + _linePixel] = GetColorOfScreenPixel(_linePixel, _bus.Graphics.RenderModeScheduler.CurrentLine);
                    _linePixel++;
                }
            }
            else
            {
                _linePixel = 0;
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_linePixel);

            foreach (GBColor pixel in _screen)
            {
                pixel.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            _linePixel = reader.ReadByte();

            foreach (GBColor pixel in _screen)
            {
                pixel.SetState(reader);
            }
        }
    }
}
