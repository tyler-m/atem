using Atem.Config;
using Atem.Graphics;
using Atem.Input;
using Atem.Input.Command;
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
                Keybinds = InputManager.GetDefaultKeybinds()
            };
        }

        [Fact]
        public void LoadValues_ShouldSetValuesFromLoadedConfig()
        {
            AtemConfig config = CreateConfig();
            config.Keybinds[CommandType.Start].Add(new Keybind() { Key = 14 });

            StubConfigStore<AtemConfig> configStore = new(config);
            StubScreen screen = new();
            StubAudioManager audioManager = new();
            Window window = new();
            InputManager inputManager = new(new KeyProvider());

            AtemConfigService configService = new(configStore, window, screen, audioManager, inputManager);

            configService.LoadConfig(); // load the config from config store

            Assert.NotEqual(config.WindowWidth, window.Width);
            Assert.NotEqual(config.WindowHeight, window.Height);
            Assert.NotEqual(config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(config.Keybinds, inputManager.Keybinds);

            configService.LoadValues(); // load values into screen, audioManager, etc. from the loaded config

            Assert.Equal(config.WindowWidth, window.Width);
            Assert.Equal(config.WindowHeight, window.Height);
            Assert.Equal(config.ScreenSizeFactor, screen.SizeFactor);
            Assert.Equal(config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.Equal(config.Keybinds, inputManager.Keybinds);
        }

        [Fact]
        public void SaveValues_ShouldSetConfigFromValues()
        {
            AtemConfig config = new();
            StubConfigStore<AtemConfig> configStore = new(config);
            StubScreen screen = new();
            StubAudioManager audioManager = new();
            Window window = new Window();
            InputManager inputManager = new(new KeyProvider());
            window.SetSize(int.MaxValue, int.MinValue);
            screen.SizeFactor = int.MaxValue;
            audioManager.VolumeFactor = int.MinValue;
            inputManager.Keybinds[CommandType.Start].Add(new Keybind() { Key = 14 });

            AtemConfigService configService = new(configStore, window, screen, audioManager, inputManager);

            Assert.NotEqual(configService.Config.WindowWidth, window.Width);
            Assert.NotEqual(configService.Config.WindowHeight, window.Height);
            Assert.NotEqual(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(configService.Config.Keybinds, inputManager.Keybinds);

            configService.SaveValues(); // save values from screen, audioManager, etc. into the config

            Assert.Equal(configService.Config.WindowWidth, window.Width);
            Assert.Equal(configService.Config.WindowHeight, window.Height);
            Assert.Equal(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.Equal(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.Equal(configService.Config.Keybinds, inputManager.Keybinds);
        }
    }
}
