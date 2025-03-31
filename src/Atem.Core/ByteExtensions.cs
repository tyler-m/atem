
namespace Atem.Core
{
    internal static class ByteExtensions
    {
        public static int Int(this bool b)
        {
            return b ? 1 : 0;
        }
        public static byte Byte(this bool b)
        {
            return (byte)(b ? 1 : 0);
        }

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

        public static bool WillCarry(this ushort u, int value)
        {
            return u + value > ushort.MaxValue;
        }

        public static bool WillBorrow(this ushort u, int value)
        {
            return value > u;
        }

        public static bool WillHalfCarry(this ushort u, int value)
        {
            return (((u & 0xFFF) + (value & 0xFFF)) & 0x1000) == 0x1000;
        }

        public static bool WillHalfBorrow(this ushort u, int value)
        {
            return (((u & 0xFFF) - (value & 0xFFF)) & 0x1000) == 0x1000;
        }

        public static bool WillCarry(this byte b, int value)
        {
            return (b & 0xFF) + (value & 0xFF) > byte.MaxValue;
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
            return (byte)us;
        }

        public static byte GetLowNibble(this byte b)
        {
            return (byte)(b & 0b00001111);
        }

        public static byte GetHighNibble(this byte b)
        {
            return (byte)((b & 0b11110000) >> 4);
        }

        public static byte SwapNibbles(this byte b)
        {
            return (byte)((b >> 4) | (b << 4));
        }
        public static bool GetBit(this ushort u, int index)
        {
            return (u & (1 << index)) != 0;
        }

        public static ushort SetBit(this ushort u, int index)
        {
            return (ushort)(u | (1 << index));
        }

        public static ushort ClearBit(this ushort u, int index)
        {
            return (ushort)(u & ~(1 << index));
        }

        public static ushort SetBit(this ushort u, int index, bool value)
        {
            if (value)
            {
                return SetBit(u, index);
            }
            else
            {
                return ClearBit(u, index);
            }
        }
    }
}
