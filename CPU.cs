using System;

namespace Atem
{
    internal class CPU
    {
        private Bus _bus;

        public CPU(Bus bus)
        {
            _bus = bus;
        }

        private byte Read(ushort address)
        {
            return _bus.Read(address);
        }

        private void Write(ushort address, byte value)
        {
            _bus.Write(address, value);
        }

        public void Clock()
        {

        }
    }
}
