using System.Collections.Generic;

namespace Atem.Core.Memory
{
    public class NullMemoryProvider : IMemoryProvider
    {
        public byte Read(ushort address, bool ignoreAccessRestrictions = false) => 0xFF;

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false) { }

        public IEnumerable<(ushort Start, ushort End)> GetMemoryRanges()
        {
            yield return (ushort.MinValue, ushort.MaxValue);
        }
    }
}
