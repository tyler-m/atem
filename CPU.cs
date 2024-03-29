using System;

namespace Atem
{
    internal struct CPUFlags
    {
        public byte F;

        public bool Z
        {
            get { return F.GetBit(7); }
            set { F = F.SetBit(7, value); }
        }

        public bool N
        {
            get { return F.GetBit(6); }
            set { F = F.SetBit(6, value); }
        }

        public bool H
        {
            get { return F.GetBit(5); }
            set { F = F.SetBit(5, value); }
        }

        public bool C
        {
            get { return F.GetBit(4); }
            set { F = F.SetBit(4, value); }
        }
    }

    internal struct CPURegisters
    {
        public byte IR;
        public byte A, B, C, D, E, H, L;
        public ushort PC, SP;
        public byte W, Z;
        public CPUFlags Flags;

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

        private int _opCycle;
        private int _opLength;

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
            byte opCode = _registers.IR;

            if (opCode == 0x00) // NOP
            {
                _opLength = 1;
            }

            _opCycle++;
            if (_opCycle == _opLength)
            {
                _registers.IR = Read(_registers.PC++);
                _opCycle = 0;
            }
        }
    }
}
