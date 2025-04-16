using Atem.Core.Audio;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    public interface IBus : IResetable, IMemoryProvider
    {
        AudioManager Audio { get; }
        Cartridge Cartridge { get; }
        bool ColorMode { get; }
        public void Write(ushort address, byte value, bool ignoreRenderMode = false);
        public bool LoadCartridge(byte[] data);
    }
}
