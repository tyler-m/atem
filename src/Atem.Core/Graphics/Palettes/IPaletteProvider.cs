
using Atem.Core.State;

namespace Atem.Core.Graphics.Palettes
{
    public interface IPaletteProvider : IStateful
    {
        public PaletteGroup ObjectPalettes { get; }
        public PaletteGroup TilePalettes { get; }
        public PaletteGroup DMGPalettes { get; }
    }
}
