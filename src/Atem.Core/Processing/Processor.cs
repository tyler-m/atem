using System.Collections.Generic;
using System;
using System.Numerics;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Processing
{
    public class Processor : IStateful
    {
        public static int Frequency = 4194304;

        private Bus _bus;
        public CPURegisters Registers;

        private Dictionary<byte, Func<Processor, int>> _instructions = new Dictionary<byte, Func<Processor, int>>();
        private Dictionary<byte, Func<Processor, int>> _instructionsCB = new Dictionary<byte, Func<Processor, int>>();

        public byte IR;
        public int Length = 0;
        public bool CB = false;
        public bool IME = false;

        public bool DoubleSpeed = false;
        public bool SpeedSwitchFlag = false;

        public byte KEY1
        {
            get
            {
                return (byte)((DoubleSpeed.Int() << 7) | (SpeedSwitchFlag.Int()));
            }
            set
            {
                DoubleSpeed = value.GetBit(7);
                SpeedSwitchFlag = value.GetBit(0);
            }
        }

        private bool _halted = false;
        private bool _busHalted = false;
        private bool _instructionFinished = true;
        private int _tick = 0;

        private ushort[] _interruptJumps = new ushort[5] { 0x0040, 0x0048, 0x0050, 0x0058, 0x0060 };
        private int _interruptType;

        public ushort AddressOfNextInstruction
        {
            get
            {
                return Registers.PC;
            }
        }

        public Processor(Bus bus)
        {
            _bus = bus;
            PopulateInstructionsLists();
        }

        private void PopulateInstructionsLists()
        {
            Instructions.Load.PopulateLookup(_instructions);
            Instructions.Control.PopulateLookup(_instructions);
            Instructions.Arithmetic.PopulateLookup(_instructions);
            Instructions.Bit.PopulateLookup(_instructionsCB);
        }

        public byte ReadByte()
        {
            return ReadBus(Registers.PC++);
        }

        public ushort ReadWord()
        {
            byte low = ReadByte();
            byte high = ReadByte();
            return (ushort)(high << 8 | low);
        }

        private byte PopByte()
        {
            return ReadBus(Registers.SP++);
        }

        public ushort PopWord()
        {
            byte low = PopByte();
            byte high = PopByte();
            return (ushort)(high << 8 | low);
        }

        private void PushByte(byte value)
        {
            WriteBus(--Registers.SP, value);
        }

        public void PushWord(ushort value)
        {
            PushByte(value.GetHighByte());
            PushByte(value.GetLowByte());
        }

        public byte ReadBus(ushort address)
        {
            return _bus.Read(address);
        }

        public void WriteBus(ushort address, byte value)
        {
            _bus.Write(address, value);
        }

        public bool Clock()
        {
            if (_busHalted)
            {
                return false;
            }

            if (_halted)
            {
                byte IE = _bus.Read(0xFFFF);
                byte IF = _bus.Read(0xFF0F);
                if ((IE & IF) != 0)
                {
                    _halted = false;
                    _instructionFinished = true;
                }
                else
                {
                    return false;
                }
            }

            if (_instructionFinished)
            {
                _tick = 0;
                _instructionFinished = false;

                if (IME)
                {
                    byte IE = _bus.Read(0xFFFF);
                    byte IF = _bus.Read(0xFF0F);
                    int i = BitOperations.TrailingZeroCount(IE & IF);

                    if (i < 5)
                    {
                        IME = false;
                        _bus.Write(0xFF0F, IF.ClearBit(i));
                        _instructionFinished = false;
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
                    Length = _instructions[IR](this);
                }
            }

            if (CB && _tick == 1)
            {
                IR = ReadByte();
                Length = _instructionsCB[IR](this);
                CB = false;
            }

            _tick++;
            return _instructionFinished = _tick >= Length && !CB;
        }

        public void Halt()
        {
            _halted = true;
        }

        internal void RequestHalt()
        {
            _busHalted = true;
        }

        internal void RequestUnhalt()
        {
            _busHalted = false;
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(Registers.A);
            writer.Write(Registers.B);
            writer.Write(Registers.C);
            writer.Write(Registers.D);
            writer.Write(Registers.E);
            writer.Write(Registers.H);
            writer.Write(Registers.L);
            writer.Write(Registers.SP);
            writer.Write(Registers.PC);
            writer.Write(Registers.Flags.F);
            writer.Write(IR);
            writer.Write(Length);
            writer.Write(CB);
            writer.Write(IME);
            writer.Write(DoubleSpeed);
            writer.Write(SpeedSwitchFlag);
            writer.Write(_halted);
            writer.Write(_busHalted);
            writer.Write(_instructionFinished);
            writer.Write(_tick);
            writer.Write(_interruptType);
        }

        public void SetState(BinaryReader reader)
        {
            Registers.A = reader.ReadByte();
            Registers.B = reader.ReadByte();
            Registers.C = reader.ReadByte();
            Registers.D = reader.ReadByte();
            Registers.E = reader.ReadByte();
            Registers.H = reader.ReadByte();
            Registers.L = reader.ReadByte();
            Registers.SP = reader.ReadUInt16();
            Registers.PC = reader.ReadUInt16();
            Registers.Flags.F = reader.ReadByte();
            IR = reader.ReadByte();
            Length = reader.ReadInt32();
            CB = reader.ReadBoolean();
            IME = reader.ReadBoolean();
            DoubleSpeed = reader.ReadBoolean();
            SpeedSwitchFlag = reader.ReadBoolean();
            _halted = reader.ReadBoolean();
            _busHalted = reader.ReadBoolean();
            _instructionFinished = reader.ReadBoolean();
            _tick = reader.ReadInt32();
            _interruptType = reader.ReadInt32();
        }
    }
}
