
using System;

namespace Atem.Core.Audio.Channel
{
    internal class PulseChannel : AudioChannel
    {
        private const int PERIOD_SWEEP_UPDATE_PERIOD = 8192; // 128 Hz

        private byte[] _dutyCycles = new byte[] { 0b11111110, 0b01111110, 0b01111000, 0b10000001 };
        private byte _sampleIndex = 0;
        private int _periodSweepTimer = 0;
        private byte _periodSweepPeriod = 0;

        private byte _duty = 0;
        private byte _initialPeriodSweepPeriod = 0;
        private bool _periodSweepEnabled = false;
        private byte _periodSweepDirection = 0;
        private byte _periodSweepStep = 0;

        public byte InitialPeriodSweepPeriod { get { return _initialPeriodSweepPeriod; } set { _initialPeriodSweepPeriod = value; } }
        public byte PeriodSweepDirection { get { return _periodSweepDirection; } set { _periodSweepDirection = value; } }
        public byte PeriodSweepStep { get { return _periodSweepStep; } set { _periodSweepStep = value; } }
        public byte Duty { get { return _duty; } set { _duty = value; } }

        public PulseChannel(bool periodSweepEnabled = false)
        {
            _periodSweepEnabled = periodSweepEnabled;
        }

        public override void OnClock()
        {
            if (ChannelTimer % PERIOD_SWEEP_UPDATE_PERIOD == 0)
            {
                UpdatePeriodSweep();
            }
        }

        public override void OnTrigger()
        {
            _sampleIndex = 0;
            _periodSweepTimer = 0;
            _periodSweepPeriod = InitialPeriodSweepPeriod;
        }

        public override void OnPeriodReset()
        {
            _sampleIndex = (byte)((_sampleIndex + 1) % 8);
        }

        public override byte ProvideSample()
        {
            if (_dutyCycles[Duty].GetBit(_sampleIndex))
            {
                return Volume;
            }

            return MIN_CHANNEL_VOLUME;
        }

        private void UpdatePeriodSweep()
        {
            if (_periodSweepEnabled && InitialPeriodSweepPeriod != 0)
            {
                _periodSweepTimer++;

                if (_periodSweepTimer >= _periodSweepPeriod)
                {
                    if (PeriodSweepDirection == 0)
                    {
                        if (InitialPeriod + InitialPeriod / Math.Pow(2, PeriodSweepStep) > 0x7FF)
                        {
                            IsOn = false;
                            return;
                        }

                        InitialPeriod = (ushort)(InitialPeriod + InitialPeriod / Math.Pow(2, PeriodSweepStep));
                    }
                    else
                    {
                        InitialPeriod = (ushort)(InitialPeriod - InitialPeriod / Math.Pow(2, PeriodSweepStep));
                    }

                    _periodSweepPeriod = InitialPeriodSweepPeriod;
                    _periodSweepTimer = 0;
                }
            }
        }
    }
}
