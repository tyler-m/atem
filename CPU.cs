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
            byte opcode = _registers.IR;

            if (opcode == 0x00) // NOP
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                }
            }
            else if (opcode == 0x21) // LD HL,u16
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    _registers.HL = _registers.WZ;
                }
            }
            else if (opcode == 0x31) // LD SP,u16
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    _registers.SP = _registers.WZ;
                }
            }
            else if (opcode == 0x32) // LD (HL-),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    Write(_registers.HL--, _registers.A);
                }
            }
            else if (opcode == 0xAF) // XOR A,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A &= _registers.A;
                    _registers.Flags.C = false;
                    _registers.Flags.N = false;
                    _registers.Flags.H = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xCB) // CB
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else // handle CB prefixed opcode
                {
                    opcode = _registers.Z;

                    if (opcode == 0x7C) // BIT 7,H
                    {
                        if (_opCycle == 1)
                        {
                            _opLength = 2;
                            _registers.Flags.H = true;
                            _registers.Flags.N = false;
                            _registers.Flags.Z = !_registers.H.GetBit(7);
                        }
                    }
                    else
                    {
                        throw new Exception($"Unhandled CB prefixed opcode 0x{opcode:X2}.");
                    }
                }
            }
            else
            {
                throw new Exception($"Unhandled opcode 0x{opcode:X2}.");
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
