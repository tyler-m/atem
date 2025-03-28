
using System.IO;

namespace Atem.Core.Audio.Channel
{
    public class WaveChannel : AudioChannel
    {
        private const int RAM_SIZE = 16;

        private byte[] _ram = new byte[RAM_SIZE];
        private int _sampleIndex = 0;
        private byte _outputLevel = 0;
        private bool _isOutputting = false;

        protected override int ChannelLength { get => 256; }
        protected override int PeriodUpdatesPerClock { get => 2; }

        public byte OutputLevel { get => _outputLevel; set => _outputLevel = value; }

        public override bool IsOutputting
        {
            get => _isOutputting;
            set
            {
                _isOutputting = value;

                if (!IsOutputting)
                {
                    On = false;
                }
            }
        }

        public override void OnPeriodReset()
        {
            _sampleIndex = (_sampleIndex + 1) % (RAM_SIZE * 2);
        }

        public override byte ProvideSample()
        {
            int index = _sampleIndex / 2;
            byte sample;

            if (_sampleIndex % 2 == 0)
            {
                sample = _ram[index].GetHighNibble();
            }
            else
            {
                sample = _ram[index].GetLowNibble();
            }

            int shift = 8;
            if (OutputLevel != 0)
            {
                shift = OutputLevel - 1;
            }

            return (byte)(sample >> shift);
        }

        public void WriteRAM(int index, byte value)
        {
            if (index >= 0 && index < _ram.Length)
            {
                _ram[index] = value;
            }
        }

        public byte ReadRAM(int index)
        {
            if (index >= 0 && index < _ram.Length)
            {
                return _ram[index];
            }

            return 0xFF;
        }

        public override void GetState(BinaryWriter writer)
        {
            writer.Write(_ram);
            writer.Write(_sampleIndex);
            writer.Write(_outputLevel);
            writer.Write(_isOutputting);

            base.GetState(writer);
        }

        public override void SetState(BinaryReader reader)
        {
            _ram = reader.ReadBytes(_ram.Length);
            _sampleIndex = reader.ReadInt32();
            _outputLevel = reader.ReadByte();
            _isOutputting = reader.ReadBoolean();

            base.SetState(reader);
        }
    }
}
