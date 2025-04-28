using System;
using System.IO;

namespace Atem.Core.Audio.Channel
{
    public class PulseChannel : AudioChannel
    {
        private byte[] _dutyCycles = [0b11111110, 0b01111110, 0b01111000, 0b10000001];
        private byte _sampleIndex = 0;
        private int _periodSweepTimer = 0;
        private byte _periodSweepPeriod = 0;
        private byte _duty = 0;
        private byte _initialPeriodSweepPeriod = 0;
        private bool _periodSweepEnabled = false;
        private byte _periodSweepDirection = 0;
        private byte _periodSweepStep = 0;

        public byte InitialPeriodSweepPeriod { get => _initialPeriodSweepPeriod; set => _initialPeriodSweepPeriod = value; }
        public byte PeriodSweepDirection { get => _periodSweepDirection; set => _periodSweepDirection = value; }
        public byte PeriodSweepStep { get => _periodSweepStep; set => _periodSweepStep = value; }
        public byte Duty { get => _duty; set => _duty = value; }

        public PulseChannel(bool periodSweepEnabled = false)
        {
            _periodSweepEnabled = periodSweepEnabled;
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

        public override void UpdatePeriodSweep()
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
                            On = false;
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

        public override void GetState(BinaryWriter writer)
        {
            writer.Write(_sampleIndex);
            writer.Write(_periodSweepTimer);
            writer.Write(_periodSweepPeriod);
            writer.Write(_duty);
            writer.Write(_initialPeriodSweepPeriod);
            writer.Write(_periodSweepEnabled);
            writer.Write(_periodSweepDirection);
            writer.Write(_periodSweepStep);

            base.GetState(writer);
        }

        public override void SetState(BinaryReader reader)
        {
            _sampleIndex = reader.ReadByte();
            _periodSweepTimer = reader.ReadInt32();
            _periodSweepPeriod = reader.ReadByte();
            _duty = reader.ReadByte();
            _initialPeriodSweepPeriod = reader.ReadByte();
            _periodSweepEnabled = reader.ReadBoolean();
            _periodSweepDirection = reader.ReadByte();
            _periodSweepStep = reader.ReadByte();

            base.SetState(reader);
        }
    }
}
