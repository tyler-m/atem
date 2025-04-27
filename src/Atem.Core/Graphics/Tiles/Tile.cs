using System;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Tiles
{
    public class Tile : IStateful
    {
        private readonly byte[] _data = new byte[16];

        public byte GetByte(int index)
        {
            return _data[index];
        }

        public void SetByte(int index, byte value)
        {
            if ((uint)index > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _data[index] = value;
        }

        public byte GetPixel(int x, int y, bool flipX = false, bool flipY = false)
        {
            if ((uint)x > 7 || (uint)y > 7)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (flipX)
            {
                x = 7 - x;
            }

            if (flipY)
            {
                y = 7 - y;
            }

            int rowOffset = y * 2;
            int shift = 7 - x;

            byte lowByte = _data[rowOffset];
            byte highByte = _data[rowOffset + 1];

            byte lowBit = (byte)((lowByte >> shift) & 1);
            byte highBit = (byte)((highByte >> shift) & 1);
            return (byte)((highBit << 1) | lowBit);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_data);
        }

        public void SetState(BinaryReader reader)
        {
            reader.ReadBytes(_data.Length).CopyTo(_data, 0);
        }
    }
}