
using System.Collections.Generic;

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

        class Sprite
        {
            public byte X;
            public byte Y;
            public byte Tile;
            public byte Flags;

            public Sprite(byte x, byte y, byte tile, byte flags)
            {
                X = x;
                Y = y;
                Tile = tile;
                Flags = flags;
            }
        }

        List<Sprite> _spriteBuffer = new List<Sprite>();
        private int _oamScanIndex = 0;

        private Bus _bus;

        private int _lineDotCount;
        private byte _linePixel;
        private byte[] _vram = new byte[0x2000];
        private byte[] _oam = new byte[160];
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
        public byte WY;
        public byte WX;

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

                if (value == PPUMode.OAM)
                {
                    _oamScanIndex = 0;
                    _spriteBuffer.Clear();
                }
            }
        }

        public PPU(Bus bus)
        {
            _bus = bus;
            Mode = PPUMode.OAM;
        }

        public void WriteVRAM(ushort address, byte value)
        {
            if (Mode == PPUMode.Draw)
             {
                //return;
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

        public void WriteOAM(ushort address, byte value)
        {
            if (Mode == PPUMode.OAM || Mode == PPUMode.Draw)
            {
                return;
            }
            _oam[address & 0x00FF] = value;
        }

        public byte ReadOAM(ushort address)
        {
            if (Mode == PPUMode.OAM || Mode == PPUMode.Draw)
            {
                return 0xFF;
            }
            return _oam[address & 0x00FF];
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

            if (Mode == PPUMode.OAM)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (_spriteBuffer.Count >= 10)
                    {
                        break;
                    }

                    byte y = _oam[_oamScanIndex++];
                    byte x = _oam[_oamScanIndex++];
                    byte tile = _oam[_oamScanIndex++];
                    byte flags = _oam[_oamScanIndex++];

                    if (x > 0 && LY + 16 >= y && LY + 16 < y + 8)
                    {
                        _spriteBuffer.Add(new Sprite(x, y, tile, flags));
                    }
                }
            }
            else if (Mode == PPUMode.Draw)
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
                    byte bgColor = (byte)((BGP >> 2 * id) & 0b11);

                    bool foundSprite = false;
                    byte spriteColor = 0;
                    Sprite sprite = null;
                    for (int j = 0; j < _spriteBuffer.Count; j++)
                    {
                        sprite = _spriteBuffer[j];
                        if (sprite.X - 8 > _linePixel - 8 && sprite.X - 8 <= _linePixel)
                        {
                            foundSprite = true;
                            relativeX = _linePixel - (sprite.X - 8); // coordinates of pixel inside tile
                            relativeY = LY - (sprite.Y - 16);

                            tileDataAddress = GetTileDataAddress() + sprite.Tile * 16;
                            low = _vram[tileDataAddress + relativeY * 2];
                            high = _vram[tileDataAddress + relativeY * 2 + 1];
                            id = ((low >> (7 - relativeX)) & 1) | (((high >> (7 - relativeX)) & 1) << 1);
                            if (sprite.Flags.GetBit(4))
                            {
                                spriteColor = (byte)((OBP1 >> 2 * id) & 0b11);
                            }
                            else
                            {
                                spriteColor = (byte)((OBP0 >> 2 * id) & 0b11);
                            }
                        }
                    }

                    byte color = bgColor;

                    if (foundSprite)
                    {
                        if (spriteColor != 0)
                        {
                            if (!sprite.Flags.GetBit(7) || bgColor == 0)
                            {
                                color = spriteColor;
                            }
                        }
                    }

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
                        _bus.RequestInterrupt(InterruptType.VerticalBlank);
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
