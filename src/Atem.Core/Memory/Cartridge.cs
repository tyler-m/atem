using System;
using System.IO;
using System.Text;
using Atem.Core.Memory.Mapper;

namespace Atem.Core.Memory
{
    public class Cartridge
    {
        private string _filepath;
        private byte _type;
        private byte _colorFlag;

        public bool Loaded;
        public string Title;

        public IMapper _mbc;

        public bool SupportsColor
        {
            get => _colorFlag == 0x80 || _colorFlag == 0xC0;
        }

        public void SaveRAM()
        {
            File.WriteAllBytes(_filepath + ".sav", _mbc.ExportSave());
        }

        public bool Load(string filepath)
        {
            Loaded = false;

            if (!File.Exists(filepath))
            {
                return false;
            }

            byte[] rom = File.ReadAllBytes(filepath);

            byte checksum = 0;
            for (ushort address = 0x0134; address <= 0x014C; address++)
            {
                checksum -= (byte)(rom[address] + 1);
            }

            if (checksum != rom[0x014d])
            {
                return false;
            }

            Title = string.Empty;
            for (ushort address = 0x0134; address <= 0x0143; address++)
            {
                byte c = rom[address];
                if (c != 0)
                {
                    Title += Encoding.UTF8.GetString(new byte[] { c });
                }
                else
                {
                    break;
                }
            }

            _colorFlag = rom[0x143];

            _type = rom[0x0147];
            int ramSize = rom[0x0149];

            int ramSizeInBytes;
            if (ramSize == 2)
            {
                ramSizeInBytes = 1 << 13;
            }
            else if (ramSize == 3)
            {
                ramSizeInBytes = 1 << 15;
            }
            else if (ramSize == 4)
            {
                ramSizeInBytes = 1 << 17;
            }
            else
            {
                ramSizeInBytes = 1 << 16;
            }

            if (_type == 0x00) // No MBC
            {
                _mbc = new NoMap();
                _mbc.Init(_type, rom, ramSizeInBytes);
            }
            else if (_type >= 0x01 && _type <= 0x03) // MBC1
            {
                _mbc = new MBC1();
                _mbc.Init(_type, rom, ramSizeInBytes);
            }
            else if (_type >= 0x0F && _type <= 0x13) // MBC3
            {
                _mbc = new MBC3();
                _mbc.Init(_type, rom, ramSizeInBytes);
            }
            else if (_type >= 0x19 && _type <= 0x1E)
            {
                _mbc = new MBC5();
                _mbc.Init(_type, rom, ramSizeInBytes);
            }
            else
            {
                return false;
            }

            // check if cartridge RAM has been saved for this game
            if (File.Exists(filepath + ".sav"))
            {
                byte[] saveFile = File.ReadAllBytes(filepath + ".sav");
                Array.Copy(saveFile, _mbc.RAM, _mbc.RAM.Length);

                // RTC data is appended at the end of a save file and is 48 bytes long
                if (saveFile.Length - ramSizeInBytes == 48)
                {
                    if (_mbc is MBC3 c)
                    {
                        byte[] rtcData = new byte[48];
                        Array.Copy(saveFile, saveFile.Length - 48, rtcData, 0, 48);
                        c.LoadRTCFromSaveData(rtcData);
                    }
                }
            }

            _filepath = filepath;
            Loaded = true;
            return Loaded;
        }

        public byte ReadROM(ushort address)
        {
            byte block = address.GetHighByte();

            if (block <= 0x3F)
            {
                return _mbc.ReadROM(address);
            }
            else if (block <= 0x7F)
            {
                return _mbc.ReadROMBank(address);
            }

            return 0xFF;
        }

        internal void WriteROM(ushort address, byte value)
        {
            _mbc.WriteRegister(address, value);
        }

        public byte ReadRAM(ushort address)
        {
            return _mbc.ReadRAM((ushort)(address - 0xA000));
        }

        public void WriteRAM(ushort address, byte value)
        {
            _mbc.WriteRAM((ushort)(address - 0xA000), value);
        }
    }
}
