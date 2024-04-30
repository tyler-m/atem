using System.Collections.Generic;

namespace Atem.Core.Graphics
{
    internal delegate void VerticalBlankEvent(byte[] screen);

    internal enum RenderMode
    {
        HorizontalBlank,
        VerticalBlank,
        OAM,
        Draw
    }

    internal class GraphicsManager
    {
        public static float FrameRate = 59.73f;

        private Bus _bus;
        private List<Sprite> _spriteBuffer = new List<Sprite>();
        private int _oamScanIndex = 0;
        private int _lineDotCount;
        private byte _linePixel;
        private byte[] _vram = new byte[0x2000];
        private byte[] _oam = new byte[160];
        private byte[] _screen = new byte[160 * 144];

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;

        public bool Enabled = false;
        public int WindowTileMapArea = 0;
        public int BackgroundTileMapArea = 0;
        public int TileDataArea = 0;
        public bool WindowEnabled = false;
        public bool LargeObjects = false;
        public bool ObjectsEnabled = false;
        public bool BackgroundAndWindowEnabled = false;
        public bool InterruptOnLineY = false;
        public bool InterruptOnOAM = false;
        public bool InterruptOnVerticalBlank = false;
        public bool InterruptOnHorizontalBlank = false;
        public int ScreenX = 0;
        public int ScreenY = 0;
        public int WindowX = 0;
        public int WindowY = 0;
        public byte CurrentLine = 0;
        public int LineYToCompare = 0;
        public Palette BackgroundPalette = new Palette(0);
        public Palette ObjectPalette0 = new Palette(0);
        public Palette ObjectPalette1 = new Palette(0);

        private bool _currentlyOnLineY;
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

        private byte _dma;

        public byte DMA
        {
            get
            {
                return _dma;
            }
            set
            {
                for (int i = 0; i < 160; i++)
                {
                    _oam[i] = _bus.Read((ushort)((value << 8) + i));
                }

                _dma = value;
            }
        }

        RenderMode _mode;
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
                    _oamScanIndex = 0;
                    _spriteBuffer.Clear();

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

        public GraphicsManager(Bus bus)
        {
            _bus = bus;
            Registers = new GraphicsRegisters(this);
            Mode = RenderMode.OAM;
        }

        public void WriteVRAM(ushort address, byte value)
        {
            if (Mode == RenderMode.Draw)
             {
                return;
            }
            _vram[address & 0x1FFF] = value;
        }

        public byte ReadVRAM(ushort address)
        {
            if (Mode == RenderMode.Draw)
            {
                return 0xFF;
            }
            return _vram[address & 0x1FFF];
        }

        public void WriteOAM(ushort address, byte value)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return;
            }
            _oam[address & 0x00FF] = value;
        }

        public byte ReadOAM(ushort address)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return 0xFF;
            }
            return _oam[address & 0x00FF];
        }

        private ushort GetBackgroundTileMapAddress()
        {
            if (BackgroundTileMapArea == 1)
            {
                return 0x1C00;
            }
            else
            {
                return 0x1800;
            }
        }

        private ushort GetWindowTileMapAddress()
        {
            if (WindowTileMapArea == 1)
            {
                return 0x1C00;
            }
            else
            {
                return 0x1800;
            }
        }

        private ushort GetTileDataAddress(int tileIndex)
        {
            if (TileDataArea == 1)
            {
                return (ushort)(0x0000 + tileIndex * 16);
            }
            else
            {
                sbyte offset = (sbyte)tileIndex;
                return (ushort)(0x1000 + offset * 16);
            }
        }

        private void FindObject(byte lineY)
        {
            byte y = _oam[_oamScanIndex++];
            byte x = _oam[_oamScanIndex++];
            byte tile = _oam[_oamScanIndex++];
            byte flags = _oam[_oamScanIndex++];

            if (x > 0 && lineY + 16 >= y && lineY + 16 < y + 8 + (LargeObjects ? 8 : 0))
            {
                _spriteBuffer.Add(new Sprite(x, y, tile, flags));
            }
        }

        private byte GetTileColorFromScreenPixel(int pixelX, int pixelY, bool window = false)
        {
            int tileMapX, tileMapY, tileMapAddress;

            if (window)
            {
                tileMapX = pixelX - (WindowX - 7);
                tileMapY = pixelY - WindowY;
                int tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                tileMapAddress = GetWindowTileMapAddress() + tileMapOffset;
            }
            else
            {
                tileMapX = (ScreenX + pixelX) % 256;
                tileMapY = (ScreenY + pixelY) % 256;
                int tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                tileMapAddress = GetBackgroundTileMapAddress() + tileMapOffset;
            }

            byte tileIndex = _vram[tileMapAddress];

            int tileDataAddress = GetTileDataAddress(tileIndex);
            int relativeX = tileMapX % 8;
            int relativeY = tileMapY % 8;
            byte low = _vram[tileDataAddress + relativeY * 2];
            byte high = _vram[tileDataAddress + relativeY * 2 + 1];

            int id = ((low >> (7 - relativeX)) & 1) | (((high >> (7 - relativeX)) & 1) << 1);
            return BackgroundPalette[id];
        }

        private byte GetColorOfScreenPixel(byte pixelX, byte pixelY)
        {
            bool window = WindowEnabled && pixelX > WindowX - 8 && WindowY <= pixelY;
            byte tileColor = GetTileColorFromScreenPixel(pixelX, pixelY, window);

            bool foundSprite = false;
            byte spriteColor = 0;
            Sprite sprite = null;
            for (int j = 0; j < _spriteBuffer.Count; j++)
            {
                sprite = _spriteBuffer[j];
                if (sprite.X > pixelX && sprite.X <= pixelX + 8)
                {
                    int relativeX = pixelX - (sprite.X - 8); // coordinates of pixel inside tile
                    int relativeY = pixelY - (sprite.Y - 16);

                    if (LargeObjects && sprite.FlipY)
                    {
                        relativeY = 15 - relativeY;
                    }

                    int tileDataAddress = sprite.Tile * 16;
                    byte low = _vram[tileDataAddress + relativeY * 2];
                    byte high = _vram[tileDataAddress + relativeY * 2 + 1];
                    byte relativeBit = (byte)(sprite.FlipX ? relativeX : 7 - relativeX);
                    int id = ((low >> relativeBit) & 1) | (((high >> relativeBit) & 1) << 1);

                    if (id == 0) // the pixel on this sprite is transparent
                    {
                        continue;
                    }

                    if (sprite.Palette)
                    {
                        spriteColor = ObjectPalette1[id];
                    }
                    else
                    {
                        spriteColor = ObjectPalette0[id];
                    }

                    foundSprite = true;
                    break;
                }
            }

            byte color = tileColor;
            if (foundSprite)
            {
                if (!sprite.Flags.GetBit(7) || tileColor == 0)
                {
                    color = spriteColor;
                }
            }

            return color;
        }

        public void Clock()
        {
            if (!Enabled)
            {
                return;
            }

            if (Mode == RenderMode.OAM)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (_spriteBuffer.Count < 10)
                    {
                        FindObject(CurrentLine);
                    }
                }
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
            if (Mode == 0)
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
                    Mode = 0;
                    _linePixel = 0;
                }
            }
        }
    }
}
