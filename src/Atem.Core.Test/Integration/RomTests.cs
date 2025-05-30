﻿using System.Security.Cryptography;
using Atem.Core.Audio;
using Atem.Core.Graphics.Interrupts;
using Atem.Core.Graphics.Objects;
using Atem.Core.Graphics.Palettes;
using Atem.Core.Graphics.Screen;
using Atem.Core.Graphics.Tiles;
using Atem.Core.Graphics.Timing;
using Atem.Core.Graphics;
using Atem.Core.Input;
using Atem.Core.Memory;
using Atem.Core.Processing;

namespace Atem.Core.Test.Integration
{
    public class RomTests
    {
        private static (Emulator, ScreenRenderer) CreateTestEmulator()
        {
            Bus bus = new();

            Interrupt interrupt = new();
            Processor processor = new(bus, interrupt);
            Joypad joypad = new(interrupt);
            Core.Processing.Timer timer = new(interrupt);
            SerialManager serial = new(interrupt);
            AudioManager audio = new();
            Cartridge cartridge = new();

            RenderModeScheduler renderModeScheduler = new();
            PaletteProvider paletteProvider = new();
            HDMA hdma = new(bus, renderModeScheduler);
            StatInterruptDispatcher statInterruptDispatcher = new(interrupt, renderModeScheduler);
            TileManager tileManager = new(renderModeScheduler, paletteProvider, cartridge);
            ObjectManager objectManager = new(bus, renderModeScheduler, tileManager, paletteProvider, cartridge);
            ScreenRenderer screenRenderer = new(renderModeScheduler, tileManager, objectManager, cartridge);
            GraphicsManager graphics = new(interrupt, renderModeScheduler, paletteProvider, hdma, statInterruptDispatcher, tileManager, objectManager, screenRenderer);
            SystemMemory systemMemory = new();

            Emulator emulator = new(bus, processor, interrupt, joypad, timer, serial, audio, cartridge, graphics, systemMemory);

            return (emulator, screenRenderer);
        }

        private static byte[] GetScreenDataAsByteArray(ScreenRenderer screenRenderer)
        {
            return screenRenderer.Screen.SelectMany(p => new[] { (byte)(p.Color & 0xFF), (byte)((p.Color & 0xFF00) >> 8) }).ToArray();
        }

        private static string HashScreenData(byte[] screenData)
        {
            return BitConverter.ToString(SHA256.HashData(screenData)).Replace("-", "").ToLowerInvariant();
        }

        private static void RunRomTest(RomTestCase testCase)
        {
            (Emulator emulator, ScreenRenderer screenRenderer) = CreateTestEmulator();

            string romFilePath = RomPathResolver.Resolve(testCase.RelativeFilePath);
            emulator.LoadCartridge(File.ReadAllBytes(romFilePath));

            for (int i = 0; i < testCase.MaxUpdates; i++)
            {
                emulator.Update();
            }

            byte[] screenData = GetScreenDataAsByteArray(screenRenderer);
            string screenHash = HashScreenData(screenData);

            Assert.Equal(testCase.ExpectedScreenHash, screenHash);
        }

        [Fact]
        public void CpuInstrsTest_HasExpectedOutput()
        {
            string relativeFilePath = Path.Join("test", "roms", "cpu_instrs.gb");
            int maxUpdates = 1760;
            string expectedScreenHash = "d2b0468248a30090662dcdc145284eafaec9e6431ea363d8bb71934edc48a58e";

            RunRomTest(new RomTestCase(relativeFilePath, maxUpdates, expectedScreenHash));
        }

        [Fact]
        public void CgbSoundTest_HasExpectedOutput()
        {
            string relativeFilePath = Path.Join("test", "roms", "cgb_sound.gb");
            int maxUpdates = 2200;
            string expectedScreenHash = "a71dd6f39d4a1ef47cfb3ec2ba3636b6cd6ff5f149499861f7692f27e91125e5";

            RunRomTest(new RomTestCase(relativeFilePath, maxUpdates, expectedScreenHash));
        }
    }
}
