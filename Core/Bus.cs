using Atem.Core.Audio;
using Atem.Core.Processing;
using Atem.Core.Graphics;

namespace Atem
{
    internal class Bus
    {
        private Processor _processor;
        private GraphicsManager _graphics;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private Serial _serial;
        private BootROM _bootROM;
        private Cartridge _cartridge;
        private AudioManager _audio;
        private byte[] _hram = new byte[0x7F];
        private byte[] _wram = new byte[0x2000];

        public void SetComponents(Processor processor, GraphicsManager graphics, Timer timer, Interrupt interrupt, Joypad joypad, Serial serial, AudioManager audio)
        {
            _processor = processor;
            _graphics = graphics;
            _timer = timer;
            _interrupt = interrupt;
            _joypad = joypad;
            _serial = serial;
            _audio = audio;
        }

        public void LoadBootROM(string filepath, bool enabled = true)
        {
            _bootROM = new BootROM(filepath, enabled);
        }

        public void LoadCartridge(string filepath)
        {
            _cartridge = new Cartridge(filepath);
        }

        public byte Read(ushort address)
        {
            byte block = address.GetHighByte();
            byte offset = address.GetLowByte();

            if (block <= 0x00 && _bootROM.Enabled) // boot ROM
            {
                return _bootROM.Read(address);
            }
            else if (block <= 0x7F) // cartridge ROM
            {
                return _cartridge.ReadROM(address);
            }
            else if (block <= 0x9F) // VRAM
            {
                return _graphics.ReadVRAM(address);
            }
            else if (block <= 0xBF) // cartridge RAM
            {
                return _cartridge.ReadRAM(address);
            }
            else if (block <= 0xDF) // WRAM
            {
                return _wram[address - 0xC000];
            }
            else if (block <= 0xFD) // echo RAM
            {

            }
            else if (block <= 0xFE)
            {
                if (offset <= 0x9F) // OAM
                {
                    return _graphics.ReadOAM(address);
                }
            }
            else if (block <= 0xFF)
            {
                return ReadIO(offset);
            }

            return 0xFF;
        }

        private byte ReadIO(byte offset)
        {
            if (offset == 0x00)
            {
                return _joypad.JOYP;
            }
            else if (offset == 0x01)
            {
                return _serial.SC;
            }
            else if (offset == 0x02)
            {
                return _serial.SB;
            }
            else if (offset == 0x04)
            {
                return _timer.DIV;
            }
            else if (offset == 0x05)
            {
                return _timer.TIMA;
            }
            else if (offset == 0x06)
            {
                return _timer.TMA;
            }
            else if (offset == 0x07)
            {
                return _timer.TAC;
            }
            else if (offset == 0x0F)
            {
                return _interrupt.IF;
            }
            else if (offset == 0x10)
            {
                return _audio.Registers.NR10;
            }
            else if (offset == 0x11)
            {
                return _audio.Registers.NR11;
            }
            else if (offset == 0x12)
            {
                return _audio.Registers.NR12;
            }
            else if (offset == 0x13)
            {
                return _audio.Registers.NR13;
            }
            else if (offset == 0x14)
            {
                return _audio.Registers.NR14;
            }
            else if (offset == 0x16)
            {
                return _audio.Registers.NR21;
            }
            else if (offset == 0x17)
            {
                return _audio.Registers.NR22;
            }
            else if (offset == 0x18)
            {
                return _audio.Registers.NR23;
            }
            else if (offset == 0x19)
            {
                return _audio.Registers.NR24;
            }
            else if (offset == 0x1A)
            {
                return _audio.Registers.NR30;
            }
            else if (offset == 0x1B)
            {
                return _audio.Registers.NR31;
            }
            else if (offset == 0x1C)
            {
                return _audio.Registers.NR32;
            }
            else if (offset == 0x1D)
            {
                return _audio.Registers.NR33;
            }
            else if (offset == 0x1E)
            {
                return _audio.Registers.NR34;
            }
            else if (offset == 0x20)
            {
                return _audio.Registers.NR41;
            }
            else if (offset == 0x21)
            {
                return _audio.Registers.NR42;
            }
            else if (offset == 0x22)
            {
                return _audio.Registers.NR43;
            }
            else if (offset == 0x23)
            {
                return _audio.Registers.NR44;
            }
            else if (offset == 0x24)
            {
                return _audio.Registers.NR50;
            }
            else if (offset == 0x25)
            {
                return _audio.Registers.NR51;
            }
            else if (offset == 0x26)
            {
                return _audio.Registers.NR52;
            }
            else if (offset >= 27 && offset <= 0x2F)
            {

            }
            else if (offset >= 0x30 && offset <= 0x3F)
            {
                return _audio.ReadWaveRAM((byte)(offset - 0x30));
            }
            else if (offset == 0x40)
            {
                return _graphics.LCDC;
            }
            else if (offset == 0x41)
            {
                return _graphics.STAT;
            }
            else if (offset == 0x42)
            {
                return _graphics.SCY;
            }
            else if (offset == 0x43)
            {
                return _graphics.SCX;
            }
            else if (offset == 0x44)
            {
                return _graphics.LY;
            }
            else if (offset == 0x45)
            {
                return _graphics.LYC;
            }
            else if (offset == 0x46)
            {
                return _graphics.DMA;
            }
            else if (offset == 0x47)
            {
                return _graphics.BGP;
            }
            else if (offset == 0x48)
            {
                return _graphics.OBP0;
            }
            else if (offset == 0x49)
            {
                return _graphics.OBP1;
            }
            else if (offset == 0x4A)
            {
                return _graphics.WY;
            }
            else if (offset == 0x4B)
            {
                return _graphics.WX;
            }
            else if (offset >= 0x78 && offset <= 0x7F) // unused?
            {

            }
            else if (offset >= 0x80 && offset <= 0xFE)
            {
                return _hram[offset - 0x80];
            }
            else if (offset == 0xFF)
            {
                return _interrupt.IE;
            }
            else
            {
                throw new System.Exception($"ReadIO undefined at offset {offset:X2}.");
            }

            return 0xFF;
        }

        private void WriteIO(byte offset, byte value)
        {
            if (offset == 0x00)
            {
                _joypad.JOYP = value;
            }
            else if (offset == 0x01)
            {
                _serial.SC = value;
            }
            else if (offset == 0x02)
            {
                _serial.SB = value;
            }
            else if (offset == 0x04)
            {
                _timer.DIV = value;
            }
            else if (offset == 0x05)
            {
                _timer.TIMA = value;
            }
            else if (offset == 0x06)
            {
                _timer.TMA = value;
            }
            else if (offset == 0x07)
            {
                _timer.TAC = value;
            }
            else if (offset == 0x0F)
            {
                _interrupt.IF = value;
            }
            else if (offset == 0x10)
            {
                _audio.Registers.NR10 = value;
            }
            else if (offset == 0x11)
            {
                _audio.Registers.NR11 = value;
            }
            else if (offset == 0x12)
            {
                _audio.Registers.NR12 = value;
            }
            else if (offset == 0x13)
            {
                _audio.Registers.NR13 = value;
            }
            else if (offset == 0x14)
            {
                _audio.Registers.NR14 = value;
            }
            else if (offset == 0x16)
            {
                _audio.Registers.NR21 = value;
            }
            else if (offset == 0x17)
            {
                _audio.Registers.NR22 = value;
            }
            else if (offset == 0x18)
            {
                _audio.Registers.NR23 = value;
            }
            else if (offset == 0x19)
            {
                _audio.Registers.NR24 = value;
            }
            else if (offset == 0x1A)
            {
                _audio.Registers.NR30 = value;
            }
            else if (offset == 0x1B)
            {
                _audio.Registers.NR31 = value;
            }
            else if (offset == 0x1C)
            {
                _audio.Registers.NR32 = value;
            }
            else if (offset == 0x1D)
            {
                _audio.Registers.NR33 = value;
            }
            else if (offset == 0x1E)
            {
                _audio.Registers.NR34 = value;
            }
            else if (offset == 0x20)
            {
                _audio.Registers.NR41 = value;
            }
            else if (offset == 0x21)
            {
                _audio.Registers.NR42 = value;
            }
            else if (offset == 0x22)
            {
                _audio.Registers.NR43 = value;
            }
            else if (offset == 0x23)
            {
                _audio.Registers.NR44 = value;
            }
            else if (offset == 0x24)
            {
                _audio.Registers.NR50 = value;
            }
            else if (offset == 0x25)
            {
                _audio.Registers.NR51 = value;
            }
            else if (offset == 0x26)
            {
                _audio.Registers.NR52 = value;
            }
            else if (offset >= 0x27 && offset <= 0x2F)
            {

            }
            else if (offset >= 0x30 && offset <= 0x3F)
            {
                _audio.WriteWaveRAM((byte)(offset & 0xF), value);
            }
            else if (offset == 0x40)
            {
                _graphics.LCDC = value;
            }
            else if (offset == 0x41)
            {
                _graphics.STAT = value;
            }
            else if (offset == 0x42)
            {
                _graphics.SCY = value;
            }
            else if (offset == 0x43)
            {
                _graphics.SCX = value;
            }
            else if (offset == 0x44)
            {

            }
            else if (offset == 0x45)
            {
                _graphics.LYC = value;
            }
            else if (offset == 0x46)
            {
                _graphics.DMA = value;
            }
            else if (offset == 0x47)
            {
                _graphics.BGP = value;
            }
            else if (offset == 0x48)
            {
                _graphics.OBP0 = value;
            }
            else if (offset == 0x49)
            {
                _graphics.OBP1 = value;
            }
            else if (offset == 0x4A)
            {
                _graphics.WY = value;
            }
            else if (offset == 0x4B)
            {
                _graphics.WX = value;
            }
            else if (offset == 0x50)
            {
                _bootROM.Enabled = false;
            }
            else if (offset >= 0x78 && offset <= 0x7F) // unused?
            {

            }
            else if (offset >= 0x80 && offset <= 0xFE)
            {
                _hram[offset - 0x80] = value;
            }
            else if (offset == 0xFF)
            {
                _interrupt.IE = value;
            }
            else
            {
                throw new System.Exception($"WriteIO undefined at offset {offset:X2}.");
            }
        }

        public void Write(ushort address, byte value)
        {
            byte block = address.GetHighByte();
            byte offset = address.GetLowByte();

            if (block <= 0x00 && _bootROM.Enabled) // boot ROM
            {

            }
            else if (block <= 0x7F) // cartridge ROM
            {
                _cartridge.WriteROM(address, value);
            }
            else if (block <= 0x9F) // VRAM
            {
                _graphics.WriteVRAM(address, value);
            }
            else if (block <= 0xBF) // cartridge RAM
            {
                _cartridge.WriteRAM(address, value);
            }
            else if (block <= 0xDF) // WRAM
            {
                _wram[address - 0xC000] = value;
            }
            else if (block <= 0xFD) // echo RAM
            {

            }
            else if (block <= 0xFE)
            {
                if (offset <= 0x9F) // OAM
                {
                    _graphics.WriteOAM(address, value);
                }
            }
            else if (block <= 0xFF)
            {
                WriteIO(offset, value);
            }
        }

        internal void RequestInterrupt(InterruptType interruptType)
        {
            _interrupt.SetInterrupt(interruptType);
        }
    }
}
