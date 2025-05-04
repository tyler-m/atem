using System;
using System.IO;
using System.Runtime.InteropServices;
using Atem.Core;
using Atem.Core.Audio;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;
using Atem.Core.Graphics.Objects;
using Atem.Views.MonoGame;

namespace Atem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LoadLibraries();
            Start();
        }

        private static void LoadLibraries()
        {
            string assemblyDirectoryPath = Path.GetDirectoryName(AppContext.BaseDirectory);

            string[] libraries = [];
            string systemId = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libraries = ["cimgui.dll"];
                systemId = "win-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libraries = ["libcimgui.so"];
                systemId = "linux-x64";
            }

            foreach (string library in libraries)
            {
                string libraryFilePath = Path.Combine(assemblyDirectoryPath, "runtimes", systemId, "native", library);

                if (File.Exists(libraryFilePath))
                {
                    NativeLibrary.Load(libraryFilePath);
                }
            }
        }

        private static void Start()
        {
            Bus bus = new();

            Processor processor = new(bus);
            Interrupt interrupt = new();
            Joypad joypad = new(interrupt);
            Timer timer = new(interrupt);
            SerialManager serial = new(interrupt);
            AudioManager audio = new();
            Cartridge cartridge = new();

            RenderModeScheduler renderModeScheduler = new();
            PaletteProvider paletteProvider = new();
            HDMA hdma = new(bus, renderModeScheduler);
            StatInterruptDispatcher statInterruptDispatcher = new(interrupt, renderModeScheduler);
            TileManager tileManager = new(renderModeScheduler, paletteProvider, cartridge);
            ObjectManager objectManager = new(bus, renderModeScheduler, tileManager, paletteProvider, cartridge);
            ScreenManager screenManager = new(renderModeScheduler, tileManager, objectManager, cartridge);
            GraphicsManager graphics = new(interrupt, renderModeScheduler, paletteProvider, hdma, statInterruptDispatcher, tileManager, objectManager, screenManager);

            bus.ProvideDependencies(processor, interrupt, joypad, timer, serial, graphics, audio, cartridge);

            Emulator emulator = new(bus, processor, interrupt, joypad, timer, serial, audio, cartridge, graphics);
            ViewStarter.Start(emulator);
        }
    }
}