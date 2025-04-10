using Atem.Config;

namespace Atem.Test.Config
{
    public class ConfigFileStoreTests
    {
        [Fact]
        public void Load_CreatesDefaultConfig_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            ConfigFileStore store = new(tempFilePath);

            store.Load();

            Assert.True(File.Exists(tempFilePath));
        }
    }
}