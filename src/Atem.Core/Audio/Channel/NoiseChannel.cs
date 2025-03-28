
using System.IO;

namespace Atem.Core.Audio.Channel
{
    internal class NoiseChannel : AudioChannel
    {
        private const int SHIFT_REGISTER_MODE_7BIT = 1;

        private int _shiftTimer = 0;
        private ushort _shiftRegister = 0;

        private byte _clockShift = 0;
        private byte _shiftRegisterMode = 0;
        private byte _clockDivider = 0;

        public byte ClockShift { get { return _clockShift; } set { _clockShift = value; } }
        public byte ShiftRegisterMode { get { return _shiftRegisterMode; } set { _shiftRegisterMode = value; } }
        public byte ClockDivider { get { return _clockDivider; } set { _clockDivider = value; } }

        public int ShiftPeriod
        {
            get
            {
                int divisor = 8;

                if (ClockDivider > 0)
                {
                    divisor = ClockDivider * 16;
                }

                return (divisor << ClockShift) / 4;
            }
        }

        public override void OnTrigger()
        {
            _shiftRegister = 0;
        }

        public override void OnClock()
        {
            _shiftTimer++;

            if (_shiftTimer >= ShiftPeriod)
            {
                Shift();
                _shiftTimer = 0;
            }
        }

        public override byte ProvideSample()
        {
            if (_shiftRegister.GetBit(0))
            {
                return Volume;
            }

            return MIN_CHANNEL_VOLUME;
        }

        public void Shift()
        {
            bool result = !(_shiftRegister.GetBit(0) ^ _shiftRegister.GetBit(1));
            _shiftRegister = _shiftRegister.SetBit(15, result);

            if (ShiftRegisterMode == SHIFT_REGISTER_MODE_7BIT)
            {
                _shiftRegister = _shiftRegister.SetBit(7, result);
            }
            _shiftRegister >>= 1;
        }

        public override void GetState(BinaryWriter writer)
        {
            writer.Write(_shiftTimer);
            writer.Write(_shiftRegister);
            writer.Write(_clockShift);
            writer.Write(_shiftRegisterMode);
            writer.Write(_clockDivider);

            base.GetState(writer);
        }

        public override void SetState(BinaryReader reader)
        {
            _shiftTimer = reader.ReadInt32();
            _shiftRegister = reader.ReadUInt16();
            _clockShift = reader.ReadByte();
            _shiftRegisterMode = reader.ReadByte();
            _clockDivider = reader.ReadByte();

            base.SetState(reader);
        }
    }
}
