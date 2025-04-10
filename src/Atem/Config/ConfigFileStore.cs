using System.IO;
using System.Text.Json;

namespace Atem.Config
{
    public class ConfigFileStore : IConfigStore
    {
        private string _configFilePath;
        private readonly JsonSerializerOptions _serializerOptions;

        public ConfigFileStore(string configFilePath)
        {
            _configFilePath = configFilePath;
            _serializerOptions = new JsonSerializerOptions() { WriteIndented = true };
        }

        public AtemConfig Load()
        {
            if (!File.Exists(_configFilePath))
            {
                Save(ConfigDefaults.Create());
            }

            return JsonSerializer.Deserialize<AtemConfig>(File.ReadAllText(_configFilePath));
        }

        public void Save(AtemConfig config)
        {
            string configJson = JsonSerializer.Serialize(config, _serializerOptions);
            File.WriteAllText(_configFilePath, configJson);
        }
    }
}
