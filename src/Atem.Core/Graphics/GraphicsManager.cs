using System.Collections.Generic;

namespace Atem.Core.Graphics
{
    public delegate void VerticalBlankEvent(GBColor[] screen);

    public enum RenderMode
    {
        HorizontalBlank,
        VerticalBlank,
        OAM,
        Draw
    }

    public class GraphicsManager
    {
        public static float FrameRate = 59.73f;

        private Bus _bus;
        private List<Sprite> _spriteBuffer = new List<Sprite>();
        private int _lineDotCount;
        private byte _linePixel;
        private byte[] _vram = new byte[0x4000];
        private GBColor[] _screen = new GBColor[160 * 144];
        private Sprite[] _objects = new Sprite[40];
        private int _objectIndex = 0;

        private bool _windowWasTriggeredThisFrame;
        private bool _justEnteredHorizontalBlank;

        public GraphicsRegisters Registers;
        public event VerticalBlankEvent OnVerticalBlank;

        public bool Enabled = false;
        public int WindowTileMapArea = 0;
        public int BackgroundTileMapArea = 0;
        public int TileDataArea = 0;
        public bool WindowEnabled = false;
        public bool LargeObjects = false;
        public bool ObjectsEnabled = false;
        public bool BackgroundAndWindowEnabledOrPriority = false;
        public bool InterruptOnLineY = false;
        public bool InterruptOnOAM = false;
        public bool InterruptOnVerticalBlank = false;
        public bool InterruptOnHorizontalBlank = false;
        public int ScreenX = 0;
        public int ScreenY = 0;
        public int WindowX = 0;
        public int WindowY = 0;
        public byte CurrentLine = 0;
        public byte CurrentWindowLine = 0;
        public int LineYToCompare = 0;
        public PaletteGroup TilePalettes = new PaletteGroup();
        public PaletteGroup ObjectPalettes = new PaletteGroup();
        public PaletteGroup DMGPalettes = new PaletteGroup();
        public ushort SourceAddressDMA = 0;
        public ushort DestAddressDMA = 0;
        public byte TransferLengthRemaining = 0;
        public bool TransferActive = false;
        public bool HorizontalBlankTransferActive = false;
        public bool HorizontalBlankTransfer = false;

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

        private byte _odma;

        public byte ODMA
        {
            get
            {
                return _odma;
            }
            set
            {
                int address = value << 8;

                for (int i = 0; i < 40; i++)
                {
                    _objects[i].Populate(
                        _bus.Read((ushort)(address + 4*i)),
                        _bus.Read((ushort)(address + 4*i + 1)),
                        _bus.Read((ushort)(address + 4*i + 2)),
                        _bus.Read((ushort)(address + 4*i + 3)));
                }

                _odma = value;
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
                    _objectIndex = 0;
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

        public byte Bank { get; set; }

        public GraphicsManager(Bus bus)
        {
            _bus = bus;
            Registers = new GraphicsRegisters(this);
            TilePalettes[0] = new Palette(new GBColor[] { GBColor.FromValue(0x1F), GBColor.FromValue(0), GBColor.FromValue(0), GBColor.FromValue(0) });
            for (int i = 0; i < _objects.Length; i++)
            {
                _objects[i] = new Sprite();
            }
            Mode = RenderMode.OAM;
        }

        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode = false)
        {
            address -= 0x8000;

            if (Mode == RenderMode.Draw & !ignoreRenderMode)
            {
                return;
            }

            _vram[address + 0x2000 * Bank] = value;
        }

        public byte ReadVRAM(ushort address)
        {
            address -= 0x8000;

            if (Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            return _vram[address + 0x2000 * Bank];
        }

        public void WriteOAM(ushort address, byte value)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return;
            }

            int adjustedAddress = address & 0xFF;
            int index = adjustedAddress / 4;

            if (adjustedAddress % 4 == 0)
            {
                _objects[index].Y = value;
            }
            else if (adjustedAddress % 4 == 1)
            {
                _objects[index].X = value;
            }
            else if (adjustedAddress % 4 == 2)
            {
                _objects[index].Tile = value;
            }
            else
            {
                _objects[index].Flags = value;
            }
        }

        public byte ReadOAM(ushort address)
        {
            if (Mode == RenderMode.OAM || Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            int adjustedAddress = address & 0xFF;
            int index = adjustedAddress / 4;

            if (adjustedAddress % 4 == 0)
            {
                return _objects[index].Y;
            }
            else if (adjustedAddress % 4 == 1)
            {
                return _objects[index].X;
            }
            else if (adjustedAddress % 4 == 2)
            {
                return _objects[index].Tile;
            }
            else
            {
                return _objects[index].Flags;
            }
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

        private ushort GetTileDataAddress(int tileIndex, int bank = 0)
        {
            if (TileDataArea == 1)
            {
                return (ushort)((0x0000 + tileIndex * 16) + bank * 0x2000);
            }
            else
            {
                sbyte offset = (sbyte)tileIndex;
                return (ushort)((0x1000 + offset * 16) + bank * 0x2000);
            }
        }

        private GBColor GetColorOfScreenPixel(byte pixelX, byte pixelY)
        {
            bool window = WindowEnabled && pixelX > WindowX - 8 && WindowY <= pixelY;
            _windowWasTriggeredThisFrame |= window;

            (GBColor tileColor, int tileId, bool tilePriority) = GetTileInfo(pixelX, window ? CurrentWindowLine : pixelY, window);

            if (!_bus.ColorMode && !BackgroundAndWindowEnabledOrPriority)
            {
                tileColor = new GBColor(0xFFFF);
            }

            (GBColor spriteColor, int spriteId, Sprite sprite) = GetSpriteInfo(pixelX, pixelY);

            GBColor pixelColor = tileColor;
            if (sprite != null && spriteId != 0 && ObjectsEnabled)
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

        public int GetTileId(int tileDataAddress, int offsetX, int offsetY, bool flipX = false, bool flipY = false)
        {
            if (flipX)
            {
                offsetX = 7 - offsetX;
            }

            if (flipY)
            {
                offsetY = 7 - offsetY;
            }

            byte low = _vram[tileDataAddress + offsetY * 2];
            byte high = _vram[tileDataAddress + offsetY * 2 + 1];
            return ((low >> (7 - offsetX)) & 1) | (((high >> (7 - offsetX)) & 1) << 1);
        }

        public int GetSpriteId(Sprite sprite, int pixelX, int pixelY)
        {
            int offsetX = pixelX - (sprite.X - 8); // coordinates of pixel inside tile at (x, y)
            int offsetY = pixelY - (sprite.Y - 16);
            int spriteTile = sprite.Tile;

            if (LargeObjects)
            {
                // ignore first bit of tile with 8x16 objects
                spriteTile &= 0b11111110;

                if (sprite.FlipY)
                {
                    offsetY = 15 - offsetY;
                }
            }
            else
            {
                if (sprite.FlipY)
                {
                    offsetY = 7 - offsetY;
                }
            }

            int tileDataAddress = spriteTile * 16 + (_bus.ColorMode ? 0x2000 * sprite.Bank : 0);
            int id = GetTileId(tileDataAddress, offsetX, offsetY, sprite.FlipX);
            return id;
        }

        public (GBColor tileColor, int tileId, bool tilePriority) GetTileInfo(int pixelX, int pixelY, bool window)
        {
            int tileMapX, tileMapY, tileMapAddress, tileId, tileIndex;
            Palette tilePalette;
            bool tilePriority;

            if (window)
            {
                tileMapX = pixelX - (WindowX - 7);
                tileMapY = pixelY;
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

            tileIndex = _vram[tileMapAddress]; // the index of the tile that the pixel at (x, y) belongs to

            if (_bus.ColorMode)
            {
                byte bgMapAttributes = _vram[tileMapAddress + 0x2000];
                int paletteIndex = bgMapAttributes & 0b111;
                int bank = bgMapAttributes.GetBit(3).Int();
                bool flipX = bgMapAttributes.GetBit(5);
                bool flipY = bgMapAttributes.GetBit(6);
                tilePriority = bgMapAttributes.GetBit(7);
                tilePalette = TilePalettes[paletteIndex];
                int tileDataAddress = GetTileDataAddress(tileIndex, bank);
                tileId = GetTileId(tileDataAddress, tileMapX % 8, tileMapY % 8, flipX, flipY);
            }
            else
            {
                tilePalette = DMGPalettes[0];
                int tileDataAddress = GetTileDataAddress(tileIndex);
                tileId = GetTileId(tileDataAddress, tileMapX % 8, tileMapY % 8);
                tilePriority = false;
            }

            return (tilePalette[tileId], tileId, tilePriority);
        }

        public (GBColor spriteColor, int spriteId, Sprite sprite) GetSpriteInfo(int pixelX, int pixelY)
        {
            GBColor spriteColor = null;
            Sprite sprite = null;
            int spriteId = 0;

            for (int j = 0; j < _spriteBuffer.Count; j++)
            {
                Sprite tempSprite = _spriteBuffer[j];

                if (tempSprite.X > pixelX && tempSprite.X <= pixelX + 8)
                {
                    int id = GetSpriteId(tempSprite, pixelX, pixelY);

                    if (id == 0)
                    {
                        continue;
                    }

                    if (_bus.ColorMode)
                    {
                        // in color mode object locations in OAM determines priority
                        // (the object list is populated in order of OAM location, so we
                        // just take the first visible object we encounter in the list)
                        if (sprite == null)
                        {
                            spriteColor = ObjectPalettes[tempSprite.ColorPalette][id];
                            sprite = tempSprite;
                            spriteId = id;
                        }
                    }
                    else
                    {
                        // in no-color mode, object X coordinates determine priority.
                        // if X coordinates between objects are identical, the object
                        // earliest in OAM is prioritized
                        if (sprite == null || tempSprite.X < sprite.X)
                        {
                            if (tempSprite.Palette)
                            {
                                spriteColor = DMGPalettes[2][id];
                            }
                            else
                            {
                                spriteColor = DMGPalettes[1][id];
                            }

                            sprite = tempSprite;
                            spriteId = id;
                        }
                    }
                }
            }

            return (spriteColor, spriteId, sprite);
        }

        public void Clock()
        {
            if (!Enabled)
            {
                return;
            }

            ClockTransfer();

            if (Mode == RenderMode.OAM)
            {
                for (int i = 0; i < 2; i++)
                {
                    // find object to add to the object buffer for the current line
                    if (_spriteBuffer.Count < 10)
                    {
                        Sprite sprite = _objects[_objectIndex++];

                        if (sprite.X > 0 && CurrentLine + 16 >= sprite.Y && CurrentLine + 16 < sprite.Y + 8 + (LargeObjects ? 8 : 0))
                        {
                            _spriteBuffer.Add(sprite);
                        }
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
                    _justEnteredHorizontalBlank = true;
                }
            }
        }

        private void ClockTransfer()
        {
            if(TransferActive && _justEnteredHorizontalBlank && HorizontalBlankTransfer)
            {
                HorizontalBlankTransferActive = true;
            }

            // this condition is satisfied only once per horizontal blank
            if (HorizontalBlankTransferActive)
            {
                for (int i = 0; i < 16; i++)
                {
                    WriteVRAM(DestAddressDMA, _bus.Read(SourceAddressDMA), true);
                    DestAddressDMA++;
                    SourceAddressDMA++;
                }

                TransferLengthRemaining--;
                HorizontalBlankTransferActive = false;

                if (TransferLengthRemaining == 0xFF)
                {
                    TransferActive = false;
                }
            }

            if (_justEnteredHorizontalBlank)
            {
                _justEnteredHorizontalBlank = false;
            }
        }

        public void StartTransfer(byte value)
        {
            if (TransferActive && !value.GetBit(7))
            {
                // cancel current horizontal blank transfer
                TransferActive = false;
            }
            else
            {
                HorizontalBlankTransfer = value.GetBit(7);

                // number of bytes to transfer divided by 16, minus 1
                // e.g. a length of 0 means 16 bytes to transfer
                TransferLengthRemaining = (byte)(value & 0x7F);

                if (HorizontalBlankTransfer)
                {
                    TransferActive = true;
                }
                else
                {
                    // because we aren't waiting for horizontal blank
                    // we fulfill the transfer request in-place
                    while (TransferLengthRemaining != 0xFF)
                    {
                        WriteVRAM(DestAddressDMA, _bus.Read(SourceAddressDMA), true);
                        DestAddressDMA++;
                        SourceAddressDMA++;

                        if ((DestAddressDMA & 0xF) == 0)
                        {
                            TransferLengthRemaining--;
                        }
                    }
                }
            }
        }
    }
}
