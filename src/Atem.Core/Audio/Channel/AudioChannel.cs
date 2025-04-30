using System;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Audio.Channel
{
    public abstract class AudioChannel : IAudioChannel, IStateful
    {
        private const int PeriodUpdatePeriod = 2048; // 512 Hz
        private const int VolumeEnvelopeIncreasing = 1;
        private const int DefaultChannelLength = 64;

        protected const int MAX_CHANNEL_VOLUME = 15;
        protected const int MIN_CHANNEL_VOLUME = 0;

        // APU steps increment at 512 Hz
        // AudioChannel gets clocked at 1048576 Hz
        // 1048576 Hz / 512 Hz = 2048 clocks per step increment
        // TODO: this should be based on DIV from Timer. From Pan Docs:
        // A "DIV-APU" counter is increased every time DIV's bit 4 (5 in double-speed mode)
        // goes from 1 to 0, therefore at a frequency of 512 Hz(regardless of whether
        // double-speed is active). Thus, the counter can be made to increase faster by
        // writing to DIV while its relevant bit is set(which clears DIV, and triggers the
        // falling edge).
        private const int ClocksPerStep = 2048;
        private int _stepTimer;
        private int _step;

        private int _periodTimer;
        private int _volumeEnvelopeTimer;
        private bool _lengthEnabled;
        private bool _trigger;

        private static float NormalizeSample(byte volume) => (float)volume / MAX_CHANNEL_VOLUME;

        public byte Volume { get; set; }
        public bool On { get; set; }
        public byte InitialVolume { get; set; }
        public ushort InitialPeriod { get; set; }
        public byte VolumeEnvelopeDirection { get; set; }
        public byte VolumeEnvelopePeriod { get; set; }
        public int LengthTimer { get; set; }
        public bool LeftChannel { get; set; }
        public bool RightChannel { get; set; }
        public bool LengthEnabled { get => _lengthEnabled; set => SetLengthEnabled(value); }
        public bool Trigger { get => _trigger; set => SetTrigger(value); }
        
        public bool UserMute { get; set; }

        protected virtual int PeriodUpdatesPerClock { get => 1; }
        protected virtual int ChannelLength { get => DefaultChannelLength; }
        public virtual bool IsOutputting { get => InitialVolume != 0 || VolumeEnvelopeDirection != 0; set { } }

        public abstract byte ProvideSample();
        public virtual void OnClock() { }
        public virtual void OnPeriodReset() { }
        public virtual void OnTrigger() { }
        public virtual void UpdatePeriodSweep() { }

        private void SetLengthEnabled(bool value)
        {
            bool oldLengthEnabled = _lengthEnabled;
            _lengthEnabled = value;

            CheckIncrementLength(oldLengthEnabled);
        }

        private void SetTrigger(bool value)
        {
            _trigger = value;

            if (_trigger)
            {
                HandleTrigger();
            }
        }

        private void HandleTrigger()
        {
            On = true;
            Volume = InitialVolume;
            _volumeEnvelopeTimer = 0;
            _periodTimer = InitialPeriod;

            if (LengthTimer == ChannelLength)
            {
                LengthTimer = 0; // "trigger should treat 0 length as maximum"

                // blargg 03-trigger
                // test 8: trigger that un-freezes enabled length should clock it
                CheckIncrementLength();
            }

            // blargg 02-len ctr
            // test 14: disabled DAC should prevent enable at trigger
            if (!IsOutputting)
            {
                On = false;
            }

            OnTrigger();
        }

        public void Clock()
        {
            for (int i = 0; i < PeriodUpdatesPerClock; i++)
            {
                UpdatePeriod();
            }

            UpdateStep();

            OnClock();
        }

        public void RealignStep()
        {
            // blargg 07-len sweep period sync
            // test 5: powering up APU MODs next frame time with 8192
            _stepTimer = _stepTimer % ClocksPerStep;
            _step = 7;
        }

        private void UpdateStep()
        {
            _stepTimer++;

            if (_stepTimer >= ClocksPerStep)
            {
                _stepTimer = 0;
                _step = (_step + 1) & 7;

                switch (_step)
                {
                    case 0:
                    case 4:
                        IncrementLength();
                        break;
                    case 2:
                    case 6:
                        IncrementLength();
                        UpdatePeriodSweep();
                        break;
                    case 7:
                        UpdateVolumeEnvelope();
                        break;
                }
            }
        }

        public float Sample()
        {
            if (IsOutputting && On)
            {
                return NormalizeSample(ProvideSample());
            }

            return 0.0f;
        }

        private void UpdatePeriod()
        {
            _periodTimer++;

            if (_periodTimer >= PeriodUpdatePeriod)
            {
                _periodTimer = InitialPeriod;
                OnPeriodReset();
            }
        }

        private void UpdateVolumeEnvelope()
        {
            _volumeEnvelopeTimer++;

            if (_volumeEnvelopeTimer >= VolumeEnvelopePeriod && VolumeEnvelopePeriod > 0)
            {
                _volumeEnvelopeTimer = 0;

                if (VolumeEnvelopeDirection == VolumeEnvelopeIncreasing)
                {
                    Volume = (byte)Math.Min(Volume + 1, MAX_CHANNEL_VOLUME);
                }
                else
                {
                    Volume = (byte)Math.Max(Volume - 1, MIN_CHANNEL_VOLUME);
                }
            }
        }

        private void IncrementLength()
        {
            if (_lengthEnabled)
            {
                LengthTimer++;
                if (LengthTimer >= ChannelLength)
                {
                    LengthTimer = ChannelLength;
                    On = false;
                }
            }
        }

        private void CheckIncrementLength(bool preventUpdate = false)
        {
            if (!preventUpdate)
            {
                if (_step == 0 || _step == 2 || _step == 4 || _step == 6)
                {
                    IncrementLength();
                }
            }
        }

        public virtual void GetState(BinaryWriter writer)
        {
            writer.Write(_stepTimer);
            writer.Write(_step);
            writer.Write(_periodTimer);
            writer.Write(_volumeEnvelopeTimer);
            writer.Write(Volume);
            writer.Write(On);
            writer.Write(_lengthEnabled);
            writer.Write(InitialVolume);
            writer.Write(InitialPeriod);
            writer.Write(VolumeEnvelopeDirection);
            writer.Write(VolumeEnvelopePeriod);
            writer.Write(LengthTimer);
            writer.Write(LeftChannel);
            writer.Write(RightChannel);
            writer.Write(_trigger);
        }
        public virtual void SetState(BinaryReader reader)
        {
            _stepTimer = reader.ReadInt32();
            _step = reader.ReadInt32();
            _periodTimer = reader.ReadInt32();
            _volumeEnvelopeTimer = reader.ReadInt32();
            Volume = reader.ReadByte();
            On = reader.ReadBoolean();
            _lengthEnabled = reader.ReadBoolean();
            InitialVolume = reader.ReadByte();
            InitialPeriod = reader.ReadUInt16();
            VolumeEnvelopeDirection = reader.ReadByte();
            VolumeEnvelopePeriod = reader.ReadByte();
            LengthTimer = reader.ReadByte();
            LeftChannel = reader.ReadBoolean();
            RightChannel = reader.ReadBoolean();
            _trigger = reader.ReadBoolean();
        }
    }
}
