using System;
using System.IO;
using System.Text.Json;

namespace Atem.Config
{
    public class FileConfigStore<T> : IConfigStore<T> where T : IConfig<T>
    {
        private readonly IConfigDefaultsProvider<T> _configDefaultsProvider;
        private readonly string _configFilePath;
        private readonly JsonSerializerOptions _serializerOptions;

        public FileConfigStore(IConfigDefaultsProvider<T> configDefaultsProvider, string configFilePath)
        {
            _configDefaultsProvider = configDefaultsProvider;
            _configFilePath = configFilePath;
            _serializerOptions = new JsonSerializerOptions() { WriteIndented = true };
        }

        public void Save(T config)
        {
            ArgumentNullException.ThrowIfNull(config);

            string configJson = JsonSerializer.Serialize(config, _serializerOptions);
            File.WriteAllText(_configFilePath, configJson);
        }

        public T Load()
        {
            if (!File.Exists(_configFilePath))
            {
                Save(_configDefaultsProvider.GetDefaults());
            }

            return JsonSerializer.Deserialize<T>(File.ReadAllText(_configFilePath));
        }
    }
}
