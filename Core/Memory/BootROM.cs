using System.IO;

namespace Atem
{
    internal class BootROM
    {
        private byte[] _rom = new byte[0x100];

        public bool Enabled;

        public byte Read(ushort address)
        {
            return _rom[address & 0xFF];
        }

        public BootROM(string filepath = "", bool enabled = true)
        {
            Enabled = enabled;

            if (filepath != "")
            {
                Load(filepath);
            }
        }

        private void Load(string filepath)
        {
            if (File.Exists(filepath))
            {
                _rom = File.ReadAllBytes(filepath);
            }
        }
    }
}
