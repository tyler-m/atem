using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core
{
    public interface IBus : IResetable, IMemoryProvider
    {
        public void Write(ushort address, byte value, bool ignoreRenderMode = false);
    }
}
