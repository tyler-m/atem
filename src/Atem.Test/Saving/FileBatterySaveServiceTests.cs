using Atem.Saving;
using Atem.Core;
using Atem.Core.Memory;

namespace Atem.Test.Saving
{
    public class FileBatterySaveServiceTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _batterySaveFilePath;
        private readonly string _cartridgePath;
        private readonly TestCartridge _cartridge;
        private readonly TestCartridgeContext _context;
        private readonly TestEmulator _emulator;
        private readonly FileBatterySaveService _saveService;

        public FileBatterySaveServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _batterySaveFilePath = Path.Combine(_testDirectory, "test.sav");
            _cartridgePath = Path.Combine(_testDirectory, "test");

            _cartridge = new TestCartridge();
            _context = new TestCartridgeContext() { Id = _cartridgePath };
            _emulator = new TestEmulator(_cartridge);
            _saveService = new FileBatterySaveService(_emulator);
        }

        [Fact]
        public void Save_WritesBatterySaveToFile()
        {
            byte[] expectedData = [1, 2, 3];
            _cartridge.SaveDataToReturn = expectedData;

            _saveService.Save(_context);

            Assert.True(File.Exists(_batterySaveFilePath));
            byte[] fileContents = File.ReadAllBytes(_batterySaveFilePath);
            Assert.Equal(expectedData, fileContents);
        }

        [Fact]
        public void Load_WhenBatterySaveFileExists_LoadsBatterySaveDataIntoCartridge()
        {
            byte[] existingData = [9, 8, 7];
            File.WriteAllBytes(_batterySaveFilePath, existingData);

            _saveService.Load(_context);

            Assert.Equal(existingData, _cartridge.LoadedSaveData);
        }

        [Fact]
        public void Load_WhenSaveFileDoesNotExist_DoesNotCallLoadBatterySave()
        {
            // Ensure no save file
            Assert.False(File.Exists(_batterySaveFilePath));

            _saveService.Load(_context);

            Assert.False(_cartridge.LoadBatterySaveCalled);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        private class TestCartridge : ICartridge
        {
            public bool LoadBatterySaveCalled { get; set; }
            public byte[] SaveDataToReturn { get; set; } = [];
            public byte[] LoadedSaveData { get; private set; } = [];
            public bool Loaded => throw new NotImplementedException();

            public void LoadBatterySave(byte[] data)
            {
                LoadedSaveData = data;
                LoadBatterySaveCalled = true;
            }

            public byte[] GetBatterySave() => SaveDataToReturn;

            public void GetState(BinaryWriter writer) => throw new NotImplementedException();
            public void SetState(BinaryReader reader) => throw new NotImplementedException();
        }

        private class TestEmulator : IEmulator
        {
            public ICartridge Cartridge { get; }

            public TestEmulator(ICartridge cartridge)
            {
                Cartridge = cartridge;
            }

            public void GetState(BinaryWriter writer) => throw new NotImplementedException();
            public void SetState(BinaryReader reader) => throw new NotImplementedException();
        }

        private class TestCartridgeContext : ICartridgeContext
        {
            public string Id { get; set; }
        }
    }
}