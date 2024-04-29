using Atem.Core.Graphics;
using Atem.Core.Processor;
using Atem.Core.Audio;

namespace Atem
{
    internal class Atem
    {
        private const int ClockFrequency = 4194304;
        private const float FrameFrequency = 59.73f;
        private const float ClocksPerFrame = ClockFrequency / FrameFrequency;

        private float _leftoverClocks = 0.0f;
        private int _clockCost = 4;

        private Processor _processor;
        private Graphics _graphics;
        private Bus _bus;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private Serial _serial;
        private AudioManager _audioManager;

        public ViewHelper ViewHelper { get; private set; }

        public event AudioManager.FullAudioBufferEvent OnFullAudioBuffer
        {
            add
            {
                _audioManager.OnFullBuffer += value;
            }
            remove
            {
                _audioManager.OnFullBuffer -= value;
            }
        }

        public event Graphics.VerticalBlankEvent OnVerticalBlank
        {
            add
            {
                _graphics.OnVerticalBlank += value;
            }
            remove
            {
                _graphics.OnVerticalBlank -= value;
            }
        }

        public Atem()
        {
            _bus = new Bus();
            _processor = new Processor(_bus);
            _graphics = new Graphics(_bus);
            _timer = new Timer(_bus);
            _interrupt = new Interrupt();
            _joypad = new Joypad(_bus);
            _serial = new Serial();
            _audioManager = new AudioManager();
            _bus.SetComponents(_processor, _graphics, _timer, _interrupt, _joypad, _serial, _audioManager);

            _bus.LoadBootROM("BOOT.bin");
            _bus.LoadCartridge("Game.gb");

            ViewHelper = new ViewHelper(_processor, _bus);
        }

        public void Update()
        {
            int additionalClocks = (int)(_leftoverClocks / _clockCost);
            float clocksForCurrentFrame = ClocksPerFrame + additionalClocks;

            for (int i = 0; i < clocksForCurrentFrame; i += _clockCost)
            {
                Clock();
            }

            _leftoverClocks += ClocksPerFrame - (int)ClocksPerFrame - _clockCost * additionalClocks;
        }

        public void ClockOneOperation()
        {
            while (!Clock()) ;
        }

        public bool Clock()
        {
            bool opFinished = _processor.Clock();
            _graphics.Clock();
            _audioManager.Clock();
            _timer.Clock();
            return opFinished;
        }

        internal void OnJoypadChange(JoypadButton button, bool down)
        {
            _joypad.OnJoypadChange(button, down);
        }
    }
}
