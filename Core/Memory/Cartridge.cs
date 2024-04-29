using System.IO;
using System.Text;

namespace Atem
{
    internal class Cartridge
    {
        private byte[] _rom;
        private byte[] _ram;

        private byte _type;
        private byte _romSize;
        private byte _ramSize;

        public bool Loaded;
        public string Title;

        public Cartridge(string filepath)
        {
            Load(filepath);
        }

        private bool Load(string filepath)
        {
            Loaded = false;

            if (!File.Exists(filepath))
            {
                return false;
            }

            byte[] data = File.ReadAllBytes(filepath);

            byte checksum = 0;
            for (ushort address = 0x0134; address <= 0x014C; address++)
            {
                checksum -= (byte)(data[address] + 1);
            }

            if (checksum != data[0x014d])
            {
                return false;
            }

            Title = string.Empty;
            for (ushort address = 0x0134; address <= 0x0143; address++)
            {
                byte c = data[address];
                if (c != 0)
                {
                    Title += Encoding.UTF8.GetString(new byte[] { c });
                }
            }

            _type = data[0x0147];
            _romSize = data[0x0148];
            _ramSize = data[0x0149];

            if (_romSize == 0x00)
            {
                _rom = new byte[0x8000];
                
                if (data.Length > _rom.Length)
                {
                    return false;
                }

                data.CopyTo(_rom, 0);
            }
            else
            {
                return false;
            }

            if (_ramSize == 0x00)
            {
                _ram = new byte[0x00];
            }
            else
            {
                return false;
            }

            Loaded = true;
            return Loaded;
        }

        public byte ReadROM(ushort address)
        {
            address &= 0x7FFF;

            if (address >= _rom.Length)
            {
                return 0xFF;
            }

            return _rom[address];
        }

        public byte ReadRAM(ushort address)
        {
            address &= 0x1FFF;

            if (address >= _ram.Length)
            {
                return 0xFF;
            }

            return _ram[address];
        }

        public void WriteRAM(ushort address, byte value)
        {
            address &= 0x1FFF;

            if (address < _ram.Length)
            {
                _ram[address] = value;
            }
        }
    }
}
