using Atem.Config;
using Atem.Core;
using Atem.Core.Memory;
using Atem.Saving;
using Atem.Shutdown;

namespace Atem.Test.Shutdown
{
    public class ShutdownServiceTests
    {
        [Fact]
        public void Shutdown_WhenCartridgeIsLoaded_BatterySaveAndConfigAreSaved()
        {
            SpyConfigService configService = new();
            StubCartridgeLoader cartridgeLoader = new();
            SpyBatterySaveService saveService = new();
            FakeCartridge cartridge = new() { Loaded = true };
            FakeEmulator emulator = new() { Cartridge = cartridge };
            ShutdownService shutdownService = new(emulator, configService, cartridgeLoader, saveService);

            shutdownService.Shutdown();

            Assert.True(saveService.SaveCalled);
            Assert.True(configService.SaveValuesCalled);
            Assert.True(configService.SaveConfigCalled);
        }

        [Fact]
        public void Shutdown_WhenCartridgeIsNotLoaded_DoesNotSaveBattery_StillSavesConfig()
        {
            SpyConfigService configService = new();
            StubCartridgeLoader cartridgeLoader = new();
            SpyBatterySaveService saveService = new();
            FakeCartridge cartridge = new() { Loaded = false };
            FakeEmulator emulator = new() { Cartridge = cartridge };
            ShutdownService shutdownService = new(emulator, configService, cartridgeLoader, saveService);

            shutdownService.Shutdown();

            Assert.False(saveService.SaveCalled);
            Assert.True(configService.SaveValuesCalled);
            Assert.True(configService.SaveConfigCalled);
        }

        private class StubCartridgeLoader : ICartridgeLoader
        {
            public ICartridgeContext Context => new FakeCartridgeContext() { Id = "test" };
            public bool Load() => throw new NotImplementedException();
        }

        private class FakeEmulator : IEmulator
        {
            public ICartridge Cartridge { get; set; }
            public void GetState(BinaryWriter writer) => throw new NotImplementedException();
            public void SetState(BinaryReader reader) => throw new NotImplementedException();
        }

        private class FakeCartridge : ICartridge
        {
            public bool Loaded { get; set; }

            public bool SupportsColor => throw new NotImplementedException();

            public void LoadBatterySave(byte[] data) => throw new System.NotImplementedException();
            public byte[] GetBatterySave() => throw new System.NotImplementedException();
            public void GetState(BinaryWriter writer) => throw new NotImplementedException();
            public void SetState(BinaryReader reader) => throw new NotImplementedException();
            public byte Read(ushort address, bool ignoreAccessRestrictions = false) => throw new NotImplementedException();
            public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false) => throw new NotImplementedException();
            public IEnumerable<(ushort Start, ushort End)> GetAddressRanges() => throw new NotImplementedException();
            public void ResetMapper() => throw new NotImplementedException();
            public bool Load(byte[] data) => throw new NotImplementedException();
        }

        private class FakeCartridgeContext : ICartridgeContext
        {
            public string Id { get; set; }
        }

        private class SpyConfigService : IConfigService
        {
            public bool SaveValuesCalled { get; private set; }
            public bool SaveConfigCalled { get; private set; }
            public void SaveValues() => SaveValuesCalled = true;
            public void SaveConfig() => SaveConfigCalled = true;
            public void LoadValues() => throw new NotImplementedException();
            public void LoadConfig() => throw new NotImplementedException();
        }

        private class SpyBatterySaveService : IBatterySaveService
        {
            public bool SaveCalled { get; private set; }
            public void Save(ICartridgeContext context) => SaveCalled = true;
            public void Load(ICartridgeContext context) => throw new System.NotImplementedException();
        }
    }
}