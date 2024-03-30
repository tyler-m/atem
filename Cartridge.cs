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

            _rom = File.ReadAllBytes(filepath);

            byte checksum = 0;
            for (ushort address = 0x0134; address <= 0x014C; address++)
            {
                checksum -= (byte)(_rom[address] + 1);
            }

            if (checksum != _rom[0x014d])
            {
                return false;
            }

            Title = string.Empty;
            for (ushort address = 0x0134; address <= 0x0143; address++)
            {
                byte c = _rom[address];
                if (c != 0)
                {
                    Title += Encoding.UTF8.GetString(new byte[] { c });
                }
            }

            _type = _rom[0x0147];
            _romSize = _rom[0x0148];
            _ramSize = _rom[0x0149];

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
    }
}
