using Atem.Core.Audio;
using Atem.Core.Processor;

namespace Atem
{
    internal class Bus
    {
        private Processor _processor;
        private PPU _ppu;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private Serial _serial;
        private BootROM _bootROM;
        private Cartridge _cartridge;
        private AudioManager _audioManager;
        private byte[] _hram = new byte[0x7F];
        private byte[] _wram = new byte[0x2000];

        public void SetComponents(Processor processor, PPU ppu, Timer timer, Interrupt interrupt, Joypad joypad, Serial serial, AudioManager audioManager)
        {
            _processor = processor;
            _ppu = ppu;
            _timer = timer;
            _interrupt = interrupt;
            _joypad = joypad;
            _serial = serial;
            _audioManager = audioManager;
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
                return _ppu.ReadVRAM(address);
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
                    return _ppu.ReadOAM(address);
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
                return _audioManager.Registers.NR10;
            }
            else if (offset == 0x11)
            {
                return _audioManager.Registers.NR11;
            }
            else if (offset == 0x12)
            {
                return _audioManager.Registers.NR12;
            }
            else if (offset == 0x13)
            {
                return _audioManager.Registers.NR13;
            }
            else if (offset == 0x14)
            {
                return _audioManager.Registers.NR14;
            }
            else if (offset == 0x16)
            {
                return _audioManager.Registers.NR21;
            }
            else if (offset == 0x17)
            {
                return _audioManager.Registers.NR22;
            }
            else if (offset == 0x18)
            {
                return _audioManager.Registers.NR23;
            }
            else if (offset == 0x19)
            {
                return _audioManager.Registers.NR24;
            }
            else if (offset == 0x1A)
            {
                return _audioManager.Registers.NR30;
            }
            else if (offset == 0x1B)
            {
                return _audioManager.Registers.NR31;
            }
            else if (offset == 0x1C)
            {
                return _audioManager.Registers.NR32;
            }
            else if (offset == 0x1D)
            {
                return _audioManager.Registers.NR33;
            }
            else if (offset == 0x1E)
            {
                return _audioManager.Registers.NR34;
            }
            else if (offset == 0x20)
            {
                return _audioManager.Registers.NR41;
            }
            else if (offset == 0x21)
            {
                return _audioManager.Registers.NR42;
            }
            else if (offset == 0x22)
            {
                return _audioManager.Registers.NR43;
            }
            else if (offset == 0x23)
            {
                return _audioManager.Registers.NR44;
            }
            else if (offset == 0x24)
            {
                return _audioManager.Registers.NR50;
            }
            else if (offset == 0x25)
            {
                return _audioManager.Registers.NR51;
            }
            else if (offset == 0x26)
            {
                return _audioManager.Registers.NR52;
            }
            else if (offset >= 27 && offset <= 0x2F)
            {

            }
            else if (offset >= 0x30 && offset <= 0x3F)
            {
                return _audioManager.ReadWaveRAM((byte)(offset - 0x30));
            }
            else if (offset == 0x40)
            {
                return _ppu.LCDC;
            }
            else if (offset == 0x41)
            {
                return _ppu.STAT;
            }
            else if (offset == 0x42)
            {
                return _ppu.SCY;
            }
            else if (offset == 0x43)
            {
                return _ppu.SCX;
            }
            else if (offset == 0x44)
            {
                return _ppu.LY;
            }
            else if (offset == 0x45)
            {
                return _ppu.LYC;
            }
            else if (offset == 0x46)
            {
                return _ppu.DMA;
            }
            else if (offset == 0x47)
            {
                return _ppu.BGP;
            }
            else if (offset == 0x48)
            {
                return _ppu.OBP0;
            }
            else if (offset == 0x49)
            {
                return _ppu.OBP1;
            }
            else if (offset == 0x4A)
            {
                return _ppu.WY;
            }
            else if (offset == 0x4B)
            {
                return _ppu.WX;
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
                _audioManager.Registers.NR10 = value;
            }
            else if (offset == 0x11)
            {
                _audioManager.Registers.NR11 = value;
            }
            else if (offset == 0x12)
            {
                _audioManager.Registers.NR12 = value;
            }
            else if (offset == 0x13)
            {
                _audioManager.Registers.NR13 = value;
            }
            else if (offset == 0x14)
            {
                _audioManager.Registers.NR14 = value;
            }
            else if (offset == 0x16)
            {
                _audioManager.Registers.NR21 = value;
            }
            else if (offset == 0x17)
            {
                _audioManager.Registers.NR22 = value;
            }
            else if (offset == 0x18)
            {
                _audioManager.Registers.NR23 = value;
            }
            else if (offset == 0x19)
            {
                _audioManager.Registers.NR24 = value;
            }
            else if (offset == 0x1A)
            {
                _audioManager.Registers.NR30 = value;
            }
            else if (offset == 0x1B)
            {
                _audioManager.Registers.NR31 = value;
            }
            else if (offset == 0x1C)
            {
                _audioManager.Registers.NR32 = value;
            }
            else if (offset == 0x1D)
            {
                _audioManager.Registers.NR33 = value;
            }
            else if (offset == 0x1E)
            {
                _audioManager.Registers.NR34 = value;
            }
            else if (offset == 0x20)
            {
                _audioManager.Registers.NR41 = value;
            }
            else if (offset == 0x21)
            {
                _audioManager.Registers.NR42 = value;
            }
            else if (offset == 0x22)
            {
                _audioManager.Registers.NR43 = value;
            }
            else if (offset == 0x23)
            {
                _audioManager.Registers.NR44 = value;
            }
            else if (offset == 0x24)
            {
                _audioManager.Registers.NR50 = value;
            }
            else if (offset == 0x25)
            {
                _audioManager.Registers.NR51 = value;
            }
            else if (offset == 0x26)
            {
                _audioManager.Registers.NR52 = value;
            }
            else if (offset >= 0x27 && offset <= 0x2F)
            {

            }
            else if (offset >= 0x30 && offset <= 0x3F)
            {
                _audioManager.WriteWaveRAM((byte)(offset & 0xF), value);
            }
            else if (offset == 0x40)
            {
                _ppu.LCDC = value;
            }
            else if (offset == 0x41)
            {
                _ppu.STAT = value;
            }
            else if (offset == 0x42)
            {
                _ppu.SCY = value;
            }
            else if (offset == 0x43)
            {
                _ppu.SCX = value;
            }
            else if (offset == 0x44)
            {

            }
            else if (offset == 0x45)
            {
                _ppu.LYC = value;
            }
            else if (offset == 0x46)
            {
                _ppu.DMA = value;
            }
            else if (offset == 0x47)
            {
                _ppu.BGP = value;
            }
            else if (offset == 0x48)
            {
                _ppu.OBP0 = value;
            }
            else if (offset == 0x49)
            {
                _ppu.OBP1 = value;
            }
            else if (offset == 0x4A)
            {
                _ppu.WY = value;
            }
            else if (offset == 0x4B)
            {
                _ppu.WX = value;
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
                _ppu.WriteVRAM(address, value);
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
                    _ppu.WriteOAM(address, value);
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
