
namespace Atem.Views.MonoGame.Config
{
    public class ViewConfigService : IViewConfigService
    {
        private ViewConfig _config;
        private readonly IViewConfigStore _configStore;

        public ViewConfigService(IViewConfigStore configStore)
        {
            _configStore = configStore;
        }

        public void Load(View view)
        {
            _config = _configStore.Load();
            SetViewValuesFromConfig(view);
        }

        public void Save(View view)
        {
            SetConfigValuesFromView(view);
            _configStore.Save(_config);
        }

        private void SetConfigValuesFromView(View view)
        {
            _config.ScreenWidth = view.Screen.Width;
            _config.ScreenHeight = view.Screen.Height;
            _config.ScreenSizeFactor = view.Screen.SizeFactor;
            _config.UserVolumeFactor = view.Atem.Bus.Audio.UserVolumeFactor;
            _config.Keybinds = view.InputManager.Keybinds;
        }

        private void SetViewValuesFromConfig(View view)
        {
            view.Screen.Width = _config.ScreenWidth;
            view.Screen.Height = _config.ScreenHeight;
            view.Screen.SizeFactor = _config.ScreenSizeFactor;
            view.Atem.Bus.Audio.UserVolumeFactor = _config.UserVolumeFactor;
            view.InputManager.Keybinds = _config.Keybinds;
        }
    }
}
