using Atem.Core.Graphics.Palettes;
using Atem.Core.State;

namespace Atem.Core.Graphics.Objects
{
    public interface IObjectManager : IStateful
    {
        public bool ObjectsEnabled { get; set; }
        public bool LargeObjects { get; set; }
        public byte ODMA { get; set; }
        public void CollectObjectsForScanline();
        public (GBColor color, int spriteId, Sprite sprite) GetSpritePixelInfo(int x, int y);
        public void WriteOAM(ushort address, byte value);
        public byte ReadOAM(ushort address);
    }
}
