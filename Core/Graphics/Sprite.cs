
namespace Atem.Core.Graphics
{
    internal class Sprite
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public byte Flags;

        public bool Palette { get { return Flags.GetBit(4); } }
        public bool FlipX { get { return Flags.GetBit(5); } }
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
}
