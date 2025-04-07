using Atem.Views.MonoGame.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Atem.Views.MonoGame.Config
{
    public class ViewConfigService : IViewConfigService
    {
        private ViewConfig _config;
        private readonly IViewConfigStore _configStore;

        public ViewConfigService(ViewConfig config, IViewConfigStore configStore)
        {
            _config = config;
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
            SetCommands(view.InputManager.Commands);
            _config.UserVolumeFactor = view.Atem.Bus.Audio.UserVolumeFactor;
        }

        private void SetViewValuesFromConfig(View view)
        {
            view.Screen.Width = _config.ScreenWidth;
            view.Screen.Height = _config.ScreenHeight;
            view.Screen.SizeFactor = _config.ScreenSizeFactor;
            view.InputManager.Commands = GetCommands();
            view.Atem.Bus.Audio.UserVolumeFactor = _config.UserVolumeFactor;
        }

        private void SetCommands(Dictionary<ICommand, HashSet<Keys>> commands)
        {
            _config.Commands.Clear();

            foreach ((ICommand command, HashSet<Keys> keys) in commands)
            {
                _config.Commands.Add(command.Name, keys);
            }
        }

        private Dictionary<ICommand, HashSet<Keys>> GetCommands()
        {
            Dictionary<ICommand, HashSet<Keys>> commands = InputManager.DefaultCommands();

            foreach ((ICommand command, HashSet<Keys> _) in commands)
            {
                if (_config.Commands.TryGetValue(command.Name, out HashSet<Keys> keys))
                {
                    commands[command] = keys;
                }
            }

            return commands;
        }
    }
}
