using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core
{
    public interface IBus : IResetable, IMemoryProvider
    {
        Cartridge Cartridge { get; }
        bool ColorMode { get; }
        public void Write(ushort address, byte value, bool ignoreRenderMode = false);
        public bool LoadCartridge(byte[] data);
    }
}
