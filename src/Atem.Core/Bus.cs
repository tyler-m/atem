using System.Collections.Generic;
using System.IO;
using Atem.Core.Audio;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.State;

namespace Atem.Core
{
    public class Bus : IBus, IStateful
    {
        public const int Size = 0x10000;

        private IProcessor _processor;
        private GraphicsManager _graphics;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private ISerialManager _serial;
        private Cartridge _cartridge;
        private AudioManager _audio;
        private SystemMemory _systemMemory;

        private readonly IAddressable[] _memoryMap = new IAddressable[Size];

        public void ProvideDependencies(Processor processor, Interrupt interrupt, Joypad joypad, Timer timer, ISerialManager serial, GraphicsManager graphics, AudioManager audio, Cartridge cartridge)
        {
            _interrupt = interrupt;
            _joypad = joypad;
            _timer = timer;
            _serial = serial;
            _graphics = graphics;
            _audio = audio;
            _cartridge = cartridge;
            _processor = processor;
            _systemMemory = new SystemMemory();

            AddToMemoryMap(new NullMemoryProvider());
            AddToMemoryMap(_interrupt);
            AddToMemoryMap(_joypad);
            AddToMemoryMap(_timer);
            AddToMemoryMap(_serial);
            AddToMemoryMap(_graphics);
            AddToMemoryMap(_audio);
            AddToMemoryMap(_systemMemory);
            AddToMemoryMap(_cartridge);
            AddToMemoryMap(_processor);
        }

        private void AddToMemoryMap(IAddressable addressable)
        {
            foreach ((ushort startAddress, ushort endAddress) in addressable.GetAddressRanges())
            {
                for (int address = startAddress; address <= endAddress; address++)
                {
                    _memoryMap[address] = addressable;
                }
            }
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return _memoryMap[address].Read(address, ignoreAccessRestrictions);
        }
        public void Write(ushort address, byte value, bool ignoreRenderMode = false)
        {
            _memoryMap[address].Write(address, value, ignoreRenderMode);
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (ushort.MinValue, ushort.MaxValue);
        }

        public void GetState(BinaryWriter writer)
        {
            _processor.GetState(writer);
            _timer.GetState(writer);
            _interrupt.GetState(writer);
            _joypad.GetState(writer);
            _serial.GetState(writer);
            _graphics.GetState(writer);
            _audio.GetState(writer);
            _cartridge.GetState(writer);
            _systemMemory.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _processor.SetState(reader);
            _timer.SetState(reader);
            _interrupt.SetState(reader);
            _joypad.SetState(reader);
            _serial.SetState(reader);
            _graphics.SetState(reader);
            _audio.SetState(reader);
            _cartridge.SetState(reader);
            _systemMemory.SetState(reader);
        }
    }
}
