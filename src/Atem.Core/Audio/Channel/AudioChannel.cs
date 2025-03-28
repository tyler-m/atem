using Atem.Core.State;
using System;
using System.IO;

namespace Atem.Core.Audio.Channel
{
    public abstract class AudioChannel : IAudioChannel, IStateful
    {
        private const int PERIOD_UPDATE_PERIOD = 2048; // 512 Hz
        private const int LENGTH_UPDATE_PERIOD = 4096; // 256 Hz
        private const int VOLUME_ENVELOPE_UPDATE_PERIOD = 16384; // 64 Hz
        private const int VOLUME_ENVELOPE_INCREASING = 1;
        private const int DEFAULT_CHANNEL_LENGTH = 64;

        protected const int MAX_CHANNEL_VOLUME = 15;
        protected const int MIN_CHANNEL_VOLUME = 0;

        private int _channelTimer = 0;
        private int _periodTimer = 0;
        private int _lengthTimer = 0;
        private int _volumeEnvelopeTimer = 0;
        private byte _volume = 0;

        protected int ChannelTimer { get { return _channelTimer; } set { } }

        public byte Volume { get { return _volume; } set { _volume = value; } }

        private bool _channelOn = false;
        private bool _lengthEnabled = false;
        private byte _initialVolume = 0;
        private ushort _initialPeriod = 0;
        private byte _volumeEnvelopeDirection = 0;
        private byte _volumeEnvelopePeriod = 0;
        private byte _initialLengthTimer = 0;
        private bool _leftChannel = false;
        private bool _rightChannel = false;
        private bool _trigger = false;

        public bool IsOn { get { return _channelOn; } set { _channelOn = value; } }
        public bool LengthEnabled { get { return _lengthEnabled; } set { _lengthEnabled = value; } }
        public byte InitialVolume { get { return _initialVolume; } set { _initialVolume = value; } }
        public ushort InitialPeriod { get { return _initialPeriod; } set { _initialPeriod = value; } }
        public byte VolumeEnvelopeDirection { get { return _volumeEnvelopeDirection; } set { _volumeEnvelopeDirection = value; } }
        public byte VolumeEnvelopePeriod { get { return _volumeEnvelopePeriod; } set { _volumeEnvelopePeriod = value; } }
        public byte InitialLengthTimer { get { return _initialLengthTimer; } set { _initialLengthTimer = value; } }
        public bool LeftChannel { get { return _leftChannel; } set { _leftChannel = value; } }
        public bool RightChannel { get { return _rightChannel; } set { _rightChannel = value; } }

        public bool Trigger
        {
            get { return _trigger; }
            set
            {
                _trigger = value;

                if (_trigger && IsOutputting)
                {
                    IsOn = true;
                    Volume = InitialVolume;
                    _volumeEnvelopeTimer = 0;
                    _periodTimer = InitialPeriod;
                    _lengthTimer = InitialLengthTimer;
                    OnTrigger();
                }
            }
        }

        protected virtual int PeriodUpdatesPerClock { get { return 1; } }
        protected virtual int ChannelLength { get { return DEFAULT_CHANNEL_LENGTH; } }

        public virtual bool IsOutputting
        {
            get
            {
                return InitialVolume != 0 || VolumeEnvelopeDirection != 0;
            }
            set { }
        }

        public virtual void OnPeriodReset() { }
        public virtual void OnTrigger() { }
        public virtual void OnClock() { }

        public abstract byte ProvideSample();

        private float NormalizeSample(byte volume)
        {
            return (float)volume / MAX_CHANNEL_VOLUME;
        }

        public float Sample()
        {
            float normalizedSample = 0.0f;

            if (IsOutputting && IsOn)
            {
                normalizedSample = NormalizeSample(ProvideSample());
            }

            return normalizedSample;
        }

        private void UpdatePeriod()
        {
            _periodTimer++;

            if (_periodTimer >= PERIOD_UPDATE_PERIOD)
            {
                OnPeriodReset();
                _periodTimer = InitialPeriod;
            }
        }

        private void UpdateLength()
        {
            if (LengthEnabled)
            {
                _lengthTimer++;

                if (_lengthTimer >= ChannelLength)
                {
                    IsOn = false;
                }
            }
        }

        private void UpdateVolumeEnvelope()
        {
            _volumeEnvelopeTimer++;

            if (_volumeEnvelopeTimer >= VolumeEnvelopePeriod && VolumeEnvelopePeriod > 0)
            {
                _volumeEnvelopeTimer = 0;

                if (VolumeEnvelopeDirection == VOLUME_ENVELOPE_INCREASING)
                {
                    Volume = (byte)Math.Min(Volume + 1, MAX_CHANNEL_VOLUME);
                }
                else
                {
                    Volume = (byte)Math.Max(Volume - 1, MIN_CHANNEL_VOLUME);
                }
            }
        }

        public void Clock()
        {
            if (IsOn)
            {
                _channelTimer++;

                for (int i = 0; i < PeriodUpdatesPerClock; i++)
                {
                    UpdatePeriod();
                }

                if (_channelTimer % LENGTH_UPDATE_PERIOD == 0)
                {
                    UpdateLength();
                }

                if (_channelTimer % VOLUME_ENVELOPE_UPDATE_PERIOD == 0)
                {
                    UpdateVolumeEnvelope();
                    _channelTimer = 0;
                }

                OnClock();
            }
        }

        public virtual void GetState(BinaryWriter writer)
        {
            writer.Write(_channelTimer);
            writer.Write(_periodTimer);
            writer.Write(_lengthTimer);
            writer.Write(_volumeEnvelopeTimer);
            writer.Write(_volume);
            writer.Write(_channelOn);
            writer.Write(_lengthEnabled);
            writer.Write(_initialVolume);
            writer.Write(_initialPeriod);
            writer.Write(_volumeEnvelopeDirection);
            writer.Write(_volumeEnvelopePeriod);
            writer.Write(_initialLengthTimer);
            writer.Write(_leftChannel);
            writer.Write(_rightChannel);
            writer.Write(_trigger);
        }
        public virtual void SetState(BinaryReader reader)
        {
            _channelTimer = reader.ReadInt32();
            _periodTimer = reader.ReadInt32();
            _lengthTimer = reader.ReadInt32();
            _volumeEnvelopeTimer = reader.ReadInt32();
            _volume = reader.ReadByte();
            _channelOn = reader.ReadBoolean();
            _lengthEnabled = reader.ReadBoolean();
            _initialVolume = reader.ReadByte();
            _initialPeriod = reader.ReadUInt16();
            _volumeEnvelopeDirection = reader.ReadByte();
            _volumeEnvelopePeriod = reader.ReadByte();
            _initialLengthTimer = reader.ReadByte();
            _leftChannel = reader.ReadBoolean();
            _rightChannel = reader.ReadBoolean();
            _trigger = reader.ReadBoolean();
        }
    }
}
