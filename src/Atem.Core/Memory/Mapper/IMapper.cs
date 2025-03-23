
namespace Atem.Core.Memory.Mapper
{
    internal interface IMapper
    {
        public byte[] RAM { get; set; }
        public void Init(byte type, byte[] rom, int ramSize);
        public byte ReadROM(ushort address);
        public byte ReadROMBank(ushort address);
        public byte ReadRAM(ushort address);
        public void WriteRAM(ushort address, byte value);
        public void WriteRegister(ushort address, byte value);
        public byte[] ExportSave();
    }
}
