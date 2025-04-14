using System.IO;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Timing;

namespace Atem.Core.Graphics.Screen
{
    public class ScreenManager : IScreenManager
    {
        private readonly IBus _bus;
        private readonly RenderModeScheduler _renderModeScheduler;
        private GBColor[] _screen = new GBColor[160 * 144];
        private byte _linePixel;
        private bool _windowWasTriggeredThisFrame;
        private byte _currentWindowLine;
        private bool _windowEnabled;
        private bool _backgroundAndWindowEnabledOrPriority;
        private int _screenX;
        private int _screenY;
        private int _windowX;
        private int _windowY;

        public bool WindowEnabled { get => _windowEnabled; set => _windowEnabled = value; }
        public bool BackgroundAndWindowEnabledOrPriority { get => _backgroundAndWindowEnabledOrPriority; set => _backgroundAndWindowEnabledOrPriority = value; }
        public int ScreenX { get => _screenX; set => _screenX = value; }
        public int ScreenY { get => _screenY; set => _screenY = value; }
        public int WindowX { get => _windowX; set => _windowX = value; }
        public int WindowY { get => _windowY; set => _windowY = value; }
        public GBColor[] Screen { get => _screen; set => _screen = value; }

        public ScreenManager(IBus bus, RenderModeScheduler renderModeScheduler)
        {
            _bus = bus;
            _renderModeScheduler = renderModeScheduler;
            _renderModeScheduler.RenderModeChanged += RenderModeChanged;
        }

        private void RenderModeChanged(object sender, RenderModeChangedEventArgs e)
        {
            if (e.CurrentMode == RenderMode.HorizontalBlank && e.PreviousMode == RenderMode.Draw)
            {
                if (_windowWasTriggeredThisFrame)
                {
                    _currentWindowLine++;
                }

                _windowWasTriggeredThisFrame = false;
            }
            else if (e.CurrentMode == RenderMode.OAM && e.PreviousMode == RenderMode.VerticalBlank)
            {
                _currentWindowLine = 0;
            }
        }

        private GBColor GetColorOfScreenPixel(byte pixelX, byte pixelY)
        {
            bool window = _windowEnabled && pixelX > _windowX - 8 && _windowY <= pixelY;
            _windowWasTriggeredThisFrame |= window;

            (GBColor tileColor, int tileId, bool tilePriority) = _bus.Graphics.TileManager.GetTileInfo(pixelX, window ? _currentWindowLine : pixelY, window);

            if (!_bus.ColorMode && !_backgroundAndWindowEnabledOrPriority)
            {
                tileColor = new GBColor(0xFFFF);
            }

            (GBColor spriteColor, int spriteId, Sprite sprite) = _bus.Graphics.ObjectManager.GetSpritePixelInfo(pixelX, pixelY);

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
                if (tilePriority && tileId != 0 && _backgroundAndWindowEnabledOrPriority)
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
                    int index = _bus.Graphics.RenderModeScheduler.CurrentLine * 160 + _linePixel;
                    _screen[index] = GetColorOfScreenPixel(_linePixel, _bus.Graphics.RenderModeScheduler.CurrentLine);
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
            writer.Write(_windowWasTriggeredThisFrame);
            writer.Write(_currentWindowLine);
            writer.Write(_windowEnabled);
            writer.Write(_backgroundAndWindowEnabledOrPriority);
            writer.Write(_windowX);
            writer.Write(_windowY);
            writer.Write(_screenX);
            writer.Write(_screenY);

            foreach (GBColor pixel in _screen)
            {
                pixel.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            _linePixel = reader.ReadByte();
            _windowWasTriggeredThisFrame = reader.ReadBoolean();
            _currentWindowLine = reader.ReadByte();
            _windowEnabled = reader.ReadBoolean();
            _backgroundAndWindowEnabledOrPriority = reader.ReadBoolean();
            _windowX = reader.ReadInt32();
            _windowY = reader.ReadInt32();
            _screenX = reader.ReadInt32();
            _screenY = reader.ReadInt32();

            foreach (GBColor pixel in _screen)
            {
                pixel.SetState(reader);
            }
        }
    }
}
