using Atem.Config;
using Atem.Graphics;
using Atem.Input;
using Atem.Input.Command;
using Atem.IO;
using Atem.Test.Core.Audio;
using Atem.Test.Graphics;
using Atem.Views.MonoGame.Input;

namespace Atem.Test.Config
{
    public class ConfigServiceTests
    {
        private static AtemConfig CreateConfig()
        {
            return new AtemConfig()
            {
                WindowWidth = int.MaxValue,
                WindowHeight = int.MinValue,
                ScreenSizeFactor = int.MaxValue,
                UserVolumeFactor = float.MinValue,
                Keybinds = InputManager.GetDefaultKeybinds(),
                RecentFiles = [],
            };
        }

        [Fact]
        public void LoadValues_ShouldSetValuesFromLoadedConfig()
        {
            AtemConfig config = CreateConfig();
            config.Keybinds[CommandType.Start].Add(new Keybind() { Key = 14 });
            FileInfo tempFileInfo = new(Path.GetTempFileName());
            config.RecentFiles.Add(tempFileInfo.FullName);

            FakeConfigStore<AtemConfig> configStore = new() { Config = config };
            StubScreen screen = new();
            StubAudioManager audioManager = new();
            Window window = new();
            InputManager inputManager = new(new KeyProvider());
            RecentFilesService recentFilesService = new();

            AtemConfigService configService = new(configStore, window, screen, audioManager, inputManager, recentFilesService);

            configService.LoadConfig(); // load the config from config store

            Assert.NotEqual(config.WindowWidth, window.Width);
            Assert.NotEqual(config.WindowHeight, window.Height);
            Assert.NotEqual(config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(config.Keybinds, inputManager.Keybinds);
            Assert.NotEqual(config.RecentFiles, recentFilesService.RecentFiles);

            configService.LoadValues(); // load values into screen, audioManager, etc. from the loaded config

            Assert.Equal(config.WindowWidth, window.Width);
            Assert.Equal(config.WindowHeight, window.Height);
            Assert.Equal(config.ScreenSizeFactor, screen.SizeFactor);
            Assert.Equal(config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.Equal(config.Keybinds, inputManager.Keybinds);
            Assert.Equal(config.RecentFiles, recentFilesService.RecentFiles);
        }

        [Fact]
        public void SaveValues_ShouldSetConfigFromValues()
        {
            AtemConfig config = new();
            FakeConfigStore<AtemConfig> configStore = new() { Config = config };
            StubScreen screen = new();
            StubAudioManager audioManager = new();
            Window window = new();
            InputManager inputManager = new(new KeyProvider());
            window.SetSize(int.MaxValue, int.MinValue);
            screen.SizeFactor = int.MaxValue;
            audioManager.VolumeFactor = int.MinValue;
            inputManager.Keybinds[CommandType.Start].Add(new Keybind() { Key = 14 });
            RecentFilesService recentFilesService = new();
            FileInfo tempFileInfo = new(Path.GetTempFileName());
            recentFilesService.Add(tempFileInfo.FullName);

            AtemConfigService configService = new(configStore, window, screen, audioManager, inputManager, recentFilesService);

            Assert.NotEqual(configService.Config.WindowWidth, window.Width);
            Assert.NotEqual(configService.Config.WindowHeight, window.Height);
            Assert.NotEqual(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(configService.Config.Keybinds, inputManager.Keybinds);
            Assert.NotEqual(configService.Config.RecentFiles, recentFilesService.RecentFiles);

            configService.SaveValues(); // save values from screen, audioManager, etc. into the config

            Assert.Equal(configService.Config.WindowWidth, window.Width);
            Assert.Equal(configService.Config.WindowHeight, window.Height);
            Assert.Equal(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.Equal(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.Equal(configService.Config.Keybinds, inputManager.Keybinds);
            Assert.Equal(configService.Config.RecentFiles, recentFilesService.RecentFiles);
        }

        private class FakeConfigStore<T> : IConfigStore<T> where T : IConfig<T>
        {
            public T Config;
            public T Load() => Config;
            public void Save(T config) => Config = config;
        }
    }
}
