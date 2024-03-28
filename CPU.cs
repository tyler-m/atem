using System;

namespace Atem
{
    internal struct CPURegisters
    {
        public byte IR;
        public byte A, B, C, D, E, H, L;
        public ushort PC, SP;
        public byte W, Z;

        public ushort BC
        {
            get { return ((ushort)0).SetHighByte(B).SetLowByte(C); }
            set
            {
                B = value.GetHighByte();
                C = value.GetLowByte();
            }
        }

        public ushort DE
        {
            get { return ((ushort)0).SetHighByte(D).SetLowByte(E); }
            set
            {
                D = value.GetHighByte();
                E = value.GetLowByte();
            }
        }

        public ushort HL
        {
            get { return ((ushort)0).SetHighByte(H).SetLowByte(L); }
            set
            {
                H = value.GetHighByte();
                L = value.GetLowByte();
            }
        }

        public ushort WZ
        {
            get { return ((ushort)0).SetHighByte(W).SetLowByte(Z); }
            set
            {
                W = value.GetHighByte();
                Z = value.GetLowByte();
            }
        }
    }

    internal class CPU
    {
        private CPURegisters _registers;
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
