﻿using System.IO;
using System.Text;
using Atem.Core.Memory.Mapper;

namespace Atem.Core.Memory
{
    public class Cartridge : ICartridge
    {
        private byte _type;
        private byte _colorFlag;
        private bool _loaded;
        private string _title;
        private IMapper _mbc;

        public bool Loaded { get => _loaded; }
        public string Title { get => _title; }
        public bool SupportsColor { get => _colorFlag == 0x80 || _colorFlag == 0xC0; }

        public Cartridge()
        {
            ResetMapper();
        }

        public void ResetMapper()
        {
            _mbc = new NullMap();
        }

        public byte[] GetBatterySave()
        {
            return _mbc.GetBatterySave();
        }

        public void LoadBatterySave(byte[] saveData)
        {
            _mbc.LoadBatterySave(saveData);
        }

        public bool Load(byte[] rom)
        {
            _loaded = false;

            byte checksum = 0;
            for (ushort address = 0x0134; address <= 0x014C; address++)
            {
                checksum -= (byte)(rom[address] + 1);
            }

            if (checksum != rom[0x014d])
            {
                return false;
            }

            _title = string.Empty;
            for (ushort address = 0x0134; address <= 0x0143; address++)
            {
                byte c = rom[address];
                if (c != 0)
                {
                    _title += Encoding.UTF8.GetString([c]);
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
            else if (_type >= 0x19 && _type <= 0x1E) // MBC5
            {
                _mbc = new MBC5();
                _mbc.Init(_type, rom, ramSizeInBytes);
            }
            else
            {
                return false;
            }

            _loaded = true;
            return _loaded;
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

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_type);
            writer.Write(_colorFlag);
            writer.Write(_loaded);

            _mbc.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _type = reader.ReadByte();
            _colorFlag = reader.ReadByte();
            _loaded = reader.ReadBoolean();

            _mbc.SetState(reader);
        }
    }
}
