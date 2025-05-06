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
            Processor processor = new(bus);
            Interrupt interrupt = new();
            Joypad joypad = new(interrupt);
            Timer timer = new(interrupt);
            SerialManager serial = new(interrupt);
            AudioManager audio = new();
            Cartridge cartridge = new();
            GraphicsManager graphics = GraphicsManagerFactory.Create(bus, interrupt, cartridge);
            bus.ProvideDependencies(processor, interrupt, joypad, timer, serial, graphics, audio, cartridge);
            return new Emulator(bus, processor, interrupt, joypad, timer, serial, audio, cartridge, graphics);
        }
    }
}
