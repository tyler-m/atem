using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public interface ITileManager : IStateful
    {
        public TileSet TileSet { get; }
        public TileMap TileMap { get; }
        public byte Bank { get; set; }
        public int WindowTileMapArea { get; set; }
        public int BackgroundTileMapArea { get; set; }
        public int TileDataArea { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public int WindowX { get; set; }
        public int WindowY { get; set; }
        public byte ReadVRAM(ushort address);
        public void WriteVRAM(ushort address, byte value, bool ignoreRenderMode);
        public (GBColor tileColor, int tileId, bool tilePriority) GetTileInfoAtScreenPixel(int pixelX, int pixelY, bool window);
    }
}
