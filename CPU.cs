using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Numerics;

namespace Atem
{
    internal class CPU
    {
        private Bus _bus;
        public CPURegisters Registers;
        private Dictionary<byte, (OperationFunc Method, OperationInfo Info)> _operations = new Dictionary<byte, (OperationFunc, OperationInfo)>();
        private Dictionary<byte, (OperationFunc Method, OperationInfo Info)> _operationsCB = new Dictionary<byte, (OperationFunc, OperationInfo)>();

        public byte IR;
        public int Length = 0;
        public bool CB = false;
        public bool IME = false;

        private bool _operationFinished = true;
        private int _tick = 0;

        private ushort[] _interruptJumps = new ushort[5] { 0x0040, 0x0048, 0x0050, 0x0058, 0x0060 };
        private int _interruptType;

        public ushort AddressOfNextOperation
        {
            get
            {
                return Registers.PC;
            }
        }

        public CPU(Bus bus)
        {
            _bus = bus;
            BuildOperationList();
        }

        private delegate void OperationFunc(OperationInfo opInfo);

        private void BuildOperationList()
        {
            OperationInfo[] operations = Opcodes.List.Split("\r\n").Select(s => new OperationInfo(s)).ToArray();
            for (int i = 0; i < operations.Length; i++)
            {
                OperationInfo opInfo = operations[i];
                MethodInfo? methodInfo = typeof(CPU).GetMethod(opInfo.Name);
                if (methodInfo != null)
                {
                    if (i <= 0xFF)
                    {
                        _operations.Add((byte)i, ((OperationFunc)Delegate.CreateDelegate(typeof(OperationFunc), this, methodInfo), opInfo));
                    }
                    else
                    {
                        _operationsCB.Add((byte)i, ((OperationFunc)Delegate.CreateDelegate(typeof(OperationFunc), this, methodInfo), opInfo));
                    }
                }
                else
                {
                    throw new Exception("Unrecognized operation function.");
                }
            }
        }

        private byte ReadByte()
        {
            return ReadBus(Registers.PC++);
        }

        private ushort ReadWord()
        {
            byte low = ReadByte();
            byte high = ReadByte();
            return (ushort)(high << 8 | low);
        }

        private byte PopByte()
        {
            return ReadBus(Registers.SP++);
        }

        private ushort PopWord()
        {
            byte low = PopByte();
            byte high = PopByte();
            return (ushort)(high << 8 | low);
        }

        private void PushByte(byte value)
        {
            WriteBus(--Registers.SP, value);
        }

        private void PushWord(ushort value)
        {
            PushByte(value.GetHighByte());
            PushByte(value.GetLowByte());
        }

        private byte ReadBus(ushort address)
        {
            return _bus.Read(address);
        }

        private void WriteBus(ushort address, byte value)
        {
            _bus.Write(address, value);
        }

        public bool Clock()
        {
            if (_operationFinished)
            {
                _tick = 0;
                _operationFinished = false;

                if (IME)
                {
                    byte IE = _bus.Read(0xFFFF);
                    byte IF = _bus.Read(0xFF0F);
                    int i = BitOperations.TrailingZeroCount(IE & IF);

                    if (i < 5)
                    {
                        IME = false;
                        _bus.Write(0xFF0F, IF.ClearBit(i));
                        _operationFinished = false;
                        _interruptType = i;
                        Length = 5;
                        PushWord(Registers.PC);
                        Registers.PC = _interruptJumps[_interruptType];
                        _tick++;
                        return false;
                    }
                }
                
                IR = ReadByte();
                if (!CB)
                {
                    var operation = _operations[IR];
                    operation.Method.Invoke(operation.Info);
                }
            }

            if (CB && _tick == 1)
            {
                IR = ReadByte();
                var operation = _operationsCB[IR];
                operation.Method.Invoke(operation.Info);
                CB = false;
            }

            _tick++;
            return _operationFinished = _tick >= Length && !CB;
        }

        public void NOP(OperationInfo opInfo) => Nop();
        public void LD(OperationInfo opInfo) => Load(opInfo.Operand1, opInfo.Operand2);
        public void INC(OperationInfo opInfo) => Inc(opInfo.Operand1);
        public void DEC(OperationInfo opInfo) => Dec(opInfo.Operand1);
        public void RLCA(OperationInfo opInfo) => Rlca();
        public void ADD(OperationInfo opInfo) => Add(opInfo.Operand1, opInfo.Operand2);
        public void RRCA(OperationInfo opInfo) => Rrca();
        public void STOP(OperationInfo opInfo) => Stop();
        public void RLA(OperationInfo opInfo) => Rla();
        public void JR(OperationInfo opInfo) => Jr(opInfo.Operands.Length == 2 ? opInfo.Operand1 : string.Empty);
        public void RRA(OperationInfo opInfo) => Rra();
        public void DAA(OperationInfo opInfo) => Daa();
        public void CPL(OperationInfo opInfo) => Cpl();
        public void SCF(OperationInfo opInfo) => Scf();
        public void CCF(OperationInfo opInfo) => Ccf();
        public void HALT(OperationInfo opInfo) => Halt();
        public void ADC(OperationInfo opInfo) => Adc(opInfo.Operand1, opInfo.Operand2);
        public void SUB(OperationInfo opInfo) => Sub(opInfo.Operand1, opInfo.Operand2);
        public void SBC(OperationInfo opInfo) => Sbc(opInfo.Operand1, opInfo.Operand2);
        public void AND(OperationInfo opInfo) => And(opInfo.Operand1, opInfo.Operand2);
        public void XOR(OperationInfo opInfo) => Xor(opInfo.Operand1, opInfo.Operand2);
        public void OR(OperationInfo opInfo) => Or(opInfo.Operand1, opInfo.Operand2);
        public void CP(OperationInfo opInfo) => Cp(opInfo.Operand1, opInfo.Operand2);
        public void RET(OperationInfo opInfo) => Ret(opInfo.Operands.Length == 1 ? opInfo.Operand1 : string.Empty);
        public void POP(OperationInfo opInfo) => Pop(opInfo.Operand1);

        public void JP(OperationInfo opInfo)
        {
            if (opInfo.Operands.Length == 1)
            {
                Jp(string.Empty, opInfo.Operand1);
            }
            else
            {
                Jp(opInfo.Operand1, opInfo.Operand2);
            }
        }

        public void CALL(OperationInfo opInfo)
        {
            if (opInfo.Operands.Length == 1)
            {
                Call(string.Empty, opInfo.Operand1);
            }
            else
            {
                Call(opInfo.Operand1, opInfo.Operand2);
            }
        }

        public void PUSH(OperationInfo opInfo) => Push(opInfo.Operand1);
        public void RST(OperationInfo opInfo) => Rst(opInfo.Operand1);
        public void PREFIX(OperationInfo opInfo) => Prefix();
        public void RETI(OperationInfo opInfo) => Reti();
        public void DI(OperationInfo opInfo) => Di();
        public void EI(OperationInfo opInfo) => Ei();
        public void RLC(OperationInfo opInfo) => Rlc(opInfo.Operand1);
        public void RRC(OperationInfo opInfo) => Rrc(opInfo.Operand1);
        public void RL(OperationInfo opInfo) => Rl(opInfo.Operand1);
        public void RR(OperationInfo opInfo) => Rr(opInfo.Operand1);
        public void SLA(OperationInfo opInfo) => Sla(opInfo.Operand1);
        public void SRA(OperationInfo opInfo) => Sra(opInfo.Operand1);
        public void SWAP(OperationInfo opInfo) => Swap(opInfo.Operand1);
        public void SRL(OperationInfo opInfo) => Srl(opInfo.Operand1);
        public void BIT(OperationInfo opInfo) => Bit(opInfo.Operand1, opInfo.Operand2);
        public void RES(OperationInfo opInfo) => Res(opInfo.Operand1, opInfo.Operand2);
        public void SET(OperationInfo opInfo) => Set(opInfo.Operand1, opInfo.Operand2);

        public (ushort Value, bool Word) GetValue(string source)
        {
            LocationType sourceType = source.ToLocationType();
            ushort value = 0;
            bool word = false;
            if (sourceType == LocationType.Register) // A
            {
                value = Registers[source];
            }
            else if (sourceType == LocationType.WordRegister) // HL
            {
                value = Registers[source];
                word = true;
            }
            else if (sourceType == LocationType.Direct) // u8
            {
                Length += 1;
                value = ReadByte();
            }
            else if (sourceType == LocationType.DirectWord) // u16
            {
                Length += 1;
                byte low = ReadByte();
                byte high = ReadByte();
                value = (ushort)(high << 8 | low);
                word = true;
            }
            else if (sourceType == LocationType.IndirectWordRegister) // (BC)
            {
                Length += 1;
                string register = source.Substring(1, source.Length - 2);
                value = ReadBus(Registers[register]);
            }
            else if (sourceType == LocationType.IndirectIncrement) // (HL+)
            {
                Length += 1;
                string register = source.Substring(1, source.Length - 3);
                value = ReadBus(Registers[register]);
                Registers[register]++;
            }
            else if (sourceType == LocationType.IndirectDecrement) // (HL-)
            {
                Length += 1;
                string register = source.Substring(1, source.Length - 3);
                value = ReadBus(Registers[register]);
                Registers[register]--;
            }
            else if (sourceType == LocationType.IndirectOffset) // (FF00+u8)
            {
                Length += 2;
                byte offset = ReadByte();
                value = ReadBus((ushort)(0xFF00 + offset));
            }
            else if (sourceType == LocationType.IndirectOffsetRegister) // (FF00+C)
            {
                Length += 1;
                string register = source.Substring(6, 1);
                value = ReadBus((ushort)(0xFF00 + Registers[register]));
            }
            else if (sourceType == LocationType.RegisterOffset) // SP+s8
            {
                Length += 2;
                string register = source.Substring(0, 2);
                Registers.Flags.Z = false;
                Registers.Flags.N = false;
                value = Registers[register];
                sbyte offset = (sbyte)ReadByte();
                Registers.Flags.H = value.GetLowByte().WillHalfCarry(offset);
                Registers.Flags.C = value.GetLowByte().WillCarry(offset);
                if (offset >= 0)
                {
                    value += (byte)offset;
                }
                else
                {
                    value -= (byte)Math.Abs(offset);
                }
            }
            else if (sourceType == LocationType.Indirect) // (u16)
            {
                Length += 3;
                ushort address = ReadWord();
                value = ReadBus(address);
            }
            else if (sourceType == LocationType.DirectSigned) // s8
            {
                Length += 2;
                value = ReadByte();
            }
            return (value, word);
        }

        public void SetValue(string dest, (ushort Value, bool Word) value)
        {
            LocationType destType = dest.ToLocationType();
            if (destType == LocationType.Register) // A
            {
                Registers[dest] = value.Value;
            }
            else if (destType == LocationType.WordRegister) // BC
            {
                if (value.Word)
                {
                    Length += 1;
                }
                Registers[dest] = value.Value;
            }
            else if (destType == LocationType.IndirectWordRegister) // (BC)
            {
                Length += 1;
                string register = dest.Substring(1, dest.Length - 2);
                WriteBus(Registers[register], value.Value.GetLowByte());
            }
            else if (destType == LocationType.Indirect) // (u16)
            {
                Length += 3;
                byte low = ReadByte();
                byte high = ReadByte();
                ushort address = (ushort)(high << 8 | low);
                WriteBus(address, value.Value.GetLowByte());

                if (value.Word) // e.g. LD (u16),SP
                {
                    Length += 1;
                    address++;
                    WriteBus(address, value.Value.GetHighByte());
                }
            }
            else if (destType == LocationType.IndirectIncrement) // (HL+)
            {
                Length += 1;
                string register = dest.Substring(1, dest.Length - 3);
                WriteBus(Registers[register], value.Value.GetLowByte());
                Registers[register]++;
            }
            else if (destType == LocationType.IndirectDecrement) // (HL-)
            {
                Length += 1;
                string register = dest.Substring(1, dest.Length - 3);
                WriteBus(Registers[register], value.Value.GetLowByte());
                Registers[register]--;
            }
            else if (destType == LocationType.IndirectOffset) // (FF00+u8)
            {
                Length += 2;
                byte offset = ReadByte();
                WriteBus((ushort)(0xFF00 + offset), value.Value.GetLowByte());
            }
            else if (destType == LocationType.IndirectOffsetRegister) // (FF00+C)
            {
                Length += 1;
                string register = dest.Substring(6, 1);
                WriteBus((ushort)(0xFF00 + Registers[register]), value.Value.GetLowByte());
            }
        }

        public void Nop()
        {
            Length = 1;
        }

        public void Load(string dest, string source)
        {
            Length = 1;
            SetValue(dest, GetValue(source));
        }

        public void Dec(string dest)
        {
            Length = 1;
            var value = GetValue(dest);
            if (!value.Word)
            {
                Registers.Flags.Z = (byte)(value.Value - 1) == 0;
                Registers.Flags.N = true;
                Registers.Flags.H = value.Value.GetLowByte().WillHalfBorrow(1);
            }
            value.Value--;
            SetValue(dest, value);
        }

        public void Inc(string dest)
        {
            Length = 1;
            var value = GetValue(dest);
            if (!value.Word)
            {
                Registers.Flags.Z = (byte)(value.Value + 1) == 0;
                Registers.Flags.N = false;
                Registers.Flags.H = value.Value.GetLowByte().WillHalfCarry(1);
            }
            value.Value++;
            SetValue(dest, value);
        }

        public void Rlca()
        {
            Length = 1;
            Registers.Flags.Z = false;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = Registers.A.GetBit(7);
            Registers.A = (byte)(Registers.A << 1);
            Registers.A = Registers.A.SetBit(0, Registers.Flags.C);
        }

        public void Add(string dest, string source)
        {
            Length = 1;
            LocationType destType = dest.ToLocationType();
            LocationType sourceType = source.ToLocationType();
            ushort value = GetValue(source).Value;
            Registers.Flags.N = false;

            if (destType == LocationType.Register)
            {
                Registers.Flags.H = Registers[dest].GetLowByte().WillHalfCarry(value);
                Registers.Flags.C = Registers[dest].GetLowByte().WillCarry(value);
                Registers[dest] = (byte)(Registers[dest] + value);
                Registers.Flags.Z = Registers[dest] == 0;
            }
            else if (destType == LocationType.WordRegister)
            {
                Length += 1;
                if (sourceType == LocationType.WordRegister)
                {
                    Registers.Flags.H = Registers[dest].WillHalfCarry(value);
                    Registers.Flags.C = Registers[dest].WillCarry(value);
                    Registers[dest] = (ushort)(Registers[dest] + value);
                }
                else if (sourceType == LocationType.DirectSigned)
                {
                    Registers.Flags.Z = false;
                    sbyte offset = (sbyte)value.GetLowByte();
                    Registers.Flags.H = Registers[dest].GetLowByte().WillHalfCarry(offset);
                    Registers.Flags.C = Registers[dest].GetLowByte().WillCarry(offset);
                    if (offset >= 0)
                    {
                        Registers[dest] += (byte)offset;
                    }
                    else
                    {
                        Registers[dest] -= (byte)Math.Abs(offset);
                    }
                }
            }
        }

        public void Rrca()
        {
            Length = 1;
            Registers.Flags.Z = false;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = Registers.A.GetBit(0);
            Registers.A = (byte)(Registers.A >> 1);
            Registers.A = Registers.A.SetBit(7, Registers.Flags.C);
        }

        public void Stop()
        {
            Length = 1;
            throw new NotImplementedException("STOP not implemented.");
        }

        public void Rla()
        {
            Length = 1;
            Registers.Flags.Z = false;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            bool oldCarry = Registers.Flags.C;
            Registers.Flags.C = Registers.A.GetBit(7);
            Registers.A = (byte)(Registers.A << 1);
            Registers.A = Registers.A.SetBit(0, oldCarry);
        }

        public void Jr(string condition)
        {
            Length = 2;
            sbyte offset = (sbyte)ReadByte();
            bool jump = condition == string.Empty
                || condition == "NZ" && !Registers.Flags.Z || condition == "Z" && Registers.Flags.Z
                || condition == "NC" && !Registers.Flags.C || condition == "C" && Registers.Flags.C;
            if (jump)
            {
                Length += 1;
                if (offset >= 0)
                {
                    Registers.PC = (ushort)(Registers.PC + offset);
                }
                else
                {
                    Registers.PC = (ushort)(Registers.PC - Math.Abs(offset));
                }
            }
        }

        public void Rra()
        {
            Length = 1;
            Registers.Flags.Z = false;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            bool oldCarry = Registers.Flags.C;
            Registers.Flags.C = Registers.A.GetBit(0);
            Registers.A = (byte)(Registers.A >> 1);
            Registers.A = Registers.A.SetBit(7, oldCarry);
        }

        public void Daa()
        {
            Length = 1;
            int adj = 0;
            if (Registers.Flags.C || (Registers.A > 0x99 && !Registers.Flags.N))
            {
                Registers.Flags.C = (!Registers.Flags.N || Registers.Flags.C);
                adj += 0x60;
            }
            if (Registers.Flags.H || ((Registers.A & 0x0F) > 0x09 && !Registers.Flags.N))
            {
                adj += 0x06;
            }
            Registers.A += (byte)(Registers.Flags.N ? -adj : adj);
            Registers.Flags.Z = Registers.A == 0;
            Registers.Flags.H = false;
        }

        public void Cpl()
        {
            Length = 1;
            Registers.A = (byte)~Registers.A;
            Registers.Flags.N = true;
            Registers.Flags.H = true;
        }

        public void Scf()
        {
            Length = 1;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = true;
        }

        public void Ccf()
        {
            Length = 1;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = !Registers.Flags.C;
        }

        public void Halt()
        {
            throw new NotImplementedException("HALT not implemented.");
        }

        public void Adc(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            byte newValue = (byte)(Registers[dest] + (Registers.Flags.C ? 1 : 0));
            Registers.Flags.H = Registers[dest].GetLowByte().WillHalfCarry(Registers.Flags.C ? 1 : 0);
            Registers.Flags.C = Registers[dest].GetLowByte().WillCarry(Registers.Flags.C ? 1 : 0);
            Registers.Flags.H |= newValue.WillHalfCarry(value);
            Registers.Flags.C |= newValue.WillCarry(value);
            Registers.Flags.N = false;
            newValue += value.GetLowByte();
            Registers.Flags.Z = newValue == 0;
            Registers[dest] = newValue;
        }

        public void Sub(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            byte newValue = (byte)(Registers[dest] - value);
            Registers.Flags.Z = newValue == 0;
            Registers.Flags.N = true;
            Registers.Flags.H = Registers[dest].GetLowByte().WillHalfBorrow(value);
            Registers.Flags.C = Registers[dest].GetLowByte().WillBorrow(value);
            Registers[dest] = newValue;
        }

        public void Sbc(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            byte newValue = (byte)(Registers[dest] - (Registers.Flags.C ? 1 : 0));
            Registers.Flags.H = Registers[dest].GetLowByte().WillHalfBorrow(Registers.Flags.C ? 1 : 0);
            Registers.Flags.C = Registers[dest].GetLowByte().WillBorrow(Registers.Flags.C ? 1 : 0);
            Registers.Flags.H |= newValue.WillHalfBorrow(value);
            Registers.Flags.C |= newValue.WillBorrow(value);
            Registers.Flags.N = true;
            newValue -= value.GetLowByte();
            Registers.Flags.Z = newValue == 0;
            Registers[dest] = newValue;
        }

        public void And(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            Registers[dest] &= value;
            Registers.Flags.Z = Registers[dest] == 0;
            Registers.Flags.N = false;
            Registers.Flags.H = true;
            Registers.Flags.C = false;
        }

        public void Xor(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            Registers[dest] ^= value;
            Registers.Flags.Z = Registers[dest] == 0;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = false;
        }

        public void Or(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            Registers[dest] |= value;
            Registers.Flags.Z = Registers[dest] == 0;
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = false;
        }

        public void Cp(string dest, string source)
        {
            Length = 1;
            ushort value = GetValue(source).Value;
            byte newValue = (byte)(Registers[dest] - value);
            Registers.Flags.Z = newValue == 0;
            Registers.Flags.N = true;
            Registers.Flags.H = Registers[dest].GetLowByte().WillHalfBorrow(value);
            Registers.Flags.C = Registers[dest].GetLowByte().WillBorrow(value);
        }

        public void Ret(string condition)
        {
            Length = 2;
            bool jump = condition == string.Empty
                || condition == "NZ" && !Registers.Flags.Z || condition == "Z" && Registers.Flags.Z
                || condition == "NC" && !Registers.Flags.C || condition == "C" && Registers.Flags.C;
            if (jump)
            {
                Length += condition == string.Empty ? 2 : 3;
                Registers.PC = PopWord();
            }
        }

        public void Pop(string source)
        {
            Length = 3;
            Registers[source] = PopWord();
        }

        public void Jp(string condition, string dest)
        {
            LocationType destType = dest.ToLocationType();
            Length = condition == string.Empty ? 1 : 3;
            bool jump = condition == string.Empty
                || condition == "NZ" && !Registers.Flags.Z || condition == "Z" && Registers.Flags.Z
                || condition == "NC" && !Registers.Flags.C || condition == "C" && Registers.Flags.C;
            ushort value = ReadWord();
            if (jump)
            {
                if (destType == LocationType.WordRegister)
                {
                    Registers.PC = Registers[dest];
                }
                else
                {
                    Length += condition != string.Empty ? 1 : 3;
                    Registers.PC = value;
                }
            }
        }

        public void Call(string condition, string dest)
        {
            Length = 3;
            bool jump = condition == string.Empty
                || condition == "NZ" && !Registers.Flags.Z || condition == "Z" && Registers.Flags.Z
                || condition == "NC" && !Registers.Flags.C || condition == "C" && Registers.Flags.C;
            ushort value = ReadWord();
            if (jump)
            {
                Length += 3;
                PushWord(Registers.PC);
                Registers.PC = value;
            }
        }

        public void Push(string source)
        {
            Length = 4;
            PushWord(Registers[source]);
        }

        public void Rst(string dest)
        {
            Length = 4;
            PushWord(Registers.PC);
            Registers.PC = Convert.ToByte(dest.Substring(0, 2), 16);
        }

        public void Prefix()
        {
            Length = 1;
            CB = true;
        }

        public void Reti()
        {
            Length = 4;
            IME = true;
            Registers.PC = PopWord();
        }

        public void Di()
        {
            Length = 1;
            IME = false;
        }

        public void Ei()
        {
            Length = 1;
            IME = true;
        }

        public void Rlc(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(7);
            value.Value = (byte)(value.Value << 1);
            value.Value = value.Value.GetLowByte().SetBit(0, Registers.Flags.C);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Rrc(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(0);
            value.Value = (byte)(value.Value >> 1);
            value.Value = value.Value.GetLowByte().SetBit(7, Registers.Flags.C);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Rl(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            bool oldCarry = Registers.Flags.C;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(7);
            value.Value = (byte)(value.Value << 1);
            value.Value = value.Value.GetLowByte().SetBit(0, oldCarry);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Rr(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            bool oldCarry = Registers.Flags.C;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(0);
            value.Value = (byte)(value.Value >> 1);
            value.Value = value.Value.GetLowByte().SetBit(7, oldCarry);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Sla(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(7);
            value.Value = (byte)(value.Value << 1);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Sra(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(0);
            bool sign = value.Value.GetLowByte().GetBit(7);
            value.Value = (byte)(value.Value >> 1);
            value.Value = value.Value.GetLowByte().SetBit(7, sign);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Swap(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = false;
            value.Value = value.Value.GetLowByte().SwapNibbles();
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Srl(string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = false;
            Registers.Flags.C = value.Value.GetLowByte().GetBit(0);
            value.Value = (byte)(value.Value >> 1);
            Registers.Flags.Z = value.Value == 0;
            SetValue(source, value);
        }

        public void Bit(string bit, string source)
        {
            Length = 2;
            var value = GetValue(source);
            Registers.Flags.N = false;
            Registers.Flags.H = true;
            Registers.Flags.Z = !value.Value.GetLowByte().GetBit(Convert.ToInt32(bit));
            if (source.Length > 1)
            {
                Length -= 1; // BIT b,(HL) would otherwise be 4 cycles rather than 3
            }
        }

        public void Res(string bit, string source)
        {
            Length = 2;
            var value = GetValue(source);
            value.Value = value.Value.GetLowByte().ClearBit(Convert.ToInt32(bit));
            SetValue(source, value);
        }

        public void Set(string bit, string source)
        {
            Length = 2;
            var value = GetValue(source);
            value.Value = value.Value.GetLowByte().SetBit(Convert.ToInt32(bit));
            SetValue(source, value);
        }
    }
}
