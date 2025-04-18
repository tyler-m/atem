using System;
using System.IO;

namespace Atem.Core.Memory.Mapper
{
    internal class MBC3 : IMapper
    {
        private byte _type;
        private byte[] _rom;
        private byte[] _ram;
        private bool _ramEnable = false;
        private int _romBank = 1;
        private int _ramBank = 0;
        private RTC _rtc = new();
        private RTC _rtcLatched = new();
        private byte _latch = 0;

        public byte[] RAM { get => _ram; set => _ram = value; }

        public void LoadRTCFromSaveData(byte[] data)
        {
            ReadOnlySpan<byte> dataSpan = data;
            _rtc = RTC.FromSaveData(dataSpan);
            _rtcLatched = RTC.FromSaveData(dataSpan, true);
            _rtcLatched.Latched = true;
        }

        public void Init(byte type, byte[] rom, int ramSize)
        {
            _type = type;
            _rom = rom;
            _ram = new byte[ramSize];
        }

        public byte ReadROM(ushort address)
        {
            return _rom[address];
        }

        public byte ReadROMBank(ushort address)
        {
            int adjustedAddress = (address - 0x4000) + 0x4000 * _romBank;
            if (adjustedAddress < _rom.Length)
            {
                return _rom[adjustedAddress];
            }
            return 0xFF;
        }

        public void WriteRAM(ushort address, byte value)
        {
            if (_ramEnable)
            {
                if (_ramBank <= 0x03)
                {
                    int adjustedAddress = address + 0x2000 * _ramBank;
                    if (adjustedAddress < _ram.Length)
                    {
                        _ram[adjustedAddress] = value;
                    }
                }
                else // RTC register write
                {
                    if (_ramBank == 0x08)
                    {
                        _rtc.Seconds = value;
                    }
                    else if (_ramBank == 0x09)
                    {
                        _rtc.Minutes = value;
                    }
                    else if (_ramBank == 0x0A)
                    {
                        _rtc.Hours = value;
                    }
                    else if (_ramBank == 0x0B)
                    {
                        _rtc.Day = (_rtc.Day & 0b100000000) | value;
                    }
                    else if (_ramBank == 0x0C)
                    {
                        _rtc.Day = (_rtc.Day & 0xFF) | (value.GetBit(0).Int() << 8);
                        _rtc.Halt = value.GetBit(6);
                        _rtc.DayCarry = value.GetBit(7);
                    }
                }
            }
        }

        public byte ReadRAM(ushort address)
        {
            if (_ramEnable)
            {
                if (_ramBank <= 0x03)
                {
                    int adjustedAddress = address + 0x2000 * _ramBank;
                    if (adjustedAddress < _ram.Length)
                    {
                        return _ram[adjustedAddress];
                    }
                }
                else // RTC register read
                {
                    if (_ramBank == 0x08)
                    {
                        return (byte)_rtcLatched.Seconds;
                    }
                    else if (_ramBank == 0x09)
                    {
                        return (byte)_rtcLatched.Minutes;
                    }
                    else if (_ramBank == 0x0A)
                    {
                        return (byte)_rtcLatched.Hours;
                    }
                    else if (_ramBank == 0x0B)
                    {
                        _rtcLatched.Day = (byte)_rtc.Day;
                    }
                    else if (_ramBank == 0x0C)
                    {
                        return (byte)(((_rtcLatched.Day >> 8) & 0b1) | (_rtcLatched.Halt.Int() << 6) | (_rtcLatched.DayCarry.Int() << 7));
                    }
                }
            }

            return 0xFF;
        }

        public void WriteRegister(ushort address, byte value)
        {
            if (address <= 0x1FFF)
            {
                if (value == 0x0A)
                {
                    _ramEnable = true;
                }
                else
                {
                    _ramEnable = false;
                }
            }
            else if (address <= 0x3FFF)
            {
                _romBank = value;

                if (_romBank == 0x00)
                {
                    _romBank = 0x01;
                }
            }
            else if (address <= 0x5FFF)
            {
                _ramBank = value;
            }
            else if (address <= 0x7FFF)
            {
                if (value == 0x00)
                {
                    _latch = 0x00;
                }
                else if (value == 0x01)
                {
                    if (_latch == 0x00)
                    {
                        _rtcLatched = new RTC(_rtc);
                        _rtcLatched.Latched = true;
                    }

                    _latch = 0x01;
                }
            }
        }

        public byte[] GetBatterySave()
        {
            byte[] saveFile = new byte[RAM.Length + 48];
            byte[] rtcData = new byte[48];

            rtcData[0] = (byte)_rtc.Seconds;
            rtcData[4] = (byte)_rtc.Minutes;
            rtcData[8] = (byte)_rtc.Hours;
            rtcData[12] = (byte)_rtc.Day;
            rtcData[16].SetBit(0, ((ushort)_rtc.Day).GetBit(8));
            rtcData[16].SetBit(6, _rtc.Halt);
            rtcData[16].SetBit(7, _rtc.DayCarry);
            rtcData[20] = (byte)_rtcLatched.Seconds;
            rtcData[24] = (byte)_rtcLatched.Minutes;
            rtcData[28] = (byte)_rtcLatched.Hours;
            rtcData[32] = (byte)_rtcLatched.Day;
            rtcData[36].SetBit(0, ((ushort)_rtcLatched.Day).GetBit(8));
            rtcData[36].SetBit(6, _rtcLatched.Halt);
            rtcData[36].SetBit(7, _rtcLatched.DayCarry);

            byte[] timestampData = BitConverter.GetBytes((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // RTC values are conventionally saved in little endian
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampData);
            }

            Array.Copy(timestampData, 0, rtcData, 40, 4);
            Array.Copy(RAM, saveFile, RAM.Length);
            Array.Copy(rtcData, 0, saveFile, RAM.Length, rtcData.Length);
            return saveFile;
        }

        public void LoadBatterySave(byte[] saveData)
        {
            Array.Copy(saveData, _ram, _ram.Length);

            // RTC data is appended at the end of a save file and is 48 bytes long
            if (saveData.Length - _ram.Length == 48)
            {
                byte[] rtcData = new byte[48];
                Array.Copy(saveData, saveData.Length - 48, rtcData, 0, 48);
                LoadRTCFromSaveData(rtcData);
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_ram);
            writer.Write(_ramEnable);
            writer.Write(_romBank);
            writer.Write(_ramBank);
            _rtc.GetState(writer);
            _rtcLatched.GetState(writer);
            writer.Write(_latch);
        }

        public void SetState(BinaryReader reader)
        {
            _ram = reader.ReadBytes(_ram.Length);
            _ramEnable = reader.ReadBoolean();
            _romBank = reader.ReadInt32();
            _ramBank = reader.ReadInt32();
            _rtc.SetState(reader);
            _rtcLatched.SetState(reader);
            _latch = reader.ReadByte();
        }
    }
}
