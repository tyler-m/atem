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
        private bool _opCondition;

        public CPURegisters Registers
        {
            get
            {
                return _registers;
            }
        }

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

        public bool Clock()
        {
            byte opcode = _registers.IR;

            if (opcode == 0x00) // NOP
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                }
            }
            else if (opcode == 0x06) // LD B,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.B = _registers.Z;
                }
            }
            else if (opcode == 0x0C) // INC C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.C.WillCarry(1);
                    _registers.Flags.N = false;
                    _registers.C++;
                    _registers.Flags.Z = _registers.C == 0;
                }
            }
            else if (opcode == 0x0E) // LD C,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.C = _registers.Z;
                }
            }
            else if (opcode == 0x11) // LD DE,u16
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
                    _registers.DE = _registers.WZ;
                }
            }
            else if (opcode == 0x17) // RLA
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    bool carry = _registers.A.GetBit(7);
                    _registers.A = (byte)(_registers.A << 1);
                    _registers.A = _registers.A.SetBit(0, _registers.Flags.C);
                    _registers.Flags.C = carry;
                    _registers.Flags.H = false;
                    _registers.Flags.N = false;
                    _registers.Flags.Z = false; // ?
                }
            }
            else if (opcode == 0x1A) // LD A,(DE)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.DE);
                }
                else if (_opCycle == 1)
                {
                    _registers.A = _registers.Z;
                }
            }
            else if (opcode == 0x20) // JR NZ,s8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                    _opCondition = !_registers.Flags.Z;
                }
                else if (_opCycle == 1)
                {
                    if (_opCondition)
                    {
                        _opLength = 3;
                        int offset = (sbyte)_registers.Z;
                        _registers.PC = (ushort)(_registers.PC + offset);
                    }
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
            else if (opcode == 0x3E) // LD A,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.A = _registers.Z;
                }
            }
            else if (opcode == 0x4F) // LD C,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.C = _registers.A;
                }
            }
            else if (opcode == 0x77) // LD (HL),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    Write(_registers.HL, _registers.A);
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
            else if (opcode == 0xC1) // POP BC
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.SP++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.SP++);
                }
                else if (_opCycle == 2)
                {
                    _registers.BC = _registers.WZ;
                }
            }
            else if (opcode == 0xC5) // PUSH BC
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.B);
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.C);
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

                    if (opcode == 0x11) // RL C
                    {
                        if (_opCycle == 1)
                        {
                            _opLength = 2;
                            bool carry = _registers.C.GetBit(7);
                            _registers.C = (byte)(_registers.C << 1);
                            _registers.C = _registers.C.SetBit(0, _registers.Flags.C);
                            _registers.Flags.C = carry;
                            _registers.Flags.H = false;
                            _registers.Flags.N = false;
                            _registers.Flags.Z = _registers.C == 0;
                        }
                    }
                    else if (opcode == 0x7C) // BIT 7,H
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
            else if (opcode == 0xCD) // CALL u16
            {
                if (_opCycle == 0)
                {
                    _opLength = 6;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    _registers.SP--;
                }
                else if (_opCycle == 3)
                {
                    Write(_registers.SP, _registers.PC.GetHighByte());
                    _registers.SP--;
                }
                else if (_opCycle == 4)
                {
                    Write(_registers.SP, _registers.PC.GetLowByte());
                    _registers.PC = _registers.WZ;
                }
            }
            else if (opcode == 0xE0) // LD (FF00+u8),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    Write((ushort)(0xFF00 | _registers.Z), _registers.A);
                }
            }
            else if (opcode == 0xE2) // LD (FF00+C),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    Write((ushort)(0xFF00 | _registers.C), _registers.A);
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

            return _opCycle == 0;
        }
    }
}
