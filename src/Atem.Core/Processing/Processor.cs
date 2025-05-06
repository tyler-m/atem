using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Atem.Core.Processing
{
    public class Processor : IProcessor
    {
        public const int FREQUENCY = 4194304;

        private readonly ProcessorRegisters _registers;
        private readonly IBus _bus;
        private byte _instruction;
        private int _length;
        private bool _halted;
        private bool _instructionFinished = true;
        private int _tick;
        private readonly ushort[] _interruptJumps = [0x0040, 0x0048, 0x0050, 0x0058, 0x0060];
        private int _interruptType;

        private readonly Dictionary<byte, Func<IProcessor, int>> _instructions = [];
        private readonly Dictionary<byte, Func<IProcessor, int>> _instructionsCB = [];

        public ProcessorRegisters Registers => _registers;
        public bool CB { get; set; }
        public bool DoubleSpeed { get; set; }
        public bool SpeedSwitchFlag { get; set; }
        public bool IME { get; set; }

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

        public Processor(IBus bus)
        {
            _bus = bus;
            _registers = new ProcessorRegisters();
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
                        _length = 5;
                        PushWord(Registers.PC);
                        Registers.PC = _interruptJumps[_interruptType];
                        _tick++;
                        return false;
                    }
                }

                _instruction = ReadByte();
                if (!CB)
                {
                    _length = _instructions[_instruction](this);
                }
            }

            if (CB && _tick == 1)
            {
                _instruction = ReadByte();
                _length = _instructionsCB[_instruction](this);
                CB = false;
            }

            _tick++;
            return _instructionFinished = _tick >= _length && !CB;
        }

        public void Halt()
        {
            _halted = true;
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return address switch
            {
                0xFF4D => KEY1,
                _ => 0xFF,
            };
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            switch (address)
            {
                case 0xFF4D:
                    KEY1 = value;
                    break;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (0xFF4D, 0xFF4D); // KEY1
        }

        public void GetState(BinaryWriter writer)
        {
            _registers.GetState(writer);
            writer.Write(_instruction);
            writer.Write(_length);
            writer.Write(CB);
            writer.Write(IME);
            writer.Write(DoubleSpeed);
            writer.Write(SpeedSwitchFlag);
            writer.Write(_halted);
            writer.Write(_instructionFinished);
            writer.Write(_tick);
            writer.Write(_interruptType);
        }

        public void SetState(BinaryReader reader)
        {
            _registers.SetState(reader);
            _instruction = reader.ReadByte();
            _length = reader.ReadInt32();
            CB = reader.ReadBoolean();
            IME = reader.ReadBoolean();
            DoubleSpeed = reader.ReadBoolean();
            SpeedSwitchFlag = reader.ReadBoolean();
            _halted = reader.ReadBoolean();
            _instructionFinished = reader.ReadBoolean();
            _tick = reader.ReadInt32();
            _interruptType = reader.ReadInt32();
        }
    }
}
