
namespace Atem
{
    internal class Bus
    {
        private PPU _ppu;
        private BootROM _bootROM;
        private Cartridge _cartridge;
        private byte[] _hram = new byte[0x7F];

        public Bus(PPU ppu)
        {
            _ppu = ppu;
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

            }
            else if (block <= 0xFD) // echo RAM
            {

            }
            else if (block <= 0xFE)
            {
                if (offset <= 0x9F) // OAM
                {

                }
            }
            else if (block <= 0xFF)
            {
                if (offset <= 0x7F) // I/O registers
                {
                    return ReadIO(offset);
                }
                else if (offset <= 0xFE) // HRAM
                {
                    return _hram[offset & 0x7F];
                }
                else if (offset <= 0xFF) // IE register
                {

                }
            }

            return 0xFF;
        }

        private byte ReadIO(byte offset)
        {
            if (offset == 0x00)
            {

            }
            else if (offset == 0x42)
            {
                return _ppu.SCY;
            }
            else if (offset == 0x44)
            {
                return _ppu.LY;
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

            }
            else if (offset <= 0x26)
            {

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
            else if (offset == 0x50)
            {
                _bootROM.Enabled = false;
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

            }
            else if (block <= 0xFD) // echo RAM
            {

            }
            else if (block <= 0xFE)
            {
                if (offset <= 0x9F) // OAM
                {

                }
            }
            else if (block <= 0xFF)
            {
                if (offset <= 0x7F) // I/O registers
                {
                    WriteIO(offset, value);
                } 
                else if (offset <= 0xFE) // HRAM
                {
                    _hram[offset & 0x7F] = value;
                }
                else if (offset <= 0xFF) // IE register
                {

                }
            }
        }
    }
}
