using System.Collections.Generic;
using Atem.Core.Memory;

namespace Atem.Core
{
    /// <summary>
    /// Represents the main system bus for communication in the emulator. The bus
    /// provides a 64KB addressable space and supports memory-mapped I/O by delegating
    /// read and write operations to registered <see cref="IAddressable"/> objects.
    /// </summary>
    public class Bus : IBus
    {
        public const int Size = ushort.MaxValue + 1;

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
