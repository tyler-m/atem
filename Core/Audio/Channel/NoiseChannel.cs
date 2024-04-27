using System;

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
                double clockShift = ClockShift;
                
                if (ClockShift == 0)
                {
                    clockShift = 0.5;
                }

                return (int)(4 * Math.Pow(2, clockShift) * ClockDivider);
            }
        }

        public override void OnTrigger()
        {
            _shiftRegister = 0;
            _shiftTimer = 0;
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

            _shiftRegister = (ushort)(_shiftRegister >> 1);
        }
    }
}
