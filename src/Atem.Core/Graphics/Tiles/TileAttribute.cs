using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public class TileAttribute : IStateful
    {
        public byte Value { get; private set; }
        public bool Priority { get; private set; }
        public bool FlipY { get; private set; }
        public bool FlipX { get; private set; }
        public byte Bank { get; private set; }
        public byte Palette { get; private set; }

        public void Set(byte value)
        {
            Value = value;
            Priority = Value.GetBit(7);
            FlipY = Value.GetBit(6);
            FlipX = Value.GetBit(5);
            Bank = (byte)(Value.GetBit(3) ? 1 : 0);
            Palette = (byte)(Value & 0b111);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(Value);
        }

        public void SetState(BinaryReader reader)
        {
            Set(reader.ReadByte());
        }
    }
}
