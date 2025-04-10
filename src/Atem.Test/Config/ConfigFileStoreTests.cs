using Atem.Config;

namespace Atem.Test.Config
{
    public class ConfigFileStoreTests
    {
        [Fact]
        public void Load_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore<AtemConfig> store = new(tempFilePath);

            store.Load();

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void Save_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore<AtemConfig> store = new(tempFilePath);

            store.Save(AtemConfig.GetDefaults());

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void SaveThenLoad_WithDefaultConfig_ReturnsDefaultConfig()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore<AtemConfig> store = new(tempFilePath);
            AtemConfig config = AtemConfig.GetDefaults();

            store.Save(config);
            AtemConfig loadedConfig = store.Load();

            Assert.True(loadedConfig.Equals(config));
        }

        [Fact]
        public void Save_WithNullConfig_ThrowsArgumentNullException()
        {
            ConfigFileStore<AtemConfig> store = new(Path.GetTempFileName());

            Assert.Throws<ArgumentNullException>(() => store.Save(null));
        }
    }
}