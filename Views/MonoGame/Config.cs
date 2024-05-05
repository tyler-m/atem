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

        public static void CreateDefault(string configPath)
        {
            Config config = new Config();
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
