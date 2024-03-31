
namespace Atem
{
    internal class PPU
    {
        public enum PPUMode
        {
            HorizontalBlank,
            VerticalBlank,
            OAM,
            Draw
        }

        private int _lineDotCount;
        private byte _linePixel;
        private byte[] _vram = new byte[0x2000];
        private byte[] _screen = new byte[160*144];

        public delegate void VerticalBlankEvent(byte[] screen);
        public event VerticalBlankEvent OnVerticalBlank;

        public byte LCDC;
        public byte STAT;
        public byte SCY;
        public byte SCX;
        public byte LY;
        public byte BGP;
        public byte OBP0;
        public byte OBP1;

        public bool LCDEnabled
        {
            get
            {
                return LCDC.GetBit(7);
            }
        }

        public bool TileDataMode
        {
            get
            {
                return LCDC.GetBit(4);
            }
        }

        public bool BackgroundTileMapMode
        {
            get
            {
                return LCDC.GetBit(3);
            }
        }

        public PPUMode Mode
        {
            get
            {
                return (PPUMode)(STAT & 0b11);
            }
            set
            {
                STAT = STAT
                    .SetBit(0, ((byte)value).GetBit(0))
                    .SetBit(1, ((byte)value).GetBit(1));
            }
        }

        public PPU()
        {
            Mode = PPUMode.OAM;
        }

        public void WriteVRAM(ushort address, byte value)
        {
            if (Mode == PPUMode.Draw)
            {
                return;
            }
            _vram[address & 0x1FFF] = value;
        }

        public byte ReadVRAM(ushort address)
        {
            if (Mode == PPUMode.Draw)
            {
                return 0xFF;
            }
            return _vram[address & 0x1FFF];
        }

        private ushort GetTileMapAddress()
        {
            if (BackgroundTileMapMode)
            {
                return 0x1C00;
            }
            else
            {
                return 0x1800;
            }
        }

        private ushort GetTileDataAddress()
        {
            if (TileDataMode)
            {
                return 0x0000;
            }
            else
            {
                return 0x1000;
            }
        }

        public void Clock()
        {
            if (!LCDEnabled)
            {
                return;
            }

            if (Mode == PPUMode.Draw)
            {
                for (int i = 0; i < 4; i++)
                {
                    int tileMapX = (SCX + _linePixel) % 256;
                    int tileMapY = (SCY + LY) % 256;
                    int tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                    int tileMapAddress = GetTileMapAddress() + tileMapOffset;
                    byte tileIndex = _vram[tileMapAddress];

                    int tileDataAddress = GetTileDataAddress() + tileIndex * 16;
                    int relativeX = tileMapX % 8;
                    int relativeY = tileMapY % 8;
                    byte low = _vram[tileDataAddress + relativeY * 2];
                    byte high = _vram[tileDataAddress + relativeY * 2 + 1];
                    int id = ((low >> (7 - relativeX)) & 1) | (((high >> (7 - relativeX)) & 1) << 1);
                    byte color = (byte)((BGP >> 2 * id) & 0b11);

                    _screen[LY * 160 + _linePixel] = color;
                    _linePixel++;
                }
            }

            _lineDotCount += 4;

            UpdateMode();
        }

        public void UpdateMode()
        {
            if (Mode == 0)
            {
                if (_lineDotCount >= 456) 
                {
                    LY++;
                    _lineDotCount = 0;

                    if (LY >= 144)
                    {
                        Mode = PPUMode.VerticalBlank;
                        OnVerticalBlank?.Invoke(_screen);
                    }
                    else
                    {
                        Mode = PPUMode.OAM;
                    }
                }
            }
            else if (Mode == PPUMode.VerticalBlank)
            {
                if (_lineDotCount >= 456)
                {
                    LY++;
                    _lineDotCount = 0;
                }

                if (LY > 153)
                {
                    LY = 0;
                    _lineDotCount = 0;
                    Mode = PPUMode.OAM;
                }
            }
            else if (Mode == PPUMode.OAM)
            {
                if (_lineDotCount >= 80)
                {
                    Mode = PPUMode.Draw;
                }
            }
            else if (Mode == PPUMode.Draw)
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
