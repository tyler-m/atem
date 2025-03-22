using Atem.Core.Audio.Channel;

namespace Atem.Core.Audio
{
    public class AudioRegisters
    {
        private PulseChannel Channel1 => _manager?.Channel1;
        private PulseChannel Channel2 => _manager?.Channel2;
        private WaveChannel Channel3 => _manager?.Channel3;
        private NoiseChannel Channel4 => _manager?.Channel4;

        AudioManager _manager;

        public AudioRegisters(AudioManager manager)
        {
            _manager = manager;
        }

        private static byte BitsToByte(params bool[] bits)
        {
            byte result = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                result = result.SetBit(i, bits[bits.Length - i - 1]);
            }
            return result;
        }

        public byte NR50
        {
            get
            {
                return (byte)((_manager.LeftChannelVolume << 4) | _manager.RightChannelVolume);
            }
            set
            {
                _manager.LeftChannelVolume = (byte)(value & 0b01110000 >> 4);
                _manager.RightChannelVolume = (byte)(value & 0b111);
            }
        }

        public byte NR51
        {
            get
            {
                return BitsToByte(Channel4.LeftChannel, Channel3.LeftChannel, Channel2.LeftChannel, Channel1.LeftChannel,
                    Channel4.RightChannel, Channel3.RightChannel, Channel2.RightChannel, Channel1.RightChannel);
            }
            set
            {
                Channel4.LeftChannel = value.GetBit(7);
                Channel3.LeftChannel = value.GetBit(6);
                Channel2.LeftChannel = value.GetBit(5);
                Channel1.LeftChannel = value.GetBit(4);
                Channel4.RightChannel = value.GetBit(3);
                Channel3.RightChannel = value.GetBit(2);
                Channel2.RightChannel = value.GetBit(1);
                Channel1.RightChannel = value.GetBit(0);
            }
        }

        private byte _nr52;

        public byte NR52
        {
            set
            {
                if (!On && value.GetBit(7))
                {
                    NR10 = 0;
                    NR11 = 0;
                    NR12 = 0;
                    NR13 = 0;
                    NR14 = 0;
                    NR21 = 0;
                    NR22 = 0;
                    NR23 = 0;
                    NR24 = 0;
                    NR41 = 0;
                    NR42 = 0;
                    NR43 = 0;
                    NR44 = 0;
                }

                _nr52 = _nr52.SetBit(7, value.GetBit(7));
            }
            get
            {
                return (byte)((_nr52.GetBit(7) ? 1 : 0) << 7 | (Channel4.IsOn ? 1 : 0) << 3 | (Channel3.IsOn ? 1 : 0) << 2 | (Channel2.IsOn ? 1 : 0) << 1 | (Channel1.IsOn ? 1 : 0));
            }
        }

        private bool On { get { return NR52.GetBit(7); } }

        public byte NR10
        {
            get
            {
                return (byte)((Channel1.InitialPeriodSweepPeriod & 0b111) << 4
                    | (Channel1.PeriodSweepDirection & 0b1) << 3
                    | Channel1.PeriodSweepStep & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel1.InitialPeriodSweepPeriod = (byte)((value & 0b01110000) >> 4);
                    Channel1.PeriodSweepDirection = (byte)(value.GetBit(3) ? 1 : 0);
                    Channel1.PeriodSweepStep = (byte)(value & 0b111);
                }
            }
        }

        public byte NR11
        {
            get
            {
                return (byte)(Channel1.Duty << 6 | Channel1.InitialLengthTimer & 0b111111);
            }
            set
            {
                if (On)
                {
                    Channel1.Duty = (byte)(value >> 6);
                    Channel1.InitialLengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR12
        {
            get
            {
                return (byte)(Channel1.InitialVolume << 4 | (Channel1.VolumeEnvelopeDirection & 0b1) << 3 | Channel1.VolumeEnvelopePeriod & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel1.InitialVolume = (byte)(value >> 4);
                    Channel1.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel1.VolumeEnvelopePeriod = (byte)(value & 0b00000111);
                }
            }
        }

        public byte NR13
        {
            get
            {
                return (byte)(Channel1.InitialPeriod & 0xFF);
            }
            set
            {
                if (On)
                {
                    Channel1.InitialPeriod = (ushort)(Channel1.InitialPeriod & 0b11100000000 | value);
                }
            }
        }

        public byte NR14
        {
            get
            {
                return (byte)((Channel1.Trigger ? 1 : 0) << 7
                    | (Channel1.LengthEnabled ? 1 : 0) << 6
                    | Channel1.InitialPeriod.GetHighByte() & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel1.Trigger = value.GetBit(7);
                    Channel1.LengthEnabled = value.GetBit(6);
                    Channel1.InitialPeriod = (ushort)(Channel1.InitialPeriod.GetLowByte() | (value & 0b111) << 8);
                }
            }
        }

        public byte NR21
        {
            get
            {
                return (byte)(Channel2.Duty << 6 | Channel2.InitialLengthTimer & 0b111111);
            }
            set
            {
                if (On)
                {
                    Channel2.Duty = (byte)(value >> 6);
                    Channel2.InitialLengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR22
        {
            get
            {
                return (byte)(Channel2.InitialVolume << 4 | (Channel2.VolumeEnvelopeDirection & 0b1) << 3 | Channel2.VolumeEnvelopePeriod & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel2.InitialVolume = (byte)(value >> 4);
                    Channel2.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel2.VolumeEnvelopePeriod = (byte)(value & 0b00000111);
                }
            }
        }

        public byte NR23
        {
            get
            {
                return (byte)(Channel2.InitialPeriod & 0xFF);
            }
            set
            {
                if (On)
                {
                    Channel2.InitialPeriod = (ushort)(Channel2.InitialPeriod & 0b11100000000 | value);
                }
            }
        }

        public byte NR24
        {
            get
            {
                return (byte)((Channel2.Trigger ? 1 : 0) << 7
                    | (Channel2.LengthEnabled ? 1 : 0) << 6
                    | Channel2.InitialPeriod.GetHighByte() & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel2.Trigger = value.GetBit(7);
                    Channel2.LengthEnabled = value.GetBit(6);
                    Channel2.InitialPeriod = (ushort)(Channel2.InitialPeriod.GetLowByte() | (value & 0b111) << 8);
                }
            }
        }

        public byte NR30
        {
            get
            {
                return (byte)((Channel3.IsOutputting ? 1 : 0) << 7);
            }
            set
            {
                if (On)
                {
                    Channel3.IsOutputting = value.GetBit(7);
                }
            }
        }

        public byte NR31
        {
            get
            {
                return Channel3.InitialLengthTimer;
            }
            set
            {
                if (On)
                {
                    Channel3.InitialLengthTimer = value;
                }
            }
        }

        public byte NR32
        {
            get
            {
                return (byte)((Channel3.OutputLevel & 0b11) << 5);
            }
            set
            {
                if (On)
                {
                    Channel3.OutputLevel = (byte)((value & 0b01100000) >> 5);
                }
            }
        }

        public byte NR33
        {
            get
            {
                return (byte)(Channel3.InitialPeriod & 0xFF);
            }
            set
            {
                if (On)
                {
                    Channel3.InitialPeriod = (ushort)(Channel3.InitialPeriod & 0b11100000000 | value);
                }
            }
        }

        public byte NR34
        {
            get
            {
                return (byte)((Channel3.Trigger ? 1 : 0) << 7
                    | (Channel3.LengthEnabled ? 1 : 0) << 6
                    | Channel3.InitialPeriod.GetHighByte() & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel3.LengthEnabled = value.GetBit(6);
                    Channel3.InitialPeriod = (ushort)(Channel3.InitialPeriod.GetLowByte() | (value & 0b111) << 8);
                    Channel3.Trigger = value.GetBit(7);
                }
            }
        }

        public byte NR41
        {
            get
            {
                return (byte)(Channel4.InitialLengthTimer & 0b00111111);
            }
            set
            {
                if (On)
                {
                    Channel4.InitialLengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR42
        {
            get
            {
                return (byte)(Channel4.InitialVolume << 4 | (Channel4.VolumeEnvelopeDirection & 0b1) << 3 | Channel4.VolumeEnvelopePeriod & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel4.InitialVolume = (byte)(value >> 4);
                    Channel4.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel4.VolumeEnvelopePeriod = (byte)(value & 0b00000111);
                }
            }
        }

        public byte NR43
        {
            get
            {
                return (byte)(Channel4.ClockShift << 4 | ((Channel4.ShiftRegisterMode & 0b1) << 3) | Channel4.ClockDivider & 0b111);
            }
            set
            {
                if (On)
                {
                    Channel4.ClockShift = (byte)(value >> 4);
                    Channel4.ShiftRegisterMode = (byte)(value.GetBit(3) ? 1 : 0);
                    Channel4.ClockDivider = (byte)(value & 0b111);
                }
            }
        }

        public byte NR44
        {
            get
            {
                return (byte)((Channel4.Trigger ? 1 : 0) << 7 | (Channel4.LengthEnabled ? 1 : 0) << 6);
            }
            set
            {
                if (On)
                {
                    Channel4.LengthEnabled = value.GetBit(6);
                    Channel4.Trigger = value.GetBit(7);
                }
            }
        }
    }
}
