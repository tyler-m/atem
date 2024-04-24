﻿
namespace Atem
{
    internal class Timer
    {
        // should probably be incrementing _div every clock
        // and bit shifting it on read instead of using the
        // additional _divTick with conditionals
        private byte _div;
        private byte _tima;
        private byte _tma;
        private byte _tac;

        private int _divTick;
        private int _timaTick;
        private Bus _bus;

        public Timer(Bus bus)
        {
            _bus = bus;
        }

        public byte DIV
        { 
            get
            {
                return _div;
            }
            set
            {
                _div = 0;
            }
        }

        public byte TIMA
        {
            get
            {
                return _tima;
            }
            set
            {
                _tima = value;
            }
        }

        public byte TMA
        {
            get
            {
                return _tma;
            }
            set
            {
                _tma = value;
            }
        }

        public byte TAC
        {
            get
            {
                return _tac;
            }
            set
            {
                _tac = TAC;
            }
        }

        private bool TACEnabled
        {
            get
            {
                return _tac.GetBit(2);
            }
        }

        private int TIMATickPeriod
        {
            get
            {
                int clockMode = _tac & 0b11;

                if (clockMode == 0)
                {
                    return 256; // 4194304Hz / 4096Hz / 4
                }
                else if (clockMode == 1)
                {
                    return 4; // 4194304Hz / 262144Hz / 4
                }
                else if (clockMode == 2)
                {
                    return 16; // 4194304Hz / 65536Hz / 4
                }
                else
                {
                    return 64; // 4194304Hz / 16384Hz / 4
                }
            }
        }

        private void IncrementTIMA()
        {
            if (_tima.WillCarry(1))
            {
                _tima = _tma;
                _bus.RequestInterrupt(InterruptType.Timer);
            }
            else
            {
                _tima++;
            }
        }

        public void Clock()
        {
            _divTick++;

            if (_divTick >= 64) // 4194304Hz / 16384Hz / 4
            {
                _div++;
                _divTick = 0;
            }

            if (TACEnabled)
            {
                _timaTick++;

                if (_timaTick >= TIMATickPeriod)
                {
                    IncrementTIMA();
                    _timaTick = 0;
                }
            }
        }
    }
}
