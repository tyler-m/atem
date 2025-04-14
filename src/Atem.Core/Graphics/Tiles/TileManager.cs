using System.IO;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Timing;

namespace Atem.Core.Graphics.Tiles
{
    public class TileManager : ITileManager
    {
        private byte[] _vram;
        private byte _bank;
        private int _windowTileMapArea;
        private int _backgroundTileMapArea;
        private int _tileDataArea;
        private int _screenX;
        private int _screenY;
        private int _windowX;
        private int _windowY;
        private readonly IBus _bus;
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly IPaletteProvider _paletteProvider;

        public byte[] VRAM { get => _vram; set => _vram = value; }
        public byte Bank { get => _bank; set => _bank = value; }
        public int WindowTileMapArea { get => _windowTileMapArea; set => _windowTileMapArea = value; }
        public int BackgroundTileMapArea { get => _backgroundTileMapArea; set => _backgroundTileMapArea = value; }
        public int TileDataArea { get => _tileDataArea; set => _tileDataArea = value; }
        public int ScreenX { get => _screenX; set => _screenX = value; }
        public int ScreenY { get => _screenY; set => _screenY = value; }
        public int WindowX { get => _windowX; set => _windowX = value; }
        public int WindowY { get => _windowY; set => _windowY = value; }

        public TileManager(IBus bus, IRenderModeScheduler renderModeScheduler, IPaletteProvider paletteProvider)
        {
            _bus = bus;
            _renderModeScheduler = renderModeScheduler;
            _paletteProvider = paletteProvider;

            _vram = new byte[0x4000];
        }

        public byte ReadVRAM(ushort address)
        {
            if (_renderModeScheduler.Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            return _vram[address - 0x8000 + _bank * 0x2000];
        }

        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode = false)
        {
            if (_renderModeScheduler.Mode == RenderMode.Draw && !ignoreRenderMode)
            {
                return;
            }

            _vram[address - 0x8000 + _bank * 0x2000] = value;
        }

        private ushort GetWindowTileMapAddress()
        {
            if (_windowTileMapArea == 1)
            {
                return 0x1C00;
            }
            else
            {
                return 0x1800;
            }
        }

        private ushort GetBackgroundTileMapAddress()
        {
            if (_backgroundTileMapArea == 1)
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
            if (_tileDataArea == 1)
            {
                return (ushort)(0x0000 + tileIndex * 16 + bank * 0x2000);
            }
            else
            {
                sbyte offset = (sbyte)tileIndex;
                return (ushort)(0x1000 + offset * 16 + bank * 0x2000);
            }
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
            return low >> 7 - offsetX & 1 | (high >> 7 - offsetX & 1) << 1;
        }

        public (GBColor tileColor, int tileId, bool tilePriority) GetTileInfo(int pixelX, int pixelY, bool window)
        {
            int tileMapX, tileMapY, tileMapAddress, tileId, tileIndex;
            Palette tilePalette;
            bool tilePriority;

            if (window)
            {
                tileMapX = pixelX - (_windowX - 7);
                tileMapY = pixelY;
                int tileMapOffset = tileMapY / 8 * 32 + tileMapX / 8;
                tileMapAddress = GetWindowTileMapAddress() + tileMapOffset;
            }
            else
            {
                tileMapX = (_screenX + pixelX) % 256;
                tileMapY = (_screenY + pixelY) % 256;
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
                tilePalette = _paletteProvider.TilePalettes[paletteIndex];
                int tileDataAddress = GetTileDataAddress(tileIndex, bank);
                tileId = GetTileId(tileDataAddress, tileMapX % 8, tileMapY % 8, flipX, flipY);
            }
            else
            {
                tilePalette = _paletteProvider.DMGPalettes[0];
                int tileDataAddress = GetTileDataAddress(tileIndex);
                tileId = GetTileId(tileDataAddress, tileMapX % 8, tileMapY % 8);
                tilePriority = false;
            }

            return (tilePalette[tileId], tileId, tilePriority);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_vram);
            writer.Write(_windowTileMapArea);
            writer.Write(_backgroundTileMapArea);
            writer.Write(_tileDataArea);
            writer.Write(_bank);
            writer.Write(_windowX);
            writer.Write(_windowY);
            writer.Write(_screenX);
            writer.Write(_screenY);
        }

        public void SetState(BinaryReader reader)
        {
            _vram = reader.ReadBytes(_vram.Length);
            _windowTileMapArea = reader.ReadInt32();
            _backgroundTileMapArea = reader.ReadInt32();
            _tileDataArea = reader.ReadInt32();
            _bank = reader.ReadByte();
            _windowX = reader.ReadInt32();
            _windowY = reader.ReadInt32();
            _screenX = reader.ReadInt32();
            _screenY = reader.ReadInt32();
        }
    }
}