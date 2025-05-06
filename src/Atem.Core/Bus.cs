using System.Collections.Generic;
using Atem.Core.Memory;

namespace Atem.Core
{
    public class Bus : IBus
    {
        public const int Size = 0x10000;

        private readonly IAddressable[] _memoryMap = new IAddressable[Size];

        public Bus()
        {
            AddToMemoryMap(new NullMemoryProvider());
        }

        public void AddAddressables(IEnumerable<IAddressable> addressables)
        {
            foreach (IAddressable addressable in addressables)
            {
                AddToMemoryMap(addressable);
            }
        }

        private void AddToMemoryMap(IAddressable addressable)
        {
            foreach ((ushort startAddress, ushort endAddress) in addressable.GetAddressRanges())
            {
                for (int address = startAddress; address <= endAddress; address++)
                {
                    _memoryMap[address] = addressable;
                }
            }
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return _memoryMap[address].Read(address, ignoreAccessRestrictions);
        }
        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            _memoryMap[address].Write(address, value, ignoreAccessRestrictions);
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (ushort.MinValue, ushort.MaxValue);
        }
    }
}
