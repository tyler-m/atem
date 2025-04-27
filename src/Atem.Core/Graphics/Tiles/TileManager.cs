using System.IO;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Timing;
using Atem.Core.Memory;

namespace Atem.Core.Graphics.Tiles
{
    public class TileManager : ITileManager
    {
        private readonly IRenderModeScheduler _renderModeScheduler;
        private readonly IPaletteProvider _paletteProvider;
        private readonly Cartridge _cartridge;

        private readonly TileSet _tileSet = new();
        private readonly TileMap _tileMap = new();

        public byte Bank { get; set; }
        public int TileDataArea { get; set; }
        public int WindowTileMapArea { get; set; }
        public int BackgroundTileMapArea { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public int WindowX { get; set; }
        public int WindowY { get; set; }
        public TileSet TileSet => _tileSet;
        public TileMap TileMap => _tileMap;

        public TileManager(IRenderModeScheduler renderModeScheduler, IPaletteProvider paletteProvider, Cartridge cartridge)
        {
            _renderModeScheduler = renderModeScheduler;
            _paletteProvider = paletteProvider;
            _cartridge = cartridge;
        }

        public byte ReadVRAM(ushort address)
        {
            if (_renderModeScheduler.Mode == RenderMode.Draw)
            {
                return 0xFF;
            }

            address -= 0x8000;

            if (address <= 0x17FF)
            {
                // reading tile data
                int tileOffset = address % 16;
                int tileIndex = address / 16;
                return _tileSet.GetTile(tileIndex, Bank).GetByte(tileOffset);
            }
            else
            {
                address -= 0x1800;
                if (Bank == 0)
                {
                    // reading tile map data
                    return _tileMap.GetTileIndex(address);
                }
                else
                {
                    // reading tile data attribute
                    return _tileMap.GetTileAttribute(address).Value;
                }
            }
        }

        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode = false)
        {
            if (_renderModeScheduler.Mode == RenderMode.Draw && !ignoreRenderMode)
            {
                return;
            }

            address -= 0x8000;

            if (address <= 0x17FF)
            {
                // writing tile data
                int tileOffset = address % 16;
                int tileIndex = address / 16;
                _tileSet.GetTile(tileIndex, Bank).SetByte(tileOffset, value);
            }
            else
            {
                address -= 0x1800;
                if (Bank == 0)
                {
                    // setting tile map data
                    _tileMap.SetTileIndex(address, value);
                }
                else
                {
                    // setting tile data attribute
                    _tileMap.GetTileAttribute(address).Set(value);
                }
            }
        }

        public (GBColor tileColor, int tileId, bool tilePriority) GetTileInfoAtScreenPixel(int pixelX, int pixelY, bool window)
        {
            int tileMapX, tileMapY, tileId, tileIndex, area;
            bool tilePriority;
            Palette tilePalette;
            TileAttribute tileAttribute;

            if (window)
            {
                tileMapX = pixelX - (WindowX - 7);
                tileMapY = pixelY;
                area = WindowTileMapArea;
            }
            else
            {
                tileMapX = (ScreenX + pixelX) % 256;
                tileMapY = (ScreenY + pixelY) % 256;
                area = BackgroundTileMapArea;
            }

            tileIndex = _tileMap.GetTileIndexOfTileMapArea(tileMapX / 8, tileMapY / 8, area);
            tileAttribute = _tileMap.GetTileAttributeOfTileMapArea(tileMapX / 8, tileMapY / 8, area);

            // adjust tile index
            if (TileDataArea == 0)
            {
                tileIndex = 256 + (sbyte)tileIndex;
            }

            if (_cartridge.SupportsColor)
            {
                tilePriority = tileAttribute.Priority;
                tilePalette = _paletteProvider.TilePalettes[tileAttribute.Palette];
                tileId = _tileSet.GetTile(tileIndex, tileAttribute.Bank).GetPixel(tileMapX % 8, tileMapY % 8, tileAttribute.FlipX, tileAttribute.FlipY);
            }
            else
            {
                tilePalette = _paletteProvider.DMGPalettes[0];
                tileId = _tileSet.GetTile(tileIndex, 0).GetPixel(tileMapX % 8, tileMapY % 8);
                tilePriority = false;
            }

            return (tilePalette[tileId], tileId, tilePriority);
        }

        public void GetState(BinaryWriter writer)
        {
            _tileSet.GetState(writer);
            _tileMap.GetState(writer);

            writer.Write(WindowTileMapArea);
            writer.Write(BackgroundTileMapArea);
            writer.Write(TileDataArea);
            writer.Write(Bank);
            writer.Write(WindowX);
            writer.Write(WindowY);
            writer.Write(ScreenX);
            writer.Write(ScreenY);
        }

        public void SetState(BinaryReader reader)
        {
            _tileSet.SetState(reader);
            _tileMap.SetState(reader);

            WindowTileMapArea = reader.ReadInt32();
            BackgroundTileMapArea = reader.ReadInt32();
            TileDataArea = reader.ReadInt32();
            Bank = reader.ReadByte();
            WindowX = reader.ReadInt32();
            WindowY = reader.ReadInt32();
            ScreenX = reader.ReadInt32();
            ScreenY = reader.ReadInt32();
        }
    }
}