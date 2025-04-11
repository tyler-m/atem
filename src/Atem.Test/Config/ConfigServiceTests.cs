using Atem.Config;
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
                ScreenWidth = int.MaxValue,
                ScreenHeight = int.MinValue,
                ScreenSizeFactor = float.MaxValue,
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
            InputManager inputManager = new(new KeyProvider());

            AtemConfigService configService = new(configStore, screen, audioManager, inputManager);

            configService.LoadConfig(); // load the config from config store

            Assert.NotEqual(config.ScreenWidth, screen.Width);
            Assert.NotEqual(config.ScreenHeight, screen.Height);
            Assert.NotEqual(config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(config.Keybinds, inputManager.Keybinds);

            configService.LoadValues(); // load values into screen, audioManager, etc. from the loaded config

            Assert.Equal(config.ScreenWidth, screen.Width);
            Assert.Equal(config.ScreenHeight, screen.Height);
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
            InputManager inputManager = new(new KeyProvider());
            screen.Width = int.MaxValue;
            screen.Height = int.MinValue;
            screen.SizeFactor = int.MaxValue;
            audioManager.VolumeFactor = int.MinValue;
            inputManager.Keybinds[CommandType.Start].Add(new Keybind() { Key = 14 });

            AtemConfigService configService = new(configStore, screen, audioManager, inputManager);

            Assert.NotEqual(configService.Config.ScreenWidth, screen.Width);
            Assert.NotEqual(configService.Config.ScreenHeight, screen.Height);
            Assert.NotEqual(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.NotEqual(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.NotEqual(configService.Config.Keybinds, inputManager.Keybinds);

            configService.SaveValues(); // save values from screen, audioManager, etc. into the config

            Assert.Equal(configService.Config.ScreenWidth, screen.Width);
            Assert.Equal(configService.Config.ScreenHeight, screen.Height);
            Assert.Equal(configService.Config.ScreenSizeFactor, screen.SizeFactor);
            Assert.Equal(configService.Config.UserVolumeFactor, audioManager.VolumeFactor);
            Assert.Equal(configService.Config.Keybinds, inputManager.Keybinds);
        }
    }
}
