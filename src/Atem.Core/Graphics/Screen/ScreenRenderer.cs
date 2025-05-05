using System.IO;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Memory;

namespace Atem.Core.Graphics.Screen
{
    public class ScreenRenderer : IScreenRenderer
    {
        public const int ScreenWidth = 160;
        public const int ScreenHeight = 144;

        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly ITileManager _tileManager;
        private readonly IObjectManager _objectManager;
        private readonly Cartridge _cartridge;
        private byte _linePixel;
        private bool _windowWasTriggeredThisFrame;
        private byte _currentWindowLine;

        public bool WindowEnabled { get; set; }
        public bool BackgroundAndWindowEnabledOrPriority { get; set; }
        public GBColor[] Screen { get; private set; } = new GBColor[ScreenWidth * ScreenHeight];

        public ScreenRenderer(IRenderModeScheduler renderModeScheduler, ITileManager tileManager, IObjectManager objectManager, Cartridge cartridge)
        {
            _renderModeScheduler = renderModeScheduler;
            _tileManager = tileManager;
            _objectManager = objectManager;
            _renderModeScheduler.RenderModeChanged += RenderModeChanged;
            _cartridge = cartridge;

            for (int i = 0; i < Screen.Length; i++)
            {
                Screen[i] = new GBColor(0, 0, 0);
            }
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
            (GBColor tileColor, int tileId, bool tilePriority) = _tileManager.GetTileInfoAtScreenPixel(pixelX, _windowWasTriggeredThisFrame ? _currentWindowLine : pixelY, _windowWasTriggeredThisFrame);

            if (!_cartridge.SupportsColor && !BackgroundAndWindowEnabledOrPriority)
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

            if (_cartridge.SupportsColor)
            {
                if (tilePriority && tileId != 0 && BackgroundAndWindowEnabledOrPriority)
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
                    byte pixelX = _linePixel;
                    byte pixelY = _renderModeScheduler.CurrentLine;

                    bool window = WindowEnabled && pixelX > _tileManager.WindowX - 8 && _tileManager.WindowY <= pixelY;
                    _windowWasTriggeredThisFrame |= window;

                    int pixelIndex = pixelY * ScreenWidth + pixelX;
                    Screen[pixelIndex] = GetColorOfScreenPixel(pixelX, pixelY);
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
            writer.Write(WindowEnabled);
            writer.Write(BackgroundAndWindowEnabledOrPriority);

            foreach (GBColor pixel in Screen)
            {
                pixel.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            _linePixel = reader.ReadByte();
            _windowWasTriggeredThisFrame = reader.ReadBoolean();
            _currentWindowLine = reader.ReadByte();
            WindowEnabled = reader.ReadBoolean();
            BackgroundAndWindowEnabledOrPriority = reader.ReadBoolean();

            foreach (GBColor pixel in Screen)
            {
                pixel.SetState(reader);
            }
        }
    }
}
