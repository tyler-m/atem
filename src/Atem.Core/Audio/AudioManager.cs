using Atem.Core.Audio.Channel;
using Atem.Core.Audio.Filter;
using Atem.Core.Processing;
using Atem.Core.State;
using System;
using System.IO;

namespace Atem.Core.Audio
{
    public class AudioManager: IStateful
    {
        private const float MAX_VOLUME = 15.0f;
        private const int BUFFER_SIZE = 2048;

        public const int SAMPLE_RATE = 44100;

        private AudioChannelMixer _mixer = new AudioChannelMixer();
        private byte[] _buffer = new byte[BUFFER_SIZE];
        private int _sampleTimer = 0;
        private int _bufferIndex = 0;
        private float _sampleTimerRemainder = 0.0f;
        private float _samplePeriod = Processor.Frequency / SAMPLE_RATE;

        private byte _leftChannelVolume = 0;
        private byte _rightChannelVolume = 0;

        public byte LeftChannelVolume { get { return _leftChannelVolume; } set { _leftChannelVolume = value; } }
        public byte RightChannelVolume { get { return _rightChannelVolume; } set { _rightChannelVolume = value; } }

        internal PulseChannel Channel1 = new PulseChannel(true);
        internal PulseChannel Channel2 = new PulseChannel();
        internal WaveChannel Channel3 = new WaveChannel();
        internal NoiseChannel Channel4 = new NoiseChannel();

        public AudioRegisters Registers;
        public delegate void FullAudioBufferEvent(byte[] buffer);
        public event FullAudioBufferEvent OnFullBuffer;

        public bool On { get; set; }

        public AudioManager()
        {
            Registers = new(this);
            _mixer.AddChannel(Channel1);
            _mixer.AddChannel(Channel2);
            _mixer.AddChannel(Channel3);
            _mixer.AddChannel(Channel4);
            _mixer.AddFilter(new HighPassFilter(0.991f));
        }

        public void SampleChannels()
        {
            (float leftMix, float rightMix) = _mixer.Sample();

            leftMix *= (LeftChannelVolume / MAX_VOLUME);
            rightMix *= (RightChannelVolume / 15.0f);

            leftMix = Math.Clamp(leftMix, -1.0f, 1.0f);
            rightMix = Math.Clamp(rightMix, -1.0f, 1.0f);

            short leftMixShort = (short)(leftMix * short.MaxValue);
            short rightMixShort = (short)(rightMix  * short.MaxValue);

            _buffer[_bufferIndex++] = (byte)(leftMixShort & 0xFF);
            _buffer[_bufferIndex++] = (byte)(leftMixShort >> 8);
            _buffer[_bufferIndex++] = (byte)(rightMixShort & 0xFF);
            _buffer[_bufferIndex++] = (byte)(rightMixShort >> 8);

            if (_bufferIndex == BUFFER_SIZE)
            {
                SendBuffer();
            }
        }

        public void SendBuffer()
        {
            _bufferIndex = 0;
            OnFullBuffer?.Invoke(_buffer);
        }

        private void ClockChannels()
        {
            _mixer.Clock();
        }

        public void Clock()
        {
            if (!On)
            {
                return;
            }

            ClockChannels();
            _sampleTimer += 4;

            if (_sampleTimer + _sampleTimerRemainder >= _samplePeriod)
            {
                SampleChannels();
                _sampleTimerRemainder = _sampleTimer + _sampleTimerRemainder - _samplePeriod;
                _sampleTimer = 0;
            }
        }

        public void WriteWaveRAM(byte index, byte value)
        {
            Channel3.WriteRAM(index, value);
        }

        public byte ReadWaveRAM(byte index)
        {
            return Channel3.ReadRAM(index);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_buffer);
            writer.Write(_sampleTimer);
            writer.Write(_bufferIndex);
            writer.Write(_sampleTimerRemainder);
            writer.Write(_leftChannelVolume);
            writer.Write(_rightChannelVolume);

            Channel1.GetState(writer);
            Channel2.GetState(writer);
            Channel3.GetState(writer);
            Channel4.GetState(writer);

            writer.Write(On);
        }

        public void SetState(BinaryReader reader)
        {
            _buffer = reader.ReadBytes(_buffer.Length);
            _sampleTimer = reader.ReadInt32();
            _bufferIndex = reader.ReadInt32();
            _sampleTimerRemainder = reader.ReadSingle();
            _leftChannelVolume = reader.ReadByte();
            _rightChannelVolume = reader.ReadByte();

            Channel1.SetState(reader);
            Channel2.SetState(reader);
            Channel3.SetState(reader);
            Channel4.SetState(reader);

            On = reader.ReadBoolean();
        }
    }
}
