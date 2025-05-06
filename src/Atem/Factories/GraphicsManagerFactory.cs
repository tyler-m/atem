using Atem.Core;
using Atem.Core.Graphics;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Memory;
using Atem.Core.Processing;

namespace Atem.Factories
{
    public static class GraphicsManagerFactory
    {

        public static GraphicsManager Create(Bus bus, Interrupt interrupt, Cartridge cartridge)
        {
            RenderModeScheduler renderModeScheduler = new();
            PaletteProvider paletteProvider = new();
            HDMA hdma = new(bus, renderModeScheduler);
            StatInterruptDispatcher statInterruptDispatcher = new(interrupt, renderModeScheduler);
            TileManager tileManager = new(renderModeScheduler, paletteProvider, cartridge);
            ObjectManager objectManager = new(bus, renderModeScheduler, tileManager, paletteProvider, cartridge);
            ScreenRenderer screenRenderer = new(renderModeScheduler, tileManager, objectManager, cartridge);
            return new GraphicsManager(interrupt, renderModeScheduler, paletteProvider, hdma, statInterruptDispatcher, tileManager, objectManager, screenRenderer);
        }
    }
}
