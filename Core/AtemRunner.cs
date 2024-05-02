using Atem.Core.Graphics;
using Atem.Core.Processing;
using Atem.Core.Audio;
using Atem.Core.Input;

namespace Atem.Core
{
    internal class AtemRunner
    {
        private static float ClocksPerFrame
        {
            get
            {
                return Processor.Frequency / GraphicsManager.FrameRate;
            }
        }

        private float _leftoverClocks = 0.0f;
        private int _clockCost = 4;

        private Processor _processor;
        private GraphicsManager _graphics;
        private Bus _bus;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private Serial _serial;
        private AudioManager _audio;

        public ViewHelper ViewHelper { get; private set; }

        public event AudioManager.FullAudioBufferEvent OnFullAudioBuffer
        {
            add
            {
                _audio.OnFullBuffer += value;
            }
            remove
            {
                _audio.OnFullBuffer -= value;
            }
        }

        public event VerticalBlankEvent OnVerticalBlank
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

        public AtemRunner()
        {
            _bus = new Bus();
            _processor = new Processor(_bus);
            _graphics = new GraphicsManager(_bus);
            _timer = new Timer(_bus);
            _interrupt = new Interrupt();
            _joypad = new Joypad(_bus);
            _serial = new Serial();
            _audio = new AudioManager();
            _bus.SetComponents(_processor, _graphics, _timer, _interrupt, _joypad, _serial, _audio);

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
            _timer.Clock();

            if (_processor.DoubleSpeed)
            {
                opFinished |= _processor.Clock();
                _timer.Clock();
            }

            _graphics.Clock();
            _audio.Clock();
            return opFinished;
        }

        public void OnJoypadChange(JoypadButton button, bool down)
        {
            _joypad.OnJoypadChange(button, down);
        }

        public void OnExit()
        {
            _bus.SaveCartridgeRAM();
        }
    }
}
