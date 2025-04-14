using System.IO;
using Atem.Core.State;

namespace Atem.Core.Graphics.Palettes
{
    public class Palette : IStateful
    {
        private GBColor[] _colors = new GBColor[4];

        public Palette()
        {
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = new GBColor(0);
            }
        }

        public Palette(GBColor[] colors)
        {
            for (int i = 0; i < colors.Length && i < _colors.Length; i++)
            {
                _colors[i] = colors[i];
            }
        }

        public GBColor this[int i]
        {
            get
            {
                return _colors[i];
            }
            set
            {
                _colors[i] = value;
            }
        }

        public static Palette FromDMGPalette(byte dmgPalette)
        {
            int color0 = (int)((3 - (dmgPalette & 0b11)) / 3.0f * 31);
            dmgPalette >>= 2;
            int color1 = (int)((3 - (dmgPalette & 0b11)) / 3.0f * 31);
            dmgPalette >>= 2;
            int color2 = (int)((3 - (dmgPalette & 0b11)) / 3.0f * 31);
            dmgPalette >>= 2;
            int color3 = (int)((3 - (dmgPalette & 0b11)) / 3.0f * 31);

            return new Palette(new GBColor[] {
                GBColor.FromValue(color0),
                GBColor.FromValue(color1),
                GBColor.FromValue(color2),
                GBColor.FromValue(color3)
            });
        }

        public byte ToDMGPalette()
        {
            byte color = (byte)(3 - _colors[3].Red / 31.0f * 3);
            color <<= 2;
            color |= (byte)(3 - _colors[2].Red / 31.0f * 3);
            color <<= 2;
            color |= (byte)(3 - _colors[1].Red / 31.0f * 3);
            color <<= 2;
            color |= (byte)(3 - _colors[0].Red / 31.0f * 3);
            return color;
        }

        public void GetState(BinaryWriter writer)
        {
            foreach (GBColor color in _colors)
            {
                color.GetState(writer);
            }
        }

        public void SetState(BinaryReader reader)
        {
            foreach (GBColor color in _colors)
            {
                color.SetState(reader);
            }
        }
    }
}
