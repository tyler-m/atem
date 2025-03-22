
namespace Atem.Core.Memory.Mapper
{
    internal class MBC5 : IMapper
    {
        private byte _type = 0;
        private byte[] _rom;
        private byte[] _ram;
        private bool _ramEnable = false;
        private int _ramBank = 0;
        private ushort _romBank = 0;

        public byte[] RAM { get => _ram; set => _ram = value; }

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
                int adjustedAddress = address + 0x2000 * _ramBank;
                if (adjustedAddress < _ram.Length)
                {
                    _ram[adjustedAddress] = value;
                }
            }
        }

        public byte ReadRAM(ushort address)
        {
            if (_ramEnable)
            {
                int adjustedAddress = address + 0x2000 * _ramBank;
                if (adjustedAddress < _ram.Length)
                {
                    return _ram[adjustedAddress];
                }
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
            else if (address <= 0x2FFF)
            {
                _romBank = (ushort)(_romBank.GetHighByte() | value);
            }
            else if (address <= 0x3FFF)
            {
                _romBank = _romBank.SetBit(8);
            }
            else if (address <= 0x5FFF)
            {
                if (value <= 0x0F)
                {
                    _ramBank = value;
                }
            }
            else if (address <= 0x7FFF)
            {

            }
        }
    }
}
