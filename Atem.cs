using static Atem.PPU;

namespace Atem
{
    internal class Atem
    {
        private const int ClockFrequency = 4194304;
        private const float FrameFrequency = 59.73f;
        private const float ClocksPerFrame = ClockFrequency / FrameFrequency;

        private float _leftoverClocks = 0.0f;
        private int _clockCost = 4;

        private CPU _cpu;
        private PPU _ppu;
        private Bus _bus;
        private Timer _timer;
        private Interrupt _interrupt;

        public ViewHelper ViewHelper { get; private set; }

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
            _bus = new Bus();
            _cpu = new CPU(_bus);
            _ppu = new PPU(_bus);
            _timer = new Timer();
            _interrupt = new Interrupt();
            _bus.SetComponents(_cpu, _ppu, _timer, _interrupt);

            _bus.LoadBootROM("BOOT.bin");
            _bus.LoadCartridge("Game.gb");

            ViewHelper = new ViewHelper(_cpu, _bus);
        }

        public void Update()
        {
            int additionalClocks = (int)(_leftoverClocks / _clockCost);
            float clocksForCurrentFrame = ClocksPerFrame + additionalClocks;

            for (int i = 0; i < clocksForCurrentFrame; i += _clockCost) {
                Clock();
            }

            _leftoverClocks += ClocksPerFrame - (int)ClocksPerFrame - _clockCost*additionalClocks;
        }


        public void ClockOneCPUOp()
        {
            while (!Clock()) ;
        }

        public bool Clock()
        {
            bool opFinished = _cpu.Clock();
            _ppu.Clock();
            _timer.Clock();
            return opFinished;
        }
    }
}
