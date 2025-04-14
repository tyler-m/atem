using Atem.Core.State;
using System.IO;

namespace Atem.Core.Graphics.Palettes
{
    /// <summary>
    /// Provides palettes for Graphics classes.
    /// </summary>
    public class PaletteProvider : IStateful
    {
        private readonly PaletteGroup _objectPalettes;
        private readonly PaletteGroup _tilePalettes;
        private readonly PaletteGroup _dmgPalettes;

        public PaletteGroup ObjectPalettes => _objectPalettes;
        public PaletteGroup TilePalettes => _tilePalettes;
        public PaletteGroup DMGPalettes => _dmgPalettes;

        public PaletteProvider()
        {
            _objectPalettes = new PaletteGroup();
            _dmgPalettes = new PaletteGroup();
            _tilePalettes = new PaletteGroup();
            _tilePalettes[0] = new Palette([GBColor.FromValue(0x1F), GBColor.FromValue(0), GBColor.FromValue(0), GBColor.FromValue(0)]);
        }

        public void GetState(BinaryWriter writer)
        {
            _objectPalettes.GetState(writer);
            _tilePalettes.GetState(writer);
            _dmgPalettes.GetState(writer);
        }

        public void SetState(BinaryReader reader)
        {
            _objectPalettes.SetState(reader);
            _tilePalettes.SetState(reader);
            _dmgPalettes.SetState(reader);
        }
    }
}
