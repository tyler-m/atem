using System;
using System.Collections.Generic;
using System.IO;
using Atem.Core.State;

namespace Atem.Core.Memory
{
    public class SystemMemory : IMemoryProvider, IStateful
    {
        private readonly byte[] _hram = new byte[0x7F];
        private readonly byte[] _wram = new byte[0x2000 * 4];
        private byte _svbk;

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0xCFFF)
            {
                return _wram[address - 0xC000];
            }
            else if (address <= 0xDFFF)
            {
                int bank = _svbk;
                if (bank == 0)
                {
                    bank = 1;
                }
                return _wram[address - 0xD000 + bank * 0x1000];
            }
            else if (address == 0xFF70)
            {
                return _svbk;
            }
            else if (address <= 0xFFFE)
            {
                return _hram[address.GetLowByte() - 0x80];
            }
            else
            {
                return 0xFF;
            }
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            if (address <= 0xCFFF)
            {
                _wram[address - 0xC000] = value;
            }
            else if (address <= 0xDFFF)
            {
                int bank = _svbk;
                if (bank == 0)
                {
                    bank = 1;
                }
                _wram[address - 0xD000 + bank * 0x1000] = value;
            }
            else if (address == 0xFF70)
            {
                _svbk = (byte)(value & 0b111);
            }
            else if (address <= 0xFFFE)
            {
                _hram[address.GetLowByte() - 0x80] = value;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (0xC000, 0xDFFF); // work RAM
            yield return (0xFF70, 0xFF70); // SVBK register
            yield return (0xFF80, 0xFFFE); // high RAM
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_svbk);
            writer.Write(_hram);
            writer.Write(_wram);
        }

        public void SetState(BinaryReader reader)
        {
            _svbk = reader.ReadByte();
            Array.Copy(reader.ReadBytes(_hram.Length), _hram, _hram.Length);
            Array.Copy(reader.ReadBytes(_wram.Length), _wram, _wram.Length);
        }
    }
}
