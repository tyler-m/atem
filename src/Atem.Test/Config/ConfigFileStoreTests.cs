using Atem.Config;

namespace Atem.Test.Config
{
    public class ConfigFileStoreTests
    {
        private StubAtemConfigDefaultsProvider _defaultsProvider = new();

        [Fact]
        public void Load_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore<AtemConfig> store = new(_defaultsProvider, tempFilePath);

            store.Load();

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void Save_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore<AtemConfig> store = new(_defaultsProvider, tempFilePath);

            store.Save(_defaultsProvider.GetDefaults());

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void Save_WithNullConfig_ThrowsArgumentNullException()
        {
            ConfigFileStore<AtemConfig> store = new(_defaultsProvider, Path.GetTempFileName());

            Assert.Throws<ArgumentNullException>(() => store.Save(null));
        }
    }
}