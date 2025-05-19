using Atem.Core.Audio;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core;

namespace Atem.Factories
{
    public static class EmulatorFactory
    {

        public static Emulator Create()
        {
            Bus bus = new();
            Interrupt interrupt = new();
            Processor processor = new(bus, interrupt);
            Joypad joypad = new(interrupt);
            Timer timer = new(interrupt);
            SerialManager serial = new(interrupt);
            AudioManager audio = new();
            Cartridge cartridge = new();
            GraphicsManager graphics = GraphicsManagerFactory.Create(bus, interrupt, cartridge);
            SystemMemory systemMemory = new();
            return new Emulator(bus, processor, interrupt, joypad, timer, serial, audio, cartridge, graphics, systemMemory);
        }
    }
}
