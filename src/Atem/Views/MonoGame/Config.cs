using Atem.Views.MonoGame.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Atem.Views.MonoGame
{
    public class Config
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public string RomsDirectory { get; set; }

        Dictionary<ICommand, HashSet<Keys>> _commands = [];
        public Dictionary<string, HashSet<Keys>> Commands
        {
            get
            {
                Dictionary<string, HashSet<Keys>> commands = [];
                foreach ((ICommand command, HashSet<Keys> keys) in _commands)
                {
                    commands.Add(command.Name, keys);
                }
                return commands;
            }
            set
            {
                _commands = InputManager.DefaultCommands();
                foreach ((ICommand command, HashSet<Keys> _) in _commands)
                {
                    if (value.TryGetValue(command.Name, out var keys))
                    {
                        _commands[command] = keys;
                    }
                }
            }
        }

        public Config()
        {
            _commands = InputManager.DefaultCommands();
        }

        public static void CreateDefault(string configPath)
        {
            Config config = new();
            config.ScreenWidth = 160;
            config.ScreenHeight = 144;
            config.ScreenSizeFactor = 2.0f;
            config.RomsDirectory = "roms/";

            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, options));
        }

        public static Config Load(string configPath)
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException("Config file not found.");
            }

            return JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));
        }
    }
}
