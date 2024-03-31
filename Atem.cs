using static Atem.PPU;

namespace Atem
{
    internal class Atem
    {
        private const int NumberOfClocksPerFrame = 70224;
        private int _clockCost = 4;

        private CPU _cpu;
        private PPU _ppu;
        private Bus _bus;

        public event VerticalBlankEvent OnVerticalBlank
        {
            add
            {
                _ppu.OnVerticalBlank += value;
            }
            remove
            {
                _ppu.OnVerticalBlank -= value;
            }
        }

        public Atem()
        {
            _ppu = new PPU();
            _bus = new Bus(_ppu);
            _cpu = new CPU(_bus);

            _bus.LoadBootROM("BOOT.bin");
            _bus.LoadCartridge("Game.gb");
        }

        public void Update()
        {
            for (int i = 0; i < NumberOfClocksPerFrame; i += _clockCost) {
                Clock();
            }
        }

        public void ClockCPUOneOp()
        {
            while (!_cpu.Clock()) ;
        }

        public void Clock()
        {
            _cpu.Clock();
            _ppu.Clock();
        }
    }
}
