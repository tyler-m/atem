using Atem.Core.Audio;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    public interface IBus : IResetable, IMemoryProvider
    {
        IProcessor Processor { get; }
        GraphicsManager Graphics { get; }
        Serial Serial { get; }
        Timer Timer { get; }
         AudioManager Audio { get; }
        Cartridge Cartridge { get; }
        bool ColorMode { get; }
        public void Write(ushort address, byte value);
        public bool LoadCartridge(byte[] data);
    }
}
