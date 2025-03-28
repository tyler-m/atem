
using Atem.Core.State;
using System.IO;

namespace Atem.Core.Graphics
{
    public class Sprite : IStateful
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public byte Flags;

        public byte ColorPalette { get => (byte)(Flags & 0b111); }
        public int Bank { get => Flags.GetBit(3).Int(); }
        public bool Palette { get => Flags.GetBit(4); }
        public bool FlipX { get => Flags.GetBit(5); }
        public bool FlipY { get => Flags.GetBit(6); }
        public bool Priority { get => Flags.GetBit(7); }

        public void Populate(byte y, byte x, byte tile, byte flags)
        {
            Y = y;
            X = x;
            Tile = tile;
            Flags = flags;
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Tile);
            writer.Write(Flags);
        }

        public void SetState(BinaryReader reader)
        {
            X = reader.ReadByte();
            Y = reader.ReadByte();
            Tile = reader.ReadByte();
            Flags = reader.ReadByte();
        }
    }
}
