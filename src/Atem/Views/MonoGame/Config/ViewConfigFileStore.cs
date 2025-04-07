using System.IO;
using System.Text.Json;

namespace Atem.Views.MonoGame.Config
{
    public class ViewConfigFileStore : IViewConfigStore
    {
        private string _configFilePath;
        private readonly JsonSerializerOptions _serializerOptions;

        public ViewConfigFileStore(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public ViewConfig Load()
        {
            if (!File.Exists(_configFilePath))
            {
                throw new FileNotFoundException("Config file not found.");
            }

            return JsonSerializer.Deserialize<ViewConfig>(File.ReadAllText(_configFilePath));
        }

        public void Save(ViewConfig config)
        {
            string configJson = JsonSerializer.Serialize(config, _serializerOptions);
            File.WriteAllText(_configFilePath, configJson);
        }
    }
}
