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
        public bool IME;
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

        public ushort AddressOfOperationBeingExecuted { get; set; }

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
            ushort pc = _registers.PC;

            if (opcode == 0x00) // NOP
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                }
            }
            else if (opcode == 0x01) // LD BC,u16
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
                    _registers.BC = _registers.WZ;
                }
            }
            else if (opcode == 0x04) // INC B
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.B.WillCarry(1);
                    _registers.Flags.N = false;
                    _registers.B++;
                    _registers.Flags.Z = _registers.B == 0;
                }
            }
            else if (opcode == 0x05) // DEC B
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.B.WillHalfBorrow(1);
                    _registers.Flags.N = true;
                    _registers.B--;
                    _registers.Flags.Z = _registers.B == 0;
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
            else if (opcode == 0x0B) // DEC BC
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.BC--;
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
            else if (opcode == 0x0D) // DEC C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.C.WillHalfBorrow(1);
                    _registers.Flags.N = true;
                    _registers.C--;
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
            else if (opcode == 0x12) // LD (DE),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    Write(_registers.DE, _registers.A);
                }
            }
            else if (opcode == 0x13) // INC DE
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.DE++;
                }
            }
            else if (opcode == 0x15) // DEC D
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.D.WillHalfBorrow(1);
                    _registers.Flags.N = true;
                    _registers.D--;
                    _registers.Flags.Z = _registers.D == 0;
                }
            }
            else if (opcode == 0x16) // LD D,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.D = _registers.Z;
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
            else if (opcode == 0x18) // JR s8
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {

                }
                else if (_opCycle == 2)
                {
                    int offset = (sbyte)_registers.Z;
                    _registers.PC = (ushort)(_registers.PC + offset);
                }
            }
            else if (opcode == 0x19) // ADD HL,DE
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Flags.N = false;
                    _registers.Flags.H = _registers.L.WillHalfCarry(_registers.E);
                    _registers.Flags.C = _registers.L.WillCarry(_registers.E);
                    _registers.L += _registers.E;
                }
                else if (_opCycle == 1)
                {
                    _registers.Flags.N = false;
                    int valueToAdd = _registers.D + (_registers.Flags.C ? 1 : 0);
                    _registers.Flags.H = _registers.H.WillHalfCarry(valueToAdd);
                    _registers.Flags.C = _registers.H.WillCarry(valueToAdd);
                    _registers.H += (byte)valueToAdd;
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
            else if (opcode == 0x1C) // INC E
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.E.WillCarry(1);
                    _registers.Flags.N = false;
                    _registers.E++;
                    _registers.Flags.Z = _registers.E == 0;
                }
            }
            else if (opcode == 0x1D) // DEC E
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.E.WillHalfBorrow(1);
                    _registers.Flags.N = true;
                    _registers.E--;
                    _registers.Flags.Z = _registers.E == 0;
                }
            }
            else if (opcode == 0x1E) // LD E,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.E = _registers.Z;
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
            else if (opcode == 0x22) // LD (HL+),A
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    Write(_registers.HL++, _registers.A);
                }
            }
            else if (opcode == 0x23) // INC HL
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.HL++;
                }
            }
            else if (opcode == 0x24) // INC H
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.H.WillCarry(1);
                    _registers.Flags.N = false;
                    _registers.H++;
                    _registers.Flags.Z = _registers.H == 0;
                }
            }
            else if (opcode == 0x28) // JR Z,s8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                    _opCondition = _registers.Flags.Z;
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
            else if (opcode == 0x2A) // LD A,(HL+)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.HL++);
                }
                else if (_opCycle == 1)
                {
                    _registers.A = _registers.Z;
                }
            }
            else if (opcode == 0x2E) // LD L,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.L = _registers.Z;
                }
            }
            else if (opcode == 0x2F) // CPL
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = (byte)~_registers.A;
                    _registers.Flags.H = true;
                    _registers.Flags.N = true;
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
            else if (opcode == 0x36) // LD (HL),u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                if (_opCycle == 1)
                {
                    Write(_registers.HL, _registers.Z);
                }
            }
            else if (opcode == 0x3D) // DEC A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.H = _registers.A.WillHalfBorrow(1);
                    _registers.Flags.N = true;
                    _registers.A--;
                    _registers.Flags.Z = _registers.A == 0;
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
            else if (opcode == 0x47) // LD B,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.B = _registers.A;
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
            else if (opcode == 0x56) // LD D,(HL)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.HL);
                }
                else if (_opCycle == 1)
                {
                    _registers.D = _registers.Z;
                }
            }
            else if (opcode == 0x57) // LD D,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.D = _registers.A;
                }
            }
            else if (opcode == 0x5E) // LD E,(HL)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.HL);
                }
                else if (_opCycle == 1)
                {
                    _registers.E = _registers.Z;
                }
            }
            else if (opcode == 0x5F) // LD E,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.E = _registers.A;
                }
            }
            else if (opcode == 0x67) // LD H,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.H = _registers.A;
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
            else if (opcode == 0x78) // LD A,B
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = _registers.B;
                }
            }
            else if (opcode == 0x79) // LD A,C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = _registers.C;
                }
            }
            else if (opcode == 0x7B) // LD A,E
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = _registers.E;
                }
            }
            else if (opcode == 0x7C) // LD A,H
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = _registers.H;
                }
            }
            else if (opcode == 0x7D) // LD A,L
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A = _registers.L;
                }
            }
            else if (opcode == 0x86) // Add A,(HL)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.HL);
                }
                else if (_opCycle == 1)
                {
                    _registers.Flags.C = _registers.A.WillCarry(_registers.Z);
                    _registers.Flags.H = _registers.A.WillHalfCarry(_registers.Z);
                    _registers.Flags.N = false;
                    _registers.A += _registers.Z;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0x87) // ADD A,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.C = _registers.A.WillCarry(_registers.A);
                    _registers.Flags.H = _registers.A.WillHalfCarry(_registers.A);
                    _registers.Flags.N = false;
                    _registers.A += _registers.A;
                    _registers.Flags.Z = _registers.A == 0;

                }
            }
            else if (opcode == 0x90) // SUB A,B
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.Flags.C = _registers.A.WillBorrow(_registers.B);
                    _registers.Flags.H = _registers.A.WillHalfBorrow(_registers.B);
                    _registers.Flags.N = true;
                    _registers.A -= _registers.B;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xA1) // AND A,C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A &= _registers.C;
                    _registers.Flags.Z = _registers.A == 0;
                    _registers.Flags.N = false;
                    _registers.Flags.H = true;
                    _registers.Flags.C = false;
                }
            }
            else if (opcode == 0xA7) // AND A,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A &= _registers.A;
                    _registers.Flags.Z = _registers.A == 0;
                    _registers.Flags.N = false;
                    _registers.Flags.H = true;
                    _registers.Flags.C = false;
                }
            }
            else if (opcode == 0xA9) // XOR A,C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A ^= _registers.C;
                    _registers.Flags.C = false;
                    _registers.Flags.N = false;
                    _registers.Flags.H = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xAF) // XOR A,A
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A ^= _registers.A;
                    _registers.Flags.C = false;
                    _registers.Flags.N = false;
                    _registers.Flags.H = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xB0) // OR A,B
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A |= _registers.B;
                    _registers.Flags.C = false;
                    _registers.Flags.H = false;
                    _registers.Flags.N = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xB1) // OR A,C
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.A |= _registers.C;
                    _registers.Flags.C = false;
                    _registers.Flags.H = false;
                    _registers.Flags.N = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xBE) // CP A,(HL)
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.HL);
                }
                else if (_opCycle == 1)
                {
                    _registers.Flags.C = _registers.A.WillBorrow(_registers.Z);
                    _registers.Flags.H = _registers.A.WillHalfBorrow(_registers.Z);
                    _registers.Flags.N = true;
                    _registers.Flags.Z = _registers.A == _registers.Z;
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
            else if (opcode == 0xC3) // JP u16
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    _registers.PC = _registers.WZ;
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
                    else if (opcode == 0x37) // SWAP A
                    {
                        if (_opCycle == 1)
                        {
                            _opLength = 2;
                            _registers.A = _registers.A.SwapNibbles();
                            _registers.Flags.Z = _registers.A == 0;
                            _registers.Flags.N = false;
                            _registers.Flags.H = false;
                            _registers.Flags.C = false;
                        }
                    }
                    else if (opcode == 0x87) // RES 0,A
                    {
                        if (_opCycle == 1)
                        {
                            _opLength = 2;
                            _registers.A = _registers.A.ClearBit(0);
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
            else if (opcode == 0xC9) // RET
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.Z = Read(_registers.SP++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.SP++);
                }
                else if (_opCycle == 2)
                {
                    _registers.PC = _registers.WZ;
                }
            }
            else if (opcode == 0xD1) // POP DE
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
                    _registers.DE = _registers.WZ;
                }
            }
            else if (opcode == 0xD5) // PUSH DE
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.D);
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.E);
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
            else if (opcode == 0xE1) // POP HL
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
                    _registers.HL = _registers.WZ;
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
            else if (opcode == 0xE5) // PUSH HL
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.H);
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.L);
                }
            }
            else if (opcode == 0xE6) // AND u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.A &= _registers.Z;
                    _registers.Flags.C = false;
                    _registers.Flags.H = true;
                    _registers.Flags.N = false;
                    _registers.Flags.Z = _registers.A == 0;
                }
            }
            else if (opcode == 0xE9) // JP HL
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.PC = _registers.HL;
                }
            }
            else if (opcode == 0xEA) // LD (u16),A
            {
                if(_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.WZ, _registers.A);
                }
            }
            else if (opcode == 0xEF) // RST 28h
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.PC.GetHighByte());
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.PC.GetLowByte());
                    _registers.PC = 0x0028;
                }
            }
            else if (opcode == 0xF0) // LD A,(FF00+u8)
            {
                if (_opCycle == 0)
                {
                    _opLength = 3;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.Z = Read((ushort)(0xFF00 | _registers.Z));
                }
                else if (_opCycle == 2)
                {
                    _registers.A = _registers.Z;
                }
            }
            else if (opcode == 0xF3) // DI
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.IME = false;
                }
            }
            else if (opcode == 0xF5) // PUSH AF
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.A);
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.Flags.F);
                }
            }
            else if (opcode == 0xFA) // LD A,(u16)
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.W = Read(_registers.PC++);
                }
                else if (_opCycle == 2)
                {
                    _registers.Z = Read(_registers.WZ);
                }
                else if (_opCycle == 3)
                {
                    _registers.A = _registers.Z;
                }
            }
            else if (opcode == 0xFB) // EI
            {
                if (_opCycle == 0)
                {
                    _opLength = 1;
                    _registers.IME = true;
                }
            }
            else if (opcode == 0xFE) // CP A,u8
            {
                if (_opCycle == 0)
                {
                    _opLength = 2;
                    _registers.Z = Read(_registers.PC++);
                }
                else if (_opCycle == 1)
                {
                    _registers.Flags.C = _registers.A.WillBorrow(_registers.Z);
                    _registers.Flags.H = _registers.A.WillHalfBorrow(_registers.Z);
                    _registers.Flags.N = true;
                    _registers.Flags.Z = _registers.A == _registers.Z;
                }
            }
            else if (opcode == 0xFF) // RST 38h
            {
                if (_opCycle == 0)
                {
                    _opLength = 4;
                    _registers.SP--;
                }
                else if (_opCycle == 1)
                {
                    Write(_registers.SP, _registers.PC.GetHighByte());
                    _registers.SP--;
                }
                else if (_opCycle == 2)
                {
                    Write(_registers.SP, _registers.PC.GetLowByte());
                    _registers.PC = 0x0038;
                }
            }
            else
            {
                throw new Exception($"Unhandled opcode 0x{opcode:X2}.");
            }

            if (_opCycle == 0)
            {
                AddressOfOperationBeingExecuted = pc;
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
