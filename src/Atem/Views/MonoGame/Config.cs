using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Atem.Views.MonoGame.Input;

namespace Atem.Views.MonoGame
{
    public class Config
    {
        private string _filePath;
        Dictionary<ICommand, HashSet<Keys>> _commands = [];

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public string RomsDirectory { get; set; }

        public Dictionary<ICommand, HashSet<Keys>> GetCommands()
        {
            return _commands;
        }

        public void SetCommands(Dictionary<ICommand, HashSet<Keys>> commands)
        {
            _commands = commands;
        }

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

        public Config() { }

        public Config(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("Config file not found.");
            }

            Config config = JsonSerializer.Deserialize<Config>(File.ReadAllText(_filePath));
            ScreenWidth = config.ScreenWidth;
            ScreenHeight = config.ScreenHeight;
            ScreenSizeFactor = config.ScreenSizeFactor;
            RomsDirectory = config.RomsDirectory;
            Commands = config.Commands;
        }

        public void Save()
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(this, options));
        }

        public static void CreateDefault(string configPath)
        {
            Config config = new();
            config.ScreenWidth = 160;
            config.ScreenHeight = 144;
            config.ScreenSizeFactor = 2.0f;
            config.RomsDirectory = "roms/";
            config.Commands = [];

            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, options));
        }
    }
}
