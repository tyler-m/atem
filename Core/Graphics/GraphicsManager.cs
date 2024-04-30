
using System.Collections.Generic;

namespace Atem.Core.Graphics
{
    internal class GraphicsManager
    {
        public static float FrameRate = 59.73f;

        public enum RenderMode
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

            public bool Palette { get { return Flags.GetBit(4); } }
            public bool FlipX  { get { return Flags.GetBit(5); } }
            public bool FlipY { get { return Flags.GetBit(6); } }
            public bool Priority { get { return Flags.GetBit(7); } }

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
        public byte SCY;
        public byte SCX;
        public byte LY;
        public byte LYC;
        public byte BGP;
        public byte OBP0;
        public byte OBP1;
        public byte WY;
        public byte WX;

        public byte STAT;

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

        public bool LCDEnabled { get { return LCDC.GetBit(7); } }
        public bool WindowTileMapMode { get { return LCDC.GetBit(6); } }
        public bool WindowEnabled { get { return LCDC.GetBit(5); } }
        public bool TileDataMode { get { return LCDC.GetBit(4); } }
        public bool BackgroundTileMapMode { get { return LCDC.GetBit(3); } }
        public bool LargeSprites { get { return LCDC.GetBit(2); } }

        public RenderMode Mode
        {
            get
            {
                return (RenderMode)(STAT & 0b11);
            }
            set
            {
                RenderMode prevMode = Mode;

                if (value == RenderMode.OAM)
                {
                    _oamScanIndex = 0;
                    _spriteBuffer.Clear();

                    if (prevMode != RenderMode.OAM && STAT.GetBit(5))
                    {
                        _bus.RequestInterrupt(InterruptType.STAT);
                    }
                }
                else if (value == RenderMode.VerticalBlank && prevMode != RenderMode.VerticalBlank && STAT.GetBit(4))
                {
                    _bus.RequestInterrupt(InterruptType.STAT);
                }
                else if (value == RenderMode.HorizontalBlank && prevMode != RenderMode.HorizontalBlank && STAT.GetBit(3))
                {
                    _bus.RequestInterrupt(InterruptType.STAT);
                }

                STAT = STAT
                    .SetBit(0, ((byte)value).GetBit(0))
                    .SetBit(1, ((byte)value).GetBit(1));
            }
        }

        public GraphicsManager(Bus bus)
        {
            _bus = bus;
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

        private ushort GetWindowTileMapAddress()
        {
            if (WindowTileMapMode)
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
            if (TileDataMode)
            {
                return (ushort)(0x0000 + tileIndex * 16);
            }
            else
            {
                sbyte offset = (sbyte)tileIndex;
                return (ushort)(0x1000 + offset * 16);
            }
        }

        public void Clock()
        {
            if (!LCDEnabled)
            {
                return;
            }

            if (Mode == RenderMode.OAM)
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

                    if (x > 0 && LY + 16 >= y && LY + 16 < y + 8 + (LargeSprites ? 8 : 0))
                    {
                        _spriteBuffer.Add(new Sprite(x, y, tile, flags));
                    }
                }
            }
            else if (Mode == RenderMode.Draw)
            {
                for (int i = 0; i < 4; i++)
                {
                    int tileMapX = (SCX + _linePixel) % 256;
                    int tileMapY = (SCY + LY) % 256;
                    int tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                    int tileMapAddress = GetTileMapAddress() + tileMapOffset;
                    byte tileIndex = _vram[tileMapAddress];

                    int tileDataAddress = GetTileDataAddress(tileIndex);
                    int relativeX = tileMapX % 8;
                    int relativeY = tileMapY % 8;
                    byte low = _vram[tileDataAddress + relativeY * 2];
                    byte high = _vram[tileDataAddress + relativeY * 2 + 1];
                    int id = ((low >> (7 - relativeX)) & 1) | (((high >> (7 - relativeX)) & 1) << 1);
                    byte bgColor = (byte)((BGP >> 2 * id) & 0b11);

                    if (WindowEnabled)
                    {
                        if (_linePixel > WX - 8 && WY <= LY)
                        {
                            tileMapX = _linePixel - (WX - 7);
                            tileMapY = LY - WY;
                            tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                            tileMapAddress = GetWindowTileMapAddress() + tileMapOffset;
                            tileIndex = _vram[tileMapAddress];

                            tileDataAddress = GetTileDataAddress(tileIndex);
                            relativeX = tileMapX % 8;
                            relativeY = tileMapY % 8;
                            low = _vram[tileDataAddress + relativeY * 2];
                            high = _vram[tileDataAddress + relativeY * 2 + 1];
                            id = ((low >> (7 - relativeX)) & 1) | (((high >> (7 - relativeX)) & 1) << 1);
                            bgColor = (byte)((BGP >> 2 * id) & 0b11);
                        }
                    }

                    bool foundSprite = false;
                    byte spriteColor = 0;
                    Sprite sprite = null;
                    for (int j = 0; j < _spriteBuffer.Count; j++)
                    {
                        sprite = _spriteBuffer[j];
                        if (sprite.X > _linePixel && sprite.X <= _linePixel + 8)
                        {
                            relativeX = _linePixel - (sprite.X - 8); // coordinates of pixel inside tile
                            relativeY = LY - (sprite.Y - 16);

                            if (LargeSprites && sprite.FlipY)
                            {
                                relativeY = 15 - relativeY;
                            }

                            tileDataAddress = sprite.Tile * 16;
                            low = _vram[tileDataAddress + relativeY * 2];
                            high = _vram[tileDataAddress + relativeY * 2 + 1];
                            byte relativeBit = (byte)(sprite.FlipX ? relativeX : 7 - relativeX);
                            id = ((low >> relativeBit) & 1) | (((high >> relativeBit) & 1) << 1);

                            if (id == 0) // the pixel on this sprite is transparent
                            {
                                continue;
                            }

                            if (sprite.Palette)
                            {
                                spriteColor = (byte)((OBP1 >> 2 * id) & 0b11);
                            }
                            else
                            {
                                spriteColor = (byte)((OBP0 >> 2 * id) & 0b11);
                            }
                            foundSprite = true;
                            break;
                        }
                    }

                    byte color = bgColor;

                    if (foundSprite)
                    {
                        if (!sprite.Flags.GetBit(7) || bgColor == 0)
                        {
                            color = spriteColor;
                        }
                    }

                    _screen[LY * 160 + _linePixel] = color;
                    _linePixel++;
                }
            }

            _lineDotCount += 4;

            UpdateMode();

            if (!STAT.GetBit(2) && LY == LYC)
            {
                if (STAT.GetBit(6))
                {
                    _bus.RequestInterrupt(InterruptType.STAT);
                }
            }
            STAT = STAT.SetBit(2, LYC == LY);
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
                        Mode = RenderMode.VerticalBlank;
                        OnVerticalBlank?.Invoke(_screen);
                        _bus.RequestInterrupt(InterruptType.VerticalBlank);
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
                    LY++;
                    _lineDotCount = 0;
                }

                if (LY > 153)
                {
                    LY = 0;
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
