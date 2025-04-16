using System.IO;
using Atem.Core.Debugging;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    public class AtemRunner : IResetable, IStateful
    {
        private static float ClocksPerFrame => Processor.FREQUENCY / GraphicsManager.FrameRate;

        private readonly Bus _bus;
        private readonly Interrupt _interrupt;
        private readonly Joypad _joypad;
        private readonly Timer _timer;
        private readonly Serial _serial;
        private readonly Debugger _debugger;
        private readonly int _clockCost = 4;
        private double _leftoverClocks;
        private bool _forceClock;
        private bool _paused;

        public Debugger Debugger => _debugger;
        public Bus Bus =>  _bus;
        public bool Paused { get => _paused; set => _paused = value; }

        public event VerticalBlankEvent OnVerticalBlank
        {
            add => _bus.Graphics.OnVerticalBlank += value;
            remove => _bus.Graphics.OnVerticalBlank -= value;
        }

        public AtemRunner()
        {
            _interrupt = new Interrupt();
            _joypad = new Joypad(_interrupt);
            _timer = new Timer(_interrupt);
            _serial = new Serial();
            _bus = new Bus(_interrupt, _joypad, _timer, _serial);
            _debugger = new Debugger();
        }

        public void Continue()
        {
            _paused = false;
            _forceClock = true;
        }

        private void PrepareForGameBoot(bool color)
        {
            if (color)
            {
                // CGB Mode
                _bus.Processor.Registers.A = 0x11;
                _bus.Processor.Registers.Flags.Z = true;
                _bus.Processor.Registers.B = 0x00;
                _bus.Processor.Registers.C = 0x00;
                _bus.Processor.Registers.D = 0xFF;
                _bus.Processor.Registers.E = 0x56;
                _bus.Processor.Registers.H = 0x00;
                _bus.Processor.Registers.L = 0x0D;
                _bus.Processor.Registers.PC = 0x0100;
                _bus.Processor.Registers.SP = 0xFFFE;

                _joypad.P1 = 0xC7;
                _serial.SB = 0x00;
                _serial.SC = 0x7F;
                _timer.TIMA = 0x00;
                _timer.TMA = 0x00;
                _timer.TAC = 0xF8;
                _interrupt.IF = 0xE1;
                _interrupt.IE = 0x00;
                _bus.Audio.Registers.NR10 = 0x80;
                _bus.Audio.Registers.NR11 = 0xBF;
                _bus.Audio.Registers.NR12 = 0xF3;
                _bus.Audio.Registers.NR13 = 0xFF;
                _bus.Audio.Registers.NR14 = 0xBF;
                _bus.Audio.Registers.NR21 = 0x3F;
                _bus.Audio.Registers.NR22 = 0x00;
                _bus.Audio.Registers.NR23 = 0xFF;
                _bus.Audio.Registers.NR24 = 0xBF;
                _bus.Audio.Registers.NR30 = 0x7F;
                _bus.Audio.Registers.NR31 = 0xFF;
                _bus.Audio.Registers.NR32 = 0x9F;
                _bus.Audio.Registers.NR33 = 0xFF;
                _bus.Audio.Registers.NR34 = 0xBF;
                _bus.Audio.Registers.NR41 = 0xFF;
                _bus.Audio.Registers.NR42 = 0x00;
                _bus.Audio.Registers.NR43 = 0x00;
                _bus.Audio.Registers.NR44 = 0xBF;
                _bus.Audio.Registers.NR50 = 0x77;
                _bus.Audio.Registers.NR51 = 0xF3;
                _bus.Audio.Registers.NR52 = 0xF1;
                _bus.Graphics.Registers.LCDC = 0x91;
                _bus.Graphics.Registers.SCY = 0x00;
                _bus.Graphics.Registers.SCX = 0x00;
                _bus.Graphics.Registers.LYC = 0x00;
                _bus.Graphics.Registers.BGP = 0xFC;
                _bus.Graphics.Registers.OBP0 = 0x00;
                _bus.Graphics.Registers.OBP1 = 0x00;
                _bus.Graphics.Registers.WY = 0x00;
                _bus.Graphics.Registers.WX = 0x00;
            }
            else
            {
                // DMG mode
                _bus.Processor.Registers.A = 0x01;
                _bus.Processor.Registers.Flags.Z = true;
                _bus.Processor.Registers.B = 0x00;
                _bus.Processor.Registers.C = 0x13;
                _bus.Processor.Registers.D = 0x00;
                _bus.Processor.Registers.E = 0xD8;
                _bus.Processor.Registers.H = 0x01;
                _bus.Processor.Registers.L = 0x4D;
                _bus.Processor.Registers.PC = 0x0100;
                _bus.Processor.Registers.SP = 0xFFFE;

                _joypad.P1 = 0xCF;
                _serial.SB = 0x00;
                _serial.SC = 0x7E;
                _timer.TIMA = 0x00;
                _timer.TMA = 0x00;
                _timer.TAC = 0xF8;
                _interrupt.IF = 0xE1;
                _interrupt.IE = 0x00;
                _bus.Audio.Registers.NR10 = 0x80;
                _bus.Audio.Registers.NR11 = 0xBF;
                _bus.Audio.Registers.NR12 = 0xF3;
                _bus.Audio.Registers.NR13 = 0xFF;
                _bus.Audio.Registers.NR14 = 0xBF;
                _bus.Audio.Registers.NR21 = 0x3F;
                _bus.Audio.Registers.NR22 = 0x00;
                _bus.Audio.Registers.NR23 = 0xFF;
                _bus.Audio.Registers.NR24 = 0xBF;
                _bus.Audio.Registers.NR30 = 0x7F;
                _bus.Audio.Registers.NR31 = 0xFF;
                _bus.Audio.Registers.NR32 = 0x9F;
                _bus.Audio.Registers.NR33 = 0xFF;
                _bus.Audio.Registers.NR34 = 0xBF;
                _bus.Audio.Registers.NR41 = 0xFF;
                _bus.Audio.Registers.NR42 = 0x00;
                _bus.Audio.Registers.NR43 = 0x00;
                _bus.Audio.Registers.NR44 = 0xBF;
                _bus.Audio.Registers.NR50 = 0x77;
                _bus.Audio.Registers.NR51 = 0xF3;
                _bus.Audio.Registers.NR52 = 0xF1;
                _bus.Graphics.Registers.LCDC = 0x91;
                _bus.Graphics.Registers.SCY = 0x00;
                _bus.Graphics.Registers.SCX = 0x00;
                _bus.Graphics.Registers.LY = 0x00;
                _bus.Graphics.Registers.LYC = 0x00;
                _bus.Graphics.Registers.BGP = 0xFC;
                _bus.Graphics.Registers.OBP0 = 0x00;
                _bus.Graphics.Registers.OBP1 = 0x00;
                _bus.Graphics.Registers.WY = 0x00;
                _bus.Graphics.Registers.WX = 0x00;
            }
        }

        public bool LoadCartridge(byte[] data, bool color)
        {
            Reset();

            bool loaded = _bus.LoadCartridge(data);
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
                if (_debugger.Active && !_paused)
                {
                    // we don't pause if we're forcing a clock
                    // this prevents breaking on the same breakpoint repeatedly
                    if (!_forceClock && _debugger.CheckBreakpoints(_bus.Processor.Registers.PC))
                    {
                        _paused = true;
                    }
                }

                if (!_paused)
                {
                    Clock();
                }
                else
                {
                    break;
                }
            }

            _leftoverClocks += ClocksPerFrame - (int)ClocksPerFrame - _clockCost * additionalClocks;
        }

        public bool Clock()
        {
            _forceClock = false;

            bool opFinished = _bus.Processor.Clock();
            _timer.Clock();

            if (_bus.Processor.DoubleSpeed)
            {
                opFinished |= _bus.Processor.Clock();
                _timer.Clock();
            }

            _bus.Graphics.Clock();
            _bus.Audio.Clock();
            return opFinished;
        }

        public void OnJoypadChange(JoypadButton button, bool down)
        {
            _joypad.OnJoypadChange(button, down);
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_leftoverClocks);
            _bus.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _leftoverClocks = reader.ReadDouble();
            _bus.SetState(reader);
        }

        public void Reset()
        {
            _leftoverClocks = 0;
            _forceClock = false;
            _paused = false;
            _bus.Reset();
        }
    }
}
