using System;

namespace Atem
{
    internal class Atem
    {
        private const int _numberOfClocksPerFrame = 70224;
        private int _clockCost = 4;

        private CPU _cpu;
        private PPU _ppu;
        private Bus _bus;

        public Atem()
        {
            _ppu = new PPU();
            _bus = new Bus(_ppu);
            _cpu = new CPU(_bus);

            _bus.LoadBootROM("BOOT.bin");
        }

        public void Update()
        {
            for (int i = 0; i < _numberOfClocksPerFrame; i += _clockCost) {
                Clock();
            }
        }

        public void Clock()
        {
            _cpu.Clock();
        }
    }
}
