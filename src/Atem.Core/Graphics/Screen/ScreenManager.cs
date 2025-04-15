using System.IO;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;

namespace Atem.Core.Graphics.Screen
{
    public class ScreenManager : IScreenManager
    {
        public const int ScreenWidth = 160;
        public const int ScreenHeight = 144;

        private readonly IBus _bus;
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly ITileManager _tileManager;
        private readonly IObjectManager _objectManager;
        private GBColor[] _screen = new GBColor[160 * 144];
        private byte _linePixel;
        private bool _windowWasTriggeredThisFrame;
        private byte _currentWindowLine;
        private bool _windowEnabled;
        private bool _backgroundAndWindowEnabledOrPriority;

        public bool WindowEnabled { get => _windowEnabled; set => _windowEnabled = value; }
        public bool BackgroundAndWindowEnabledOrPriority { get => _backgroundAndWindowEnabledOrPriority; set => _backgroundAndWindowEnabledOrPriority = value; }
        public GBColor[] Screen { get => _screen; set => _screen = value; }

        public ScreenManager(IBus bus, IRenderModeScheduler renderModeScheduler, ITileManager tileManager, IObjectManager objectManager)
        {
            _bus = bus;
            _renderModeScheduler = renderModeScheduler;
            _tileManager = tileManager;
            _objectManager = objectManager;
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
            bool window = _windowEnabled && pixelX > _tileManager.WindowX - 8 && _tileManager.WindowY <= pixelY;
            _windowWasTriggeredThisFrame |= window;

            (GBColor tileColor, int tileId, bool tilePriority) = _tileManager.GetTileInfo(pixelX, window ? _currentWindowLine : pixelY, window);

            if (!_bus.ColorMode && !_backgroundAndWindowEnabledOrPriority)
            {
                tileColor = new GBColor(0xFFFF);
            }

            (GBColor spriteColor, int spriteId, Sprite sprite) = _objectManager.GetSpritePixelInfo(pixelX, pixelY);

            GBColor pixelColor = tileColor;
            if (sprite != null && spriteId != 0 && _objectManager.ObjectsEnabled)
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
            if (_renderModeScheduler.Mode == RenderMode.Draw)
            {
                for (int i = 0; i < 4; i++)
                {
                    int index = _renderModeScheduler.CurrentLine * 160 + _linePixel;
                    _screen[index] = GetColorOfScreenPixel(_linePixel, _renderModeScheduler.CurrentLine);
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

            foreach (GBColor pixel in _screen)
            {
                pixel.SetState(reader);
            }
        }
    }
}
