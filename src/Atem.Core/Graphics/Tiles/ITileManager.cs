using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public interface ITileManager : IStateful
    {
        public byte[] VRAM { get; set; }
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
        public int GetTileId(int tileDataAddress, int offsetX, int offsetY, bool flipX = false, bool flipY = false);
        public (GBColor tileColor, int tileId, bool tilePriority) GetTileInfo(int pixelX, int pixelY, bool window);
    }
}
