using System.IO;

namespace Atem.Core.Memory.Mapper
{
    internal class NoMap : IMapper
    {
        private byte[] _rom;
        private byte[] _ram;

        public byte[] RAM { get => _ram; set => _ram = value; }

        public void Init(byte type, byte[] rom, int ramSize)
        {
            _rom = rom;
            _ram = new byte[ramSize];
        }

        public byte ReadROM(ushort address)
        {
            return _rom[address];
        }

        public byte ReadROMBank(ushort address)
        {
            return _rom[address];
        }

        public void WriteRAM(ushort address, byte value)
        {
            if (address < _ram.Length)
            {
                _ram[address] = value;
            }
        }

        public byte ReadRAM(ushort address)
        {
            if (address < _ram.Length)
            {
                return _ram[address];
            }

            return 0xFF;
        }

        public void WriteRegister(ushort address, byte value) { }

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
            throw new System.NotImplementedException();
        }

        public void SetState(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
