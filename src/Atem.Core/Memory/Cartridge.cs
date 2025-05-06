using System.Collections.Generic;
using System.IO;
using System.Text;
using Atem.Core.Memory.Mapper;

namespace Atem.Core.Memory
{
    public class Cartridge : ICartridge
    {
        private byte _type;
        private byte _colorFlag;
        private IMapper _mbc;

        public bool Loaded { get; private set; }
        public string Title { get; private set; }
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
            Loaded = false;

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
                    Title += Encoding.UTF8.GetString([c]);
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

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0x7FFF)
            {
                if (Loaded)
                {
                    return ReadROM(address);
                }
                else
                {
                    return 0x00;
                }
            }
            else
            {
                return ReadRAM(address);
            }
        }

        public void Write(ushort address, byte value, bool ignoreRenderMode = false)
        {
            if (address <= 0x7FFF)
            {
                WriteROM(address, value);
            }
            else
            {
                WriteRAM(address, value);
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (0x0000, 0x7FFF); // cartridge ROM
            yield return (0xA000, 0xBFFF); // cartridge RAM
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_type);
            writer.Write(_colorFlag);
            writer.Write(Loaded);
            _mbc.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _type = reader.ReadByte();
            _colorFlag = reader.ReadByte();
            Loaded = reader.ReadBoolean();
            _mbc.SetState(reader);
        }
    }
}
