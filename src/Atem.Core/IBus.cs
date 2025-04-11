using Atem.Core.Memory;

namespace Atem.Core
{
    public interface IBus : IMemoryProvider
    {
        public void Write(ushort address, byte value);
    }
}
