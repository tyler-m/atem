
namespace Atem
{
    internal class PPU
    {
        private byte[] _vram = new byte[0x2000];

        public void WriteVRAM(ushort address, byte value)
        {
            _vram[address & 0x1FFF] = value;
        }

        public byte ReadVRAM(ushort address)
        {
            return _vram[address & 0x1FFF];
        }
    }
}
