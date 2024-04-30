
namespace Atem.Core.Graphics
{
    internal class Palette
    {
        private byte _value;

        public Palette(byte value)
        {
            _value = value;
        }

        public byte this[int i]
        {
            get
            {
                return (byte)((_value >> (2 * i)) & 0b11);
            }
        }

        public static implicit operator byte(Palette palette)
        {
            return palette._value;
        }

        public static implicit operator Palette(byte b)
        {
            return new Palette(b);
        }
    }
}
