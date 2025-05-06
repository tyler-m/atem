using System;
using System.Collections.Generic;
using System.IO;
using Atem.Core.Audio.Channel;
using Atem.Core.Audio.Filter;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core.Audio
{
    public class AudioManager: IAudioManager, IStateful
    {
        private const float MAX_VOLUME = 15.0f;
        private const int BUFFER_SIZE = 2048;

        public const int SAMPLE_RATE = 44100;

        private readonly AudioChannelMixer _mixer = new();
        private byte[] _buffer = new byte[BUFFER_SIZE];
        private int _sampleTimer = 0;
        private int _bufferIndex = 0;
        private float _sampleTimerRemainder = 0.0f;
        private readonly float _samplePeriod = Processor.FREQUENCY / SAMPLE_RATE;
        private byte _leftChannelVolume = 0;
        private byte _rightChannelVolume = 0;
        private float _volumeFactor = 1.0f; // user-controlled volume

        public float VolumeFactor
        {
            get => _volumeFactor;
            set
            {
                _volumeFactor = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        public PulseChannel Channel1 { get; }
        public PulseChannel Channel2 { get; }
        public WaveChannel Channel3 { get; }
        public NoiseChannel Channel4 { get; }

        public AudioRegisters Registers { get; }
        public event Action<byte[]> OnFullAudioBuffer;

        public byte LeftChannelVolume { get => _leftChannelVolume; set => _leftChannelVolume = value; }
        public byte RightChannelVolume { get => _rightChannelVolume; set => _rightChannelVolume = value; }
        public bool On { get; set; }

        public IList<IAudioChannel> Channels { get; }

        public AudioManager()
        {
            Channel1 = new PulseChannel(true);
            Channel2 = new PulseChannel();
            Channel3 = new WaveChannel();
            Channel4 = new NoiseChannel();
            Channels = [Channel1, Channel2, Channel3, Channel4];
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

            leftMix = Math.Clamp(leftMix, -1.0f, 1.0f) * _volumeFactor;
            rightMix = Math.Clamp(rightMix, -1.0f, 1.0f) * _volumeFactor;

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
            OnFullAudioBuffer?.Invoke(_buffer);
        }

        public void Clock()
        {
            if (!On)
            {
                return;
            }

            _mixer.Clock();
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

        public byte ReadRegister(ushort address)
        {
            return address switch
            {
                0xFF10 => Registers.NR10,
                0xFF11 => Registers.NR11,
                0xFF12 => Registers.NR12,
                0xFF13 => Registers.NR13,
                0xFF14 => Registers.NR14,
                0xFF16 => Registers.NR21,
                0xFF17 => Registers.NR22,
                0xFF18 => Registers.NR23,
                0xFF19 => Registers.NR24,
                0xFF1A => Registers.NR30,
                0xFF1B => Registers.NR31,
                0xFF1C => Registers.NR32,
                0xFF1D => Registers.NR33,
                0xFF1E => Registers.NR34,
                0xFF20 => Registers.NR41,
                0xFF21 => Registers.NR42,
                0xFF22 => Registers.NR43,
                0xFF23 => Registers.NR44,
                0xFF24 => Registers.NR50,
                0xFF25 => Registers.NR51,
                0xFF26 => Registers.NR52,
                _ => 0xFF,
            };
        }

        public void WriteRegister(ushort address, byte value)
        {
            switch (address)
            {
                case 0xFF10:
                    Registers.NR10 = value;
                    break;
                case 0xFF11:
                    Registers.NR11 = value;
                    break;
                case 0xFF12:
                    Registers.NR12 = value;
                    break;
                case 0xFF13:
                    Registers.NR13 = value;
                    break;
                case 0xFF14:
                    Registers.NR14 = value;
                    break;
                case 0xFF16:
                    Registers.NR21 = value;
                    break;
                case 0xFF17:
                    Registers.NR22 = value;
                    break;
                case 0xFF18:
                    Registers.NR23 = value;
                    break;
                case 0xFF19:
                    Registers.NR24 = value;
                    break;
                case 0xFF1A:
                    Registers.NR30 = value;
                    break;
                case 0xFF1B:
                    Registers.NR31 = value;
                    break;
                case 0xFF1C:
                    Registers.NR32 = value;
                    break;
                case 0xFF1D:
                    Registers.NR33 = value;
                    break;
                case 0xFF1E:
                    Registers.NR34 = value;
                    break;
                case 0xFF20:
                    Registers.NR41 = value;
                    break;
                case 0xFF21:
                    Registers.NR42 = value;
                    break;
                case 0xFF22:
                    Registers.NR43 = value;
                    break;
                case 0xFF23:
                    Registers.NR44 = value;
                    break;
                case 0xFF24:
                    Registers.NR50 = value;
                    break;
                case 0xFF25:
                    Registers.NR51 = value;
                    break;
                case 0xFF26:
                    Registers.NR52 = value;
                    break;
            }
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0xFF26)
            {
                return ReadRegister(address);
            }
            else
            {
                return ReadWaveRAM((byte)(address.GetLowByte() - 0x30));
            }
        }

        public void Write(ushort address, byte value, bool ignoreRenderMode = false)
        {
            if (address <= 0xFF26)
            {
                WriteRegister(address, value);
            }
            else
            {
                WriteWaveRAM((byte)(address.GetLowByte() - 0x30), value);
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (0xFF10, 0xFF26); // registers
            yield return (0xFF30, 0xFF3F); // wave RAM
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
