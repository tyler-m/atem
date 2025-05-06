using System.Collections.Generic;

namespace Atem.Core.Memory
{
    public class NullMemoryProvider : IAddressable
    {
        public byte Read(ushort address, bool ignoreAccessRestrictions = false) => 0xFF;

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false) { }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (ushort.MinValue, ushort.MaxValue);
        }
    }
}
