using Atem.Core.Audio.Channel;

namespace Atem.Core.Audio
{
    public class AudioRegisters
    {
        private PulseChannel Channel1 => _manager?.Channel1;
        private PulseChannel Channel2 => _manager?.Channel2;
        private WaveChannel Channel3 => _manager?.Channel3;
        private NoiseChannel Channel4 => _manager?.Channel4;

        private readonly AudioManager _manager;

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

        private bool On { get => _manager.On; }

        public int VinLeft { get; set; }
        public int VinRight { get; set; }
        public byte NR50
        {
            get => (byte)((VinLeft << 7) | (_manager.LeftChannelVolume << 4) | (VinRight << 3) | _manager.RightChannelVolume);
            set
            {
                if (On)
                {
                    _manager.LeftChannelVolume = (byte)((value & 0b01110000) >> 4);
                    _manager.RightChannelVolume = (byte)(value & 0b111);
                    VinLeft = value.GetBit(7).Int();
                    VinRight = value.GetBit(3).Int();
                }
            }
        }

        public byte NR51
        {
            get => BitsToByte(Channel4.LeftChannel, Channel3.LeftChannel, Channel2.LeftChannel, Channel1.LeftChannel,
                    Channel4.RightChannel, Channel3.RightChannel, Channel2.RightChannel, Channel1.RightChannel);
            set
            {
                if (On)
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
        }

        public byte NR52
        {
            get => (byte)((On.Int() << 7) | 0b01110000 | (Channel4.On.Int() << 3) | (Channel3.On.Int() << 2) | (Channel2.On.Int() << 1) | Channel1.On.Int());
            set
            {
                if (!value.GetBit(7))
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
                    NR30 = 0;
                    NR31 = 0;
                    NR32 = 0;
                    NR33 = 0;
                    NR34 = 0;
                    NR41 = 0;
                    NR42 = 0;
                    NR43 = 0;
                    NR44 = 0;
                    NR50 = 0;
                    NR51 = 0;
                }

                _manager.On = value.GetBit(7);
            }
        }

        public byte NR10
        {
            get => (byte)(0b10000000
                    | ((Channel1.InitialPeriodSweepPeriod & 0b111) << 4)
                    | ((Channel1.PeriodSweepDirection & 0b1) << 3)
                    | (Channel1.PeriodSweepStep & 0b111));
            set
            {
                if (On)
                {
                    Channel1.InitialPeriodSweepPeriod = (byte)((value & 0b01110000) >> 4);
                    Channel1.PeriodSweepDirection = (byte)value.GetBit(3).Int();
                    Channel1.PeriodSweepStep = (byte)(value & 0b111);
                }
            }
        }

        public byte NR11
        {
            get => (byte)((Channel1.Duty << 6) | 0b00111111);
            set
            {
                if (On)
                {
                    Channel1.Duty = (byte)(value >> 6);
                    Channel1.LengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR12
        {
            get => (byte)((Channel1.InitialVolume << 4) | ((Channel1.VolumeEnvelopeDirection & 0b1) << 3) | (Channel1.VolumeEnvelopePeriod & 0b111));
            set
            {
                if (On)
                {
                    Channel1.InitialVolume = (byte)(value >> 4);
                    Channel1.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel1.VolumeEnvelopePeriod = (byte)(value & 0b00000111);

                    if (value >> 3 == 0)
                    {
                        Channel1.On = false;
                    }
                }
            }
        }

        public byte NR13
        {
            get => 0xFF;
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
            get => (byte)((Channel1.LengthEnabled.Int() << 6) | 0b10111111);
            set
            {
                if (On)
                {
                    Channel1.LengthEnabled = value.GetBit(6);
                    Channel1.InitialPeriod = (ushort)(Channel1.InitialPeriod.GetLowByte() | (value & 0b111) << 8);
                    Channel1.Trigger = value.GetBit(7);
                }
            }
        }

        public byte NR21
        {
            get => (byte)((Channel2.Duty << 6) | 0b111111);
            set
            {
                if (On)
                {
                    Channel2.Duty = (byte)(value >> 6);
                    Channel2.LengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR22
        {
            get => (byte)((Channel2.InitialVolume << 4) | ((Channel2.VolumeEnvelopeDirection & 0b1) << 3) | (Channel2.VolumeEnvelopePeriod & 0b111));
            set
            {
                if (On)
                {
                    Channel2.InitialVolume = (byte)(value >> 4);
                    Channel2.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel2.VolumeEnvelopePeriod = (byte)(value & 0b00000111);

                    if (value >> 3 == 0)
                    {
                        Channel2.On = false;
                    }
                }
            }
        }

        public byte NR23
        {
            get => 0xFF;
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
            get => (byte)((Channel2.LengthEnabled.Int() << 6) | 0b10111111);
            set
            {
                if (On)
                {
                    Channel2.LengthEnabled = value.GetBit(6);
                    Channel2.InitialPeriod = (ushort)(Channel2.InitialPeriod.GetLowByte() | ((value & 0b111) << 8));
                    Channel2.Trigger = value.GetBit(7);
                }
            }
        }

        public byte NR30
        {
            get => (byte)((Channel3.IsOutputting.Int() << 7) | 0b01111111);
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
            get => 0xFF;
            set
            {
                if (On)
                {
                    Channel3.LengthTimer = value;
                }
            }
        }

        public byte NR32
        {
            get => (byte)(((Channel3.OutputLevel & 0b11) << 5) | 0b10011111);
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
            get => 0xFF;
            set
            {
                if (On)
                {
                    Channel3.InitialPeriod = (ushort)((Channel3.InitialPeriod & 0b11100000000) | value);
                }
            }
        }

        public byte NR34
        {
            get => (byte)((Channel3.LengthEnabled.Int() << 6) | 0b10111111);
            set
            {
                if (On)
                {
                    Channel3.LengthEnabled = value.GetBit(6);
                    Channel3.InitialPeriod = (ushort)(Channel3.InitialPeriod.GetLowByte() | ((value & 0b111) << 8));
                    Channel3.Trigger = value.GetBit(7);
                }
            }
        }

        public byte NR41
        {
            get => 0xFF;
            set
            {
                if (On)
                {
                    Channel4.LengthTimer = (byte)(value & 0b00111111);
                }
            }
        }

        public byte NR42
        {
            get => (byte)((Channel4.InitialVolume << 4) | ((Channel4.VolumeEnvelopeDirection & 0b1) << 3) | (Channel4.VolumeEnvelopePeriod & 0b111));
            set
            {
                if (On)
                {
                    Channel4.InitialVolume = (byte)(value >> 4);
                    Channel4.VolumeEnvelopeDirection = (byte)((value & 0b00001000) >> 3);
                    Channel4.VolumeEnvelopePeriod = (byte)(value & 0b00000111);

                    if (value >> 3 == 0)
                    {
                        Channel4.On = false;
                    }
                }
            }
        }

        public byte NR43
        {
            get => (byte)((Channel4.ClockShift << 4) | ((Channel4.ShiftRegisterMode & 0b1) << 3) | (Channel4.ClockDivider & 0b111));
            set
            {
                if (On)
                {
                    Channel4.ClockShift = (byte)(value >> 4);
                    Channel4.ShiftRegisterMode = (byte)value.GetBit(3).Int();
                    Channel4.ClockDivider = (byte)(value & 0b111);
                }
            }
        }

        public byte NR44
        {
            get => (byte)((Channel4.LengthEnabled.Int() << 6) | 0b10111111);
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
