using System.IO;
using Atem.Core.Audio;
using Atem.Core.Debugging;
using Atem.Core.Graphics;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    public class AtemRunner : IResetable, IStateful
    {
        private static float ClocksPerFrame => Processor.FREQUENCY / GraphicsManager.FrameRate;

        private readonly Processor _processor;
        private readonly Bus _bus;
        private readonly Interrupt _interrupt;
        private readonly Joypad _joypad;
        private readonly Timer _timer;
        private readonly Serial _serial;
        private readonly GraphicsManager _graphics;
        private readonly AudioManager _audio;
        private readonly Cartridge _cartridge;
        private readonly Debugger _debugger;
        private readonly int _clockCost = 4;
        private double _leftoverClocks;
        private bool _forceClock;
        private bool _paused;

        public Debugger Debugger => _debugger;
        public Bus Bus => _bus;
        public Processor Processor => _processor;
        public AudioManager Audio => _audio;
        public Cartridge Cartridge => _cartridge;
        public bool Paused { get => _paused; set => _paused = value; }

        public event VerticalBlankEvent OnVerticalBlank
        {
            add => _graphics.OnVerticalBlank += value;
            remove => _graphics.OnVerticalBlank -= value;
        }

        public AtemRunner()
        {
            _bus = new Bus();

            _processor = new Processor(_bus);
            _interrupt = new Interrupt();
            _joypad = new Joypad(_interrupt);
            _timer = new Timer(_interrupt);
            _serial = new Serial();
            _audio = new AudioManager();
            _cartridge = new Cartridge();

            RenderModeScheduler renderModeScheduler = new();
            PaletteProvider paletteProvider = new();
            HDMA hdma = new(_bus, renderModeScheduler);
            StatInterruptDispatcher statInterruptDispatcher = new(_interrupt, renderModeScheduler);
            TileManager tileManager = new(renderModeScheduler, paletteProvider, _cartridge);
            ObjectManager objectManager = new(_bus, renderModeScheduler, tileManager, paletteProvider, _cartridge);
            ScreenManager screenManager = new(_bus, renderModeScheduler, tileManager, objectManager, _cartridge);
            _graphics = new GraphicsManager(_interrupt, renderModeScheduler, paletteProvider, hdma, statInterruptDispatcher, tileManager, objectManager, screenManager);

            _bus.ProvideDependencies(_processor, _interrupt, _joypad, _timer, _serial, _graphics, _audio, _cartridge);

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

        public bool LoadCartridge(byte[] data, bool color)
        {
            Reset();

            bool loaded = _cartridge.Load(data);
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
                    if (!_forceClock && _debugger.CheckBreakpoints(_processor.Registers.PC))
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
