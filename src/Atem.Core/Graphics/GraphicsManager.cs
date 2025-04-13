using System.IO;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core.Graphics
{
    public delegate void VerticalBlankEvent(GBColor[] screen);

    public class GraphicsManager : IStateful
    {
        private readonly ObjectManager _objectManager;
        public ObjectManager ObjectManager { get => _objectManager; }

        private readonly TileManager _tileManager;
        public TileManager TileManager { get => _tileManager; }

        public const float FRAME_RATE = 59.73f;

        private readonly IBus _bus;
        private readonly HDMA _hdma;
        private int _lineDotCount;
        private byte _linePixel;
        private readonly GBColor[] _screen = new GBColor[160 * 144];
        private bool _windowWasTriggeredThisFrame;
        private bool _justEnteredHorizontalBlank;
        private bool _currentlyOnLineY;
        RenderMode _mode;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;
        public bool Enabled;
        public bool WindowEnabled;
        public bool BackgroundAndWindowEnabledOrPriority;
        public bool InterruptOnLineY;
        public bool InterruptOnOAM;
        public bool InterruptOnVerticalBlank;
        public bool InterruptOnHorizontalBlank;
        public int ScreenX;
        public int ScreenY;
        public int WindowX;
        public int WindowY;
        public byte CurrentLine;
        public byte CurrentWindowLine;
        public int LineYToCompare;
        public PaletteGroup DMGPalettes = new();

        public bool CurrentlyOnLineY
        {
            get
            {
                return _currentlyOnLineY;
            }
            set
            {
                if (InterruptOnLineY && value && !_currentlyOnLineY)
                {
                    _bus.RequestInterrupt(InterruptType.STAT);
                }

                _currentlyOnLineY = value;
            }
        }

        public HDMA HDMA { get => _hdma; }


        public RenderMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                RenderMode prevMode = Mode;

                if (value == RenderMode.OAM)
                {
                    _objectManager.ResetScanline();

                    if (prevMode != RenderMode.OAM && InterruptOnOAM)
                    {
                        _bus.RequestInterrupt(InterruptType.STAT);
                    }
                }
                else if (value == RenderMode.VerticalBlank)
                {
                    OnVerticalBlank?.Invoke(_screen);
                    _bus.RequestInterrupt(InterruptType.VerticalBlank);
                    
                    if (prevMode != RenderMode.VerticalBlank && InterruptOnVerticalBlank)
                    {
                        _bus.RequestInterrupt(InterruptType.STAT);
                    }
                }
                else if (value == RenderMode.HorizontalBlank && prevMode != RenderMode.HorizontalBlank && InterruptOnHorizontalBlank)
                {
                    _bus.RequestInterrupt(InterruptType.STAT);
                }

                _mode = value;
            }
        }

        public GraphicsManager(IBus bus)
        {
            _bus = bus;
            _hdma = new(bus);
            _objectManager = new ObjectManager(bus);
            _tileManager = new TileManager(bus);
            Registers = new GraphicsRegisters(this);
            Mode = RenderMode.OAM;
        }

        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode = false)
        {
            _tileManager.WriteVRAM(address, value, ignoreRenderMode);
        }

        public byte ReadVRAM(ushort address)
        {
            return _tileManager.ReadVRAM(address);
        }

        public void WriteOAM(ushort address, byte value)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return;
            }

            _objectManager.WriteOAM(address, value);
        }

        public byte ReadOAM(ushort address)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            return _objectManager.ReadOAM(address);
        }

        private GBColor GetColorOfScreenPixel(byte pixelX, byte pixelY)
        {
            bool window = WindowEnabled && pixelX > WindowX - 8 && WindowY <= pixelY;
            _windowWasTriggeredThisFrame |= window;

            (GBColor tileColor, int tileId, bool tilePriority) = _tileManager.GetTileInfo(pixelX, window ? CurrentWindowLine : pixelY, window);

            if (!_bus.ColorMode && !BackgroundAndWindowEnabledOrPriority)
            {
                tileColor = new GBColor(0xFFFF);
            }

            (GBColor spriteColor, int spriteId, Sprite sprite) = _objectManager.GetSpritePixelInfo(pixelX, pixelY, DMGPalettes);

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
                if (tilePriority && tileId != 0 && BackgroundAndWindowEnabledOrPriority)
                {
                    pixelColor = tileColor;
                }
            }

            return pixelColor;
        }

        public void Clock()
        {
            if (!Enabled)
            {
                return;
            }

            _hdma.ClockTransfer();

            if (Mode == RenderMode.OAM)
            {
                _objectManager.CollectObjectsForScanline(CurrentLine);
            }
            else if (Mode == RenderMode.Draw)
            {
                for (int i = 0; i < 4; i++)
                {
                    _screen[CurrentLine * 160 + _linePixel] = GetColorOfScreenPixel(_linePixel, CurrentLine);
                    _linePixel++;
                }
            }

            _lineDotCount += 4;

            UpdateMode();

            CurrentlyOnLineY = LineYToCompare == CurrentLine;
        }

        private void UpdateMode()
        {
            if (Mode == RenderMode.HorizontalBlank)
            {
                if (_lineDotCount >= 456) 
                {
                    CurrentLine++;
                    _lineDotCount = 0;

                    if (CurrentLine >= 144)
                    {
                        Mode = RenderMode.VerticalBlank;
                    }
                    else
                    {
                        Mode = RenderMode.OAM;
                    }
                }
            }
            else if (Mode == RenderMode.VerticalBlank)
            {
                if (_lineDotCount >= 456)
                {
                    CurrentLine++;
                    _lineDotCount = 0;
                }

                if (CurrentLine > 153)
                {
                    CurrentLine = 0;
                    _lineDotCount = 0;
                    CurrentWindowLine = 0;
                    Mode = RenderMode.OAM;
                }
            }
            else if (Mode == RenderMode.OAM)
            {
                if (_lineDotCount >= 80)
                {
                    Mode = RenderMode.Draw;
                }
            }
            else if (Mode == RenderMode.Draw)
            {
                if (_linePixel >= 160)
                {
                    Mode = RenderMode.HorizontalBlank;
                    _linePixel = 0;
                    
                    if (_windowWasTriggeredThisFrame)
                    {
                        CurrentWindowLine++;
                    }

                    _windowWasTriggeredThisFrame = false;
                    _hdma.JustEnteredHorizontalBlank = true;
                }
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_lineDotCount);
            writer.Write(_linePixel);
            writer.Write(_tileManager.VRAM);

            foreach (GBColor pixel in _screen)
            {
                pixel.GetState(writer);
            }

            _objectManager.GetState(writer);

            writer.Write(_windowWasTriggeredThisFrame);
            writer.Write(_justEnteredHorizontalBlank);

            writer.Write(Enabled);
            writer.Write(_tileManager.WindowTileMapArea);
            writer.Write(_tileManager.BackgroundTileMapArea);
            writer.Write(_tileManager.TileDataArea);
            writer.Write(WindowEnabled);
            writer.Write(BackgroundAndWindowEnabledOrPriority);
            writer.Write(InterruptOnLineY);
            writer.Write(InterruptOnOAM);
            writer.Write(InterruptOnVerticalBlank);
            writer.Write(InterruptOnHorizontalBlank);
            writer.Write(ScreenX);
            writer.Write(ScreenY);
            writer.Write(WindowX);
            writer.Write(WindowY);
            writer.Write(CurrentLine);
            writer.Write(CurrentWindowLine);
            writer.Write(LineYToCompare);

            _tileManager.TilePalettes.GetState(writer);
            DMGPalettes.GetState(writer);

            _hdma.GetState(writer);

            writer.Write(_currentlyOnLineY);
            writer.Write((byte)_mode);
            writer.Write(_tileManager.Bank);
        }

        public void SetState(BinaryReader reader)
        {
            _lineDotCount = reader.ReadInt32();
            _linePixel = reader.ReadByte();
            _tileManager.VRAM = reader.ReadBytes(_tileManager.VRAM.Length);

            foreach (GBColor pixel in _screen)
            {
                pixel.SetState(reader);
            }

            _objectManager.SetState(reader);

            _windowWasTriggeredThisFrame = reader.ReadBoolean();
            _justEnteredHorizontalBlank = reader.ReadBoolean();

            Enabled = reader.ReadBoolean();
            _tileManager.WindowTileMapArea = reader.ReadInt32();
            _tileManager.BackgroundTileMapArea = reader.ReadInt32();
            _tileManager.TileDataArea = reader.ReadInt32();
            WindowEnabled = reader.ReadBoolean();
            BackgroundAndWindowEnabledOrPriority = reader.ReadBoolean();
            InterruptOnLineY = reader.ReadBoolean();
            InterruptOnOAM = reader.ReadBoolean();
            InterruptOnVerticalBlank = reader.ReadBoolean();
            InterruptOnHorizontalBlank = reader.ReadBoolean();
            ScreenX = reader.ReadInt32();
            ScreenY = reader.ReadInt32();
            WindowX = reader.ReadInt32();
            WindowY = reader.ReadInt32();
            CurrentLine = reader.ReadByte();
            CurrentWindowLine = reader.ReadByte();
            LineYToCompare = reader.ReadInt32();

            _tileManager.TilePalettes.SetState(reader);
            DMGPalettes.SetState(reader);

            _hdma.SetState(reader);

            _currentlyOnLineY = reader.ReadBoolean();
            _mode = (RenderMode)reader.ReadByte();
            _tileManager.Bank = reader.ReadByte();
        }
    }
}
