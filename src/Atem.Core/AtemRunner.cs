using Atem.Core.Graphics;
using Atem.Core.Processing;
using Atem.Core.Audio;
using Atem.Core.Input;

namespace Atem.Core
{
    public class AtemRunner
    {
        private static float ClocksPerFrame => Processor.Frequency / GraphicsManager.FrameRate;

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

            ViewHelper = new ViewHelper(_processor, _bus, _graphics);
        }

        private void PrepareForGameBoot(bool color)
        {
            if (color)
        {
            // CGB Mode
            _processor.Registers.A = 0x11;
            _processor.Registers.Flags.Z = true;
            _processor.Registers.B = 0x00;
            _processor.Registers.C = 0x00;
            _processor.Registers.D = 0xFF;
            _processor.Registers.E = 0x56;
            _processor.Registers.H = 0x00;
            _processor.Registers.L = 0x0D;
            _processor.Registers.PC = 0x0100;
            _processor.Registers.SP = 0xFFFE;

            _joypad.P1 = 0xC7;
            _serial.SB = 0x00;
            _serial.SC = 0x7F;
            _timer.TIMA = 0x00;
            _timer.TMA = 0x00;
            _timer.TAC = 0xF8;
            _interrupt.IF = 0xE1;
            _interrupt.IE = 0x00;
            _audio.Registers.NR10 = 0x80;
            _audio.Registers.NR11 = 0xBF;
            _audio.Registers.NR12 = 0xF3;
            _audio.Registers.NR13 = 0xFF;
            _audio.Registers.NR14 = 0xBF;
            _audio.Registers.NR21 = 0x3F;
            _audio.Registers.NR22 = 0x00;
            _audio.Registers.NR23 = 0xFF;
            _audio.Registers.NR24 = 0xBF;
            _audio.Registers.NR30 = 0x7F;
            _audio.Registers.NR31 = 0xFF;
            _audio.Registers.NR32 = 0x9F;
            _audio.Registers.NR33 = 0xFF;
            _audio.Registers.NR34 = 0xBF;
            _audio.Registers.NR41 = 0xFF;
            _audio.Registers.NR42 = 0x00;
            _audio.Registers.NR43 = 0x00;
            _audio.Registers.NR44 = 0xBF;
            _audio.Registers.NR50 = 0x77;
            _audio.Registers.NR51 = 0xF3;
            _audio.Registers.NR52 = 0xF1;
            _graphics.Registers.LCDC = 0x91;
            _graphics.Registers.SCY = 0x00;
            _graphics.Registers.SCX = 0x00;
            _graphics.Registers.LYC = 0x00;
            _graphics.Registers.BGP = 0xFC;
            _graphics.Registers.OBP0 = 0x00;
            _graphics.Registers.OBP1 = 0x00;
            _graphics.Registers.WY = 0x00;
            _graphics.Registers.WX = 0x00;
        }
            else
            {
                // DMG mode
                _processor.Registers.A = 0x01;
                _processor.Registers.Flags.Z = true;
                _processor.Registers.B = 0x00;
                _processor.Registers.C = 0x13;
                _processor.Registers.D = 0x00;
                _processor.Registers.E = 0xD8;
                _processor.Registers.H = 0x01;
                _processor.Registers.L = 0x4D;
                _processor.Registers.PC = 0x0100;
                _processor.Registers.SP = 0xFFFE;

                _joypad.P1 = 0xCF;
                _serial.SB = 0x00;
                _serial.SC = 0x7E;
                _timer.TIMA = 0x00;
                _timer.TMA = 0x00;
                _timer.TAC = 0xF8;
                _interrupt.IF = 0xE1;
                _interrupt.IE = 0x00;
                _audio.Registers.NR10 = 0x80;
                _audio.Registers.NR11 = 0xBF;
                _audio.Registers.NR12 = 0xF3;
                _audio.Registers.NR13 = 0xFF;
                _audio.Registers.NR14 = 0xBF;
                _audio.Registers.NR21 = 0x3F;
                _audio.Registers.NR22 = 0x00;
                _audio.Registers.NR23 = 0xFF;
                _audio.Registers.NR24 = 0xBF;
                _audio.Registers.NR30 = 0x7F;
                _audio.Registers.NR31 = 0xFF;
                _audio.Registers.NR32 = 0x9F;
                _audio.Registers.NR33 = 0xFF;
                _audio.Registers.NR34 = 0xBF;
                _audio.Registers.NR41 = 0xFF;
                _audio.Registers.NR42 = 0x00;
                _audio.Registers.NR43 = 0x00;
                _audio.Registers.NR44 = 0xBF;
                _audio.Registers.NR50 = 0x77;
                _audio.Registers.NR51 = 0xF3;
                _audio.Registers.NR52 = 0xF1;
                _graphics.Registers.LCDC = 0x91;
                _graphics.Registers.SCY = 0x00;
                _graphics.Registers.SCX = 0x00;
                _graphics.Registers.LY = 0x00;
                _graphics.Registers.LYC = 0x00;
                _graphics.Registers.BGP = 0xFC;
                _graphics.Registers.OBP0 = 0x00;
                _graphics.Registers.OBP1 = 0x00;
                _graphics.Registers.WY = 0x00;
                _graphics.Registers.WX = 0x00;
            }
        }

        public bool Load(string path)
        {
            bool color = path.ToLower().EndsWith(".gbc");
            bool loaded = _bus.LoadCartridge(path);
            if (loaded)
            {
                PrepareForGameBoot(color);
            }
            return loaded;
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
