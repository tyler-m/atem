﻿using Atem.Config;

namespace Atem.Test.Config
{
    public class FileConfigStoreTests
    {
        private StubAtemConfigDefaultsProvider _defaultsProvider = new();

        [Fact]
        public void Load_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            FileConfigStore<AtemConfig> store = new(_defaultsProvider, tempFilePath);

            store.Load();

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void Save_CreatesConfigFile_WhenConfigFileDoesNotExist()
        {
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            FileConfigStore<AtemConfig> store = new(_defaultsProvider, tempFilePath);

            store.Save(_defaultsProvider.GetDefaults());

            Assert.True(File.Exists(tempFilePath));
        }

        [Fact]
        public void Save_WithNullConfig_ThrowsArgumentNullException()
        {
            FileConfigStore<AtemConfig> store = new(_defaultsProvider, Path.GetTempFileName());

            Assert.Throws<ArgumentNullException>(() => store.Save(null));
        }

        [Fact]
        public void Load_WithInvalidJson_ThrowsInvalidConfigException()
        {
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, "invalid json");

            FileConfigStore<AtemConfig> store = new(_defaultsProvider, tempFilePath);

            Assert.Throws<InvalidConfigException>(store.Load);
        }

        private class StubAtemConfigDefaultsProvider : IConfigDefaultsProvider<AtemConfig>
        {
            public AtemConfig GetDefaults() => new() { WindowWidth = int.MaxValue };
        }
    }
}