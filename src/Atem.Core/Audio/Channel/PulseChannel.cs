using System.IO;

namespace Atem.Core.Audio.Channel
{
    public class PulseChannel : AudioChannel
    {
        private readonly byte[] _dutyCycles = [0b11111110, 0b01111110, 0b01111000, 0b10000001];

        private byte _sampleIndex;
        private int _periodSweepTimer;
        private byte _periodSweepPeriod;
        private bool _sweepChannel;
        private bool _periodSweepEnabled;
        private int _sweepPeriod;
        private bool _subtractedFromPeriod;

        public byte InitialPeriodSweepPeriod { get; set; }
        public byte PeriodSweepDirection { get; set; }
        public byte PeriodSweepStep { get; set; }
        public byte Duty { get; set; }

        public PulseChannel(bool sweepChannel = false)
        {
            _sweepChannel = sweepChannel;
        }

        public void OnSweepDirectionChange(byte sweepDirection)
        {
            // blargg 05-sweep details
            // test 4: exiting negate mode after calculation disables channel
            if (_subtractedFromPeriod && sweepDirection == 0)
            {
                On = false;
            }
        }

        public override void OnTrigger()
        {
            _sampleIndex = 0;
            _periodSweepTimer = 0;
            _subtractedFromPeriod = false;

            // blargg 05-sweep details
            // test 2: timer treats period 0 as 8
            _periodSweepPeriod = (byte)(InitialPeriodSweepPeriod != 0 ? InitialPeriodSweepPeriod : 8);

            if (_sweepChannel)
            {
                _sweepPeriod = InitialPeriod;
                _periodSweepEnabled = InitialPeriodSweepPeriod != 0 || PeriodSweepStep != 0;

                if (_periodSweepEnabled && PeriodSweepStep != 0)
                {
                    TrySweep(false);
                }
            }
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

        private int CalculateNextSweepPeriod(int period)
        {
            int delta = period >> PeriodSweepStep;

            if (PeriodSweepDirection == 0)
            {
                return period + delta;
            }
            else
            {
                _subtractedFromPeriod = true;
                return period - delta;
            }
        }

        private bool TrySweep(bool updateInitialPeriod)
        {
            int nextPeriod = CalculateNextSweepPeriod(_sweepPeriod);

            if (nextPeriod > 0x7FF)
            {
                On = false;
                return false;
            }

            InitialPeriod = updateInitialPeriod ? (ushort)nextPeriod : InitialPeriod;
            _sweepPeriod = updateInitialPeriod ? (ushort)nextPeriod : _sweepPeriod;
            return true;
        }

        public override void UpdatePeriodSweep()
        {
            if (_sweepChannel && _periodSweepEnabled)
            {
                _periodSweepTimer++;

                if (_periodSweepTimer >= _periodSweepPeriod)
                {
                    _periodSweepTimer = 0;

                    if (InitialPeriodSweepPeriod != 0)
                    {
                        if (PeriodSweepStep != 0)
                        {
                            if (TrySweep(true))
                            {
                                // blargg 04-sweep
                                // test 5: after updating frequency, calculates a second time
                                TrySweep(false);
                            }
                        }
                        else
                        {
                            TrySweep(false);
                        }

                        _periodSweepPeriod = InitialPeriodSweepPeriod;
                    }
                    else
                    {
                        // blargg 05-sweep details
                        // test 2: timer treats period 0 as 8
                        _periodSweepPeriod = 8;
                    }
                }
            }
        }

        public override void GetState(BinaryWriter writer)
        {
            writer.Write(_sampleIndex);
            writer.Write(_periodSweepTimer);
            writer.Write(_periodSweepPeriod);
            writer.Write(Duty);
            writer.Write(InitialPeriodSweepPeriod);
            writer.Write(_periodSweepEnabled);
            writer.Write(PeriodSweepDirection);
            writer.Write(PeriodSweepStep);
            writer.Write(_sweepPeriod);
            writer.Write(_subtractedFromPeriod);

            base.GetState(writer);
        }

        public override void SetState(BinaryReader reader)
        {
            _sampleIndex = reader.ReadByte();
            _periodSweepTimer = reader.ReadInt32();
            _periodSweepPeriod = reader.ReadByte();
            Duty = reader.ReadByte();
            InitialPeriodSweepPeriod = reader.ReadByte();
            _periodSweepEnabled = reader.ReadBoolean();
            PeriodSweepDirection = reader.ReadByte();
            PeriodSweepStep = reader.ReadByte();
            _sweepPeriod = reader.ReadInt32();
            _subtractedFromPeriod = reader.ReadBoolean();

            base.SetState(reader);
        }
    }
}
