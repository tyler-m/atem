﻿using Atem.Core.State;

namespace Atem.Core.Memory.Mapper
{
    public interface IMapper : IStateful
    {
        public byte[] RAM { get; set; }
        public void Init(byte type, byte[] rom, int ramSize);
        public byte ReadROM(ushort address);
        public byte ReadROMBank(ushort address);
        public byte ReadRAM(ushort address);
        public void WriteRAM(ushort address, byte value);
        public void WriteRegister(ushort address, byte value);
        public byte[] GetBatterySave();
        public void LoadBatterySave(byte[] saveData);
    }
}
