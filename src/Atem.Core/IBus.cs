using Atem.Core.Audio;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;

namespace Atem.Core
{
    public interface IBus : IMemoryProvider
    {
        IProcessor Processor { get; }
        GraphicsManager Graphics { get; }
        Joypad Joypad { get; }
        Serial Serial { get; }
        Timer Timer { get; }
        Interrupt Interrupt { get; }
        AudioManager Audio { get; }
        Cartridge Cartridge { get; }
        byte[] HRAM { get; }
        byte[] WRAM { get; }
        public void Write(ushort address, byte value);
        public bool LoadCartridge(byte[] data);
    }
}
