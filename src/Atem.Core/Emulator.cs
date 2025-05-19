using System.IO;
using Atem.Core.Audio;
using Atem.Core.Debugging;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    /// <summary>
    /// Represents the Game Boy emulator, holding all major subsystems
    /// including CPU, memory, graphics, audio, input, timers, and serial I/O.
    /// This class drives the emulation loop and faciliates state serialization,
    /// cartridge loading, and system reset behavior.
    /// </summary>
    public class Emulator : IEmulator
    {
        private const float ClocksPerFrame = Processor.FREQUENCY / GraphicsManager.FrameRate;

        private readonly int _clockCost = 4;
        private double _leftoverClocks;
        private bool _forceClock;
        private byte[] _resetState = new byte[0x40000];

        public Interrupt Interrupt { get; private set; }
        public Timer Timer { get; private set; }
        public GraphicsManager Graphics { get; private set; }
        public SystemMemory SystemMemory { get; private set; }
        public Debugger Debugger { get; private set; }
        public Bus Bus { get; private set; }
        public Processor Processor { get; private set; }
        public AudioManager Audio { get; private set; }
        public Joypad Joypad { get; private set; }
        public ICartridge Cartridge { get; private set; }
        public ISerialManager Serial { get; private set; }
        public bool Paused { get; set; }

        public event VerticalBlankEvent OnVerticalBlank
        {
            add => Graphics.OnVerticalBlank += value;
            remove => Graphics.OnVerticalBlank -= value;
        }

        public Emulator(
            Bus bus,
            Processor processor,
            Interrupt interrupt,
            Joypad joypad,
            Timer timer,
            ISerialManager serial,
            AudioManager audio,
            Cartridge cartridge,
            GraphicsManager graphics,
            SystemMemory systemMemory)
        {
            Bus = bus;
            Processor = processor;
            Interrupt = interrupt;
            Joypad = joypad;
            Timer = timer;
            Serial = serial;
            Audio = audio;
            Cartridge = cartridge;
            Graphics = graphics;
            SystemMemory = systemMemory;

            Bus.AddAddressables([processor, interrupt, joypad, timer, serial, audio, cartridge, graphics, systemMemory]);

            Debugger = new Debugger();

            GetResetState();
        }

        private void GetResetState()
        {
            using MemoryStream stream = new(_resetState);
            using BinaryWriter writer = new(stream);
            GetState(writer);
        }

        private void SetResetState()
        {
            // before we restore the emulator to its reset state, we need
            // to make sure the cartridge is using NullMap as its mapper
            Cartridge.ResetMapper();

            using MemoryStream stream = new(_resetState);
            using BinaryReader reader = new(stream);
            SetState(reader);
        }

        public void Continue()
        {
            Paused = false;
            _forceClock = true;
        }

        public bool LoadCartridge(byte[] data)
        {
            SetResetState();

            bool loaded = Cartridge.Load(data);
            if (loaded)
            {
                BootMode mode = BootMode.DMG;

                if (Cartridge.SupportsColor)
                {
                    mode = BootMode.CGB;
                }

                Processor.Registers.Boot(mode);
                Joypad.Boot(mode);
                Serial.Boot(mode);
                Timer.Boot(mode);
                Audio.Registers.Boot(mode);
                Graphics.Registers.Boot(mode);
            }
            return loaded;
        }

        public void Update()
        {
            int additionalClocks = (int)(_leftoverClocks / _clockCost);
            float clocksForCurrentFrame = ClocksPerFrame + additionalClocks;

            for (int i = 0; i < clocksForCurrentFrame; i += _clockCost)
            {
                if (Debugger.Active && !Paused)
                {
                    // we don't pause if we're forcing a clock
                    // this prevents breaking on the same breakpoint repeatedly
                    if (!_forceClock && Debugger.CheckBreakpoints(Processor.Registers.PC))
                    {
                        Paused = true;
                    }
                }

                if (!Paused)
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

            bool opFinished = Processor.Clock();
            Timer.Clock();
            Serial.Clock();

            if (Processor.DoubleSpeed)
            {
                opFinished |= Processor.Clock();
                Timer.Clock();
                Serial.Clock();
            }

            Graphics.Clock();
            Audio.Clock();
            return opFinished;
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_leftoverClocks);
            Processor.GetState(writer);
            Timer.GetState(writer);
            Interrupt.GetState(writer);
            Joypad.GetState(writer);
            Serial.GetState(writer);
            Graphics.GetState(writer);
            Audio.GetState(writer);
            Cartridge.GetState(writer);
            SystemMemory.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _leftoverClocks = reader.ReadDouble();
            Processor.SetState(reader);
            Timer.SetState(reader);
            Interrupt.SetState(reader);
            Joypad.SetState(reader);
            Serial.SetState(reader);
            Graphics.SetState(reader);
            Audio.SetState(reader);
            Cartridge.SetState(reader);
            SystemMemory.SetState(reader);
        }
    }
}
