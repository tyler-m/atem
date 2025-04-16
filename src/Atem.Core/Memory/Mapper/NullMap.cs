using System.IO;

namespace Atem.Core.Memory.Mapper
{
    /// <summary>
    /// A mapper intended to have no state or function.
    /// </summary>
    public class NullMap : IMapper
    {
        public byte[] RAM { get => []; set { } }

        public byte[] GetBatterySave() => [];

        public void Init(byte type, byte[] rom, int ramSize) { }

        public void LoadBatterySave(byte[] saveData) { }

        public byte ReadRAM(ushort address) => 0x00;

        public byte ReadROM(ushort address) => 0x00;

        public byte ReadROMBank(ushort address) => 0x00;

        public void WriteRAM(ushort address, byte value) { }

        public void WriteRegister(ushort address, byte value) { }

        public void GetState(BinaryWriter writer) { }

        public void SetState(BinaryReader reader) { }
    }
}
