
namespace Atem.Core.Graphics
{
    internal class PaletteGroup
    {
        private Palette[] _palettes = new Palette[8];

        public bool Increment = false;
        public int Address = 0;

        public int Count => _palettes.Length;

        public PaletteGroup()
        {
            for (int i = 0; i < 8; i++)
            {
                _palettes[i] = new Palette();
            }
        }

        public Palette this[int i]
        {
            get
            {
                return _palettes[i];
            }
            set
            {
                _palettes[i] = value;
            }
        }

        public void SetAddress(bool increment, int address)
        {
            Increment = increment;
            Address = address;
        }

        public byte ReadAtAddress()
        {
            int paletteIndex = Address / 8;
            int colorIndex = (Address % 8) / 2;
            bool highByte = (Address % 8) % 2 != 0;

            if (highByte)
            {
                return _palettes[paletteIndex][colorIndex].Color.GetHighByte();
            }
            else
            {
                return _palettes[paletteIndex][colorIndex].Color.GetLowByte();
            }
        }

        public void WriteAtAddress(byte value)
        {
            int paletteIndex = Address / 8;
            int colorIndex = (Address % 8) / 2;
            bool highByte = (Address % 8) % 2 != 0;

            if (highByte)
            {
                _palettes[paletteIndex][colorIndex].SetHighByte(value);
            }
            else
            {
                _palettes[paletteIndex][colorIndex].SetLowByte(value);
            }

            if (Increment)
            {
                Address = (Address + 1) % 0x40;
            }
        }
    }
}
