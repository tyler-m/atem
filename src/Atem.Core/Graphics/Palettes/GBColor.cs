using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Palettes
{
    public class GBColor : IStateful
    {
        private byte _red;
        private byte _green;
        private byte _blue;

        public int Red
        {
            get { return _red; }
            set
            {
                _red = (byte)(value & 0x1F);
            }
        }
        public int Green
        {
            get { return _green; }
            set
            {
                _green = (byte)(value & 0x1F);
            }
        }
        public int Blue
        {
            get { return _blue; }
            set
            {
                _blue = (byte)(value & 0x1F);
            }
        }

        public ushort Color
        {
            get
            {
                ushort color = 0;
                color |= (ushort)Blue;
                color <<= 5;
                color |= (ushort)Green;
                color <<= 5;
                color |= (ushort)Red;
                return color;
            }
            set
            {
                Red = value & 0x1F;
                value >>= 5;
                Green = value & 0x1F;
                value >>= 5;
                Blue = value & 0x1F;
            }
        }

        public GBColor(ushort color)
        {
            Color = color;
        }

        public GBColor(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static GBColor FromValue(int value)
        {
            return new GBColor(value, value, value);
        }

        public void SetHighByte(byte value)
        {
            Color = Color.SetHighByte(value);
        }

        public void SetLowByte(byte value)
        {
            Color = Color.SetLowByte(value);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_red);
            writer.Write(_green);
            writer.Write(_blue);
        }

        public void SetState(BinaryReader reader)
        {
            _red = reader.ReadByte();
            _green = reader.ReadByte();
            _blue = reader.ReadByte();
        }
    }
}
