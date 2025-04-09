﻿using Atem.Core.Audio;
using Atem.Graphics;
using Atem.Input;

namespace Atem.Config
{
    public class ConfigService : IConfigService
    {
        private Config _config;
        private readonly IConfigStore _configStore;
        private readonly IScreen _screen;
        private readonly IAudioManager _audioManager;
        private readonly InputManager _inputManager;

        public ConfigService(IConfigStore configStore, IScreen screen, IAudioManager audioManager, InputManager inputManager)
        {
            _configStore = configStore;
            _screen = screen;
            _audioManager = audioManager;
            _inputManager = inputManager;
        }

        public void Load()
        {
            _config = _configStore.Load();
            SetValuesFromConfig();
        }

        public void Save()
        {
            SetConfigValues();
            _configStore.Save(_config);
        }

        private void SetConfigValues()
        {
            _config.ScreenWidth = _screen.Width;
            _config.ScreenHeight = _screen.Height;
            _config.ScreenSizeFactor = _screen.SizeFactor;
            _config.UserVolumeFactor = _audioManager.VolumeFactor;
            _config.Keybinds = _inputManager.Keybinds;
        }

        private void SetValuesFromConfig()
        {
            _screen.Width = _config.ScreenWidth;
            _screen.Height = _config.ScreenHeight;
            _screen.SizeFactor = _config.ScreenSizeFactor;
            _audioManager.VolumeFactor = _config.UserVolumeFactor;
            _inputManager.Keybinds = _config.Keybinds;
        }
    }
}
