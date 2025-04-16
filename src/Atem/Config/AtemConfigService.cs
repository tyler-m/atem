using System.Linq;
using Atem.Core.Audio;
using Atem.Graphics;
using Atem.Input;
using Atem.IO;

namespace Atem.Config
{
    public class AtemConfigService : IConfigService
    {
        private AtemConfig _config;
        private readonly IConfigStore<AtemConfig> _configStore;
        private readonly IWindow _window;
        private readonly IScreen _screen;
        private readonly IAudioManager _audioManager;
        private readonly InputManager _inputManager;
        private readonly IRecentFilesService _recentFilesService;

        public AtemConfig Config => _config;

        public AtemConfigService(IConfigStore<AtemConfig> configStore, IWindow window, IScreen screen, IAudioManager audioManager, InputManager inputManager, IRecentFilesService recentFilesService)
        {
            _config = new AtemConfig();
            _configStore = configStore;
            _window = window;
            _screen = screen;
            _audioManager = audioManager;
            _inputManager = inputManager;
            _recentFilesService = recentFilesService;
        }

        public void LoadValues()
        {
            SetValuesFromConfig();
        }

        public void LoadConfig()
        {
            _config = _configStore.Load();
        }

        public void SaveValues()
        {
            SetConfigValues();
        }

        public void SaveConfig()
        {
            _configStore.Save(_config);
        }

        private void SetConfigValues()
        {
            _config.WindowWidth = _window.Width;
            _config.WindowHeight = _window.Height;
            _config.ScreenSizeLocked = _screen.SizeLocked;
            _config.ScreenSizeFactor = _screen.SizeFactor;
            _config.UserVolumeFactor = _audioManager.VolumeFactor;
            _config.Keybinds = _inputManager.Keybinds;
            _config.RecentFiles = _recentFilesService.RecentFiles.ToList();
        }

        private void SetValuesFromConfig()
        {
            _window.SetSize(_config.WindowWidth, _config.WindowHeight);
            _screen.SizeLocked = _config.ScreenSizeLocked;
            _screen.SizeFactor = _config.ScreenSizeFactor;
            _audioManager.VolumeFactor = _config.UserVolumeFactor;
            _inputManager.Keybinds = _config.Keybinds;
            _recentFilesService.RecentFiles = _config.RecentFiles;
        }
    }
}
