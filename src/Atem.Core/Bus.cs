using System;
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
        private IProcessor _processor;
        private GraphicsManager _graphics;
        private Timer _timer;
        private Interrupt _interrupt;
        private Joypad _joypad;
        private SerialManager _serial;
        private BootROM _bootROM;
        private Cartridge _cartridge;
        private AudioManager _audio;
        private readonly byte[] _hram = new byte[0x7F];
        private readonly byte[] _wram = new byte[0x2000 * 4];

        public byte SVBK { get; private set; }
        public int MemorySize => 0x10000;

        public void ProvideDependencies(Processor processor, Interrupt interrupt, Joypad joypad, Timer timer, SerialManager serial, GraphicsManager graphics, AudioManager audio, Cartridge cartridge)
        {
            _interrupt = interrupt;
            _joypad = joypad;
            _timer = timer;
            _serial = serial;
            _graphics = graphics;
            _audio = audio;
            _cartridge = cartridge;
            _processor = processor;
        }

        public void LoadBootROM(string filepath, bool enabled = true)
        {
            _bootROM = new BootROM(filepath, enabled);
        }

        public byte Read(ushort address)
        {
            byte block = address.GetHighByte();
            byte offset = address.GetLowByte();

            if (block <= 0x00 && _bootROM != null && _bootROM.Enabled) // boot ROM
            {
                return _bootROM.Read(address);
            }
            else if (block <= 0x7F) // cartridge ROM
            {
                if (_cartridge.Loaded)
                {
                    return _cartridge.ReadROM(address);
                }
                else
                {
                    return 0x00;
                }
            }
            else if (block <= 0x9F) // VRAM
            {
                return _graphics.ReadVRAM(address);
            }
            else if (block <= 0xBF) // cartridge RAM
            {
                if (_cartridge != null)
                {
                    return _cartridge.ReadRAM(address);
                }
                else
                {
                    return 0x00;
                }
            }
            else if (block <= 0xDF) // WRAM
            {
                if (block <= 0xCF)
                {
                    return _wram[address - 0xC000];
                }
                else
                {
                    int bank = SVBK;
                    if (bank == 0)
                    {
                        bank = 1;
                    }
                    return _wram[address - 0xD000 + bank * 0x1000];
                }
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
                return ReadRegister(offset);
            }

            return 0xFF;
        }

        private byte ReadRegister(byte offset)
        {
            if (offset == 0x00)
            {
                return _joypad.P1;
            }
            else if (offset == 0x01)
            {
                return _serial.SB;
            }
            else if (offset == 0x02)
            {
                return _serial.SC;
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
                return _graphics.Registers.LCDC;
            }
            else if (offset == 0x41)
            {
                return _graphics.Registers.STAT;
            }
            else if (offset == 0x42)
            {
                return _graphics.Registers.SCY;
            }
            else if (offset == 0x43)
            {
                return _graphics.Registers.SCX;
            }
            else if (offset == 0x44)
            {
                return _graphics.Registers.LY;
            }
            else if (offset == 0x45)
            {
                return _graphics.Registers.LYC;
            }
            else if (offset == 0x46)
            {
                return _graphics.Registers.ODMA;
            }
            else if (offset == 0x47)
            {
                return _graphics.Registers.BGP;
            }
            else if (offset == 0x48)
            {
                return _graphics.Registers.OBP0;
            }
            else if (offset == 0x49)
            {
                return _graphics.Registers.OBP1;
            }
            else if (offset == 0x4A)
            {
                return _graphics.Registers.WY;
            }
            else if (offset == 0x4B)
            {
                return _graphics.Registers.WX;
            }
            else if (offset == 0x4D)
            {
                return _processor.KEY1;
            }
            else if (offset == 0x4F)
            {
                return _graphics.Registers.VBK;
            }
            else if (offset == 0x51)
            {
                return _graphics.Registers.DMA1;
            }
            else if (offset == 0x52)
            {
                return _graphics.Registers.DMA2;
            }
            else if (offset == 0x53)
            {
                return _graphics.Registers.DMA3;
            }
            else if (offset == 0x54)
            {
                return _graphics.Registers.DMA4;
            }
            else if (offset == 0x55)
            {
                return _graphics.Registers.DMA5;
            }
            else if (offset == 0x70)
            {
                return SVBK;
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
                
            }

            return 0xFF;
        }

        private void WriteRegister(byte offset, byte value)
        {
            if (offset == 0x00)
            {
                _joypad.P1 = value;
            }
            else if (offset == 0x01)
            {
                _serial.SB = value;
            }
            else if (offset == 0x02)
            {
                _serial.SC = value;
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
                _graphics.Registers.LCDC = value;
            }
            else if (offset == 0x41)
            {
                _graphics.Registers.STAT = value;
            }
            else if (offset == 0x42)
            {
                _graphics.Registers.SCY = value;
            }
            else if (offset == 0x43)
            {
                _graphics.Registers.SCX = value;
            }
            else if (offset == 0x44)
            {

            }
            else if (offset == 0x45)
            {
                _graphics.Registers.LYC = value;
            }
            else if (offset == 0x46)
            {
                _graphics.Registers.ODMA = value;
            }
            else if (offset == 0x47)
            {
                _graphics.Registers.BGP = value;
            }
            else if (offset == 0x48)
            {
                _graphics.Registers.OBP0 = value;
            }
            else if (offset == 0x49)
            {
                _graphics.Registers.OBP1 = value;
            }
            else if (offset == 0x4A)
            {
                _graphics.Registers.WY = value;
            }
            else if (offset == 0x4B)
            {
                _graphics.Registers.WX = value;
            }
            else if (offset == 0x4D)
            {
                _processor.KEY1 = value;
            }
            else if (offset == 0x4F)
            {
                _graphics.Registers.VBK = value;
            }
            else if (offset == 0x50 && _bootROM != null)
            {
                if (_bootROM.Enabled && _cartridge.SupportsColor)
                {
                    _processor.Registers.A = 0x11;
                }

                _bootROM.Enabled = false;
            }
            else if (offset == 0x51)
            {
                _graphics.Registers.DMA1 = value;
            }
            else if (offset == 0x52)
            {
                _graphics.Registers.DMA2 = value;
            }
            else if (offset == 0x53)
            {
                _graphics.Registers.DMA3 = value;
            }
            else if (offset == 0x54)
            {
                _graphics.Registers.DMA4 = value;
            }
            else if (offset == 0x55)
            {
                _graphics.Registers.DMA5 = value;
            }
            else if (offset == 0x68)
            {
                _graphics.Registers.BGPI = value;
            }
            else if (offset == 0x69)
            {
                _graphics.Registers.BGPD = value;
            }
            else if (offset == 0x6A)
            {
                _graphics.Registers.OBPI = value;
            }
            else if (offset == 0x6B)
            {
                _graphics.Registers.OBPD = value;
            }
            else if (offset == 0x70)
            {
                SVBK = (byte)(value & 0b111);
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
                
            }
        }

        public void Write(ushort address, byte value, bool ignoreRenderMode = false)
        {
            byte block = address.GetHighByte();
            byte offset = address.GetLowByte();

            if (block <= 0x00 && _bootROM != null && _bootROM.Enabled) // boot ROM
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
                if (block <= 0xCF)
                {
                    _wram[address - 0xC000] = value;
                }
                else
                {
                    int bank = SVBK;
                    if (bank == 0)
                    {
                        bank = 1;
                    }
                    _wram[address - 0xD000 + bank * 0x1000] = value;
                }
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
                WriteRegister(offset, value);
            }
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
            
            writer.Write(SVBK);
            writer.Write(_hram);
            writer.Write(_wram);
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

            SVBK = reader.ReadByte();
            Array.Copy(reader.ReadBytes(_hram.Length), _hram, _hram.Length);
            Array.Copy(reader.ReadBytes(_wram.Length), _wram, _wram.Length);
        }
    }
}
