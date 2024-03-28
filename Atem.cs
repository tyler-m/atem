using System;

namespace Atem
{
    internal class Atem
    {
        private const int _numberOfClocksPerFrame = 70224;
        private int _clockCost = 4;

        private CPU _cpu;
        private Bus _bus;

        public Atem()
        {
            _bus = new Bus();
            _cpu = new CPU(_bus);

            _bus.LoadBootROM("BOOT.bin");
        }

        internal void Update()
        {
            for (int i = 0; i < _numberOfClocksPerFrame; i += _clockCost) {
                Clock();
            }
        }

        internal void Clock()
        {
            _cpu.Clock();
        }
    }
}
