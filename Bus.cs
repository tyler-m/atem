using System;

namespace Atem
{
    internal class Bus
    {
        private BootROM _bootROM;
        private byte[] _hram = new byte[0x7F];

        public void LoadBootROM(string filepath, bool enabled = true)
        {
            _bootROM = new BootROM(filepath, enabled);
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

            }
            else if (block <= 0x9F) // VRAM
            {

            }
            else if (block <= 0xBF) // cartridge RAM
            {

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

            }
            else if (block <= 0xBF) // cartridge RAM
            {

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
