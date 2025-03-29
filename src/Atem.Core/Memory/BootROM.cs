using System.IO;

namespace Atem.Core.Memory
{
    internal class BootROM
    {
        private byte[] _rom = new byte[0x100];
        private bool _enabled;

        public bool Enabled { get => _enabled; set => _enabled = value; }

        public BootROM(string filepath, bool enabled = true)
        {
            _enabled = enabled;
            Load(filepath);
        }

        public byte Read(ushort address)
        {
            return _rom[address & 0xFF];
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
