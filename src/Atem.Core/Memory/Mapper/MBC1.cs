using System.IO;

namespace Atem.Core.Memory.Mapper
{
    internal class MBC1 : IMapper
    {
        private byte _type;
        private byte[] _rom;
        private byte[] _ram;
        private bool _ramEnable = false;
        private bool _canSwitchBankingModes = false;
        private bool _largeCartridge = false;
        private int _romBank = 0;
        private int _extraBank = 0;
        private int _bankingMode = 0;

        public byte[] RAM { get => _ram; set => _ram = value; }

        public void Init(byte type, byte[] rom, int ramSize)
        {
            _type = type;
            _rom = rom;
            _ram = new byte[ramSize];

            _largeCartridge = _rom.Length >= 1 << 20; // > 1024 KiB rom
            _canSwitchBankingModes = !(_ram.Length <= 1 << 13 && _rom.Length <= 1 << 19); // < 8 KiB ram and 512 KiB rom
        }

        public byte ReadROM(ushort address)
        {
            int bank = 0;

            if (_largeCartridge && (_bankingMode & 0b1) == 1)
            {
                bank = _extraBank << 5;
            }

            return _rom[address + bank * 0x4000];
        }

        public byte ReadROMBank(ushort address)
        {
            int adjustedAddress = address - 0x4000;
            
            if ((_romBank & 0b11111) == 0)
            {
                adjustedAddress += 0x4000; // adjust to bank 1
            }
            else
            {
                adjustedAddress += 0x4000 * (_romBank & 0b11111);
            }

            if (address < _rom.Length)
            {
                return _rom[adjustedAddress];
            }

            return 0xFF;
        }

        public void WriteRAM(ushort address, byte value)
        {
            int bank = 0;

            if (!_largeCartridge && (_bankingMode & 0b1) == 1)
            {
                bank = _extraBank;
            }

            int adjustedAddress = address + 0x2000 * bank;
            if (adjustedAddress < _ram.Length && _ramEnable)
            {
                _ram[adjustedAddress] = value;
            }
        }

        public byte ReadRAM(ushort address)
        {
            int bank = 0;

            if (!_largeCartridge && (_bankingMode & 0b1) == 1)
            {
                bank = _extraBank;
            }

            int adjustedAddress = address + 0x2000 * bank;
            if (adjustedAddress < _ram.Length && _ramEnable)
            {
                return _ram[adjustedAddress];
            }

            return 0xFF;
        }

        public void WriteRegister(ushort address, byte value)
        {
            if (address <= 0x1FFF)
            {
                if (value.GetLowNibble() == 0xA)
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
                if (value == 0)
                {
                    _romBank = 1;
                }
                else
                {
                    _romBank = (byte)(value & 0x1F);
                }
            }
            else if (address <= 0x5FFF)
            {
                _extraBank = value & 0b11;
            }
            else
            {
                if (_canSwitchBankingModes)
                {
                    _bankingMode = value & 0b1;
                }
            }
        }

        public byte[] GetBatterySave()
        {
            return _ram;
        }

        public void LoadBatterySave(byte[] saveData)
        {
            _ram = saveData;
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_ram);
            writer.Write(_ramEnable);
            writer.Write(_romBank);
            writer.Write(_extraBank);
            writer.Write(_bankingMode);
        }

        public void SetState(BinaryReader reader)
        {
            _ram = reader.ReadBytes(_ram.Length);
            _ramEnable = reader.ReadBoolean();
            _romBank = reader.ReadInt32();
            _extraBank = reader.ReadInt32();
            _bankingMode = reader.ReadInt32();
        }
    }
}
