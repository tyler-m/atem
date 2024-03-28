
namespace Atem
{
    internal static class ByteExtensions
    {
        public static bool GetBit(this byte b, int index)
        {
            return (b & (1 << index)) != 0;
        }

        public static byte SetBit(this byte b, int index)
        {
            return (byte)(b | (1 << index));
        }

        public static byte ClearBit(this byte b, int index)
        {
            return (byte)(b & ~(1 << index));
        }

        public static byte SetBit(this byte b, int index, bool value)
        {
            if (value)
            {
                return SetBit(b, index);
            }
            else
            {
                return ClearBit(b, index);
            }
        }

        public static bool WillCarry(this byte b, int value)
        {
            return b + value > byte.MaxValue;
        }

        public static bool WillBorrow(this byte b, int value)
        {
            return value > b;
        }

        public static bool WillHalfCarry(this byte b, int value)
        {
            return (((b & 0xF) + (value & 0xF)) & 0x10) == 0x10;
        }

        public static bool WillHalfBorrow(this byte b, int value)
        {
            return (((b & 0xF) - (value & 0xF)) & 0x10) == 0x10;
        }

        public static ushort SetLowByte(this ushort b, byte value)
        {
            return (ushort)((b & 0xFF00) | value);
        }

        public static ushort SetHighByte(this ushort b, byte value)
        {
            return (ushort)((b & 0x00FF) | (value << 8));
        }

        public static byte GetHighByte(this ushort us)
        {
            return (byte)(us >> 8);
        }

        public static byte GetLowByte(this ushort us)
        {
            return (byte)(us & 0xFF);
        }
    }
}
