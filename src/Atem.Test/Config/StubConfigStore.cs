using Atem.Config;

namespace Atem.Test.Config
{
    internal class StubConfigStore<T> : IConfigStore<T> where T : IConfig<T>
    {
        public T Config;

        public StubConfigStore(T config)
        {
            Config = config;
        }

        public T Load()
        {
            return Config;
        }

        public void Save(T config) => Config = config;
    }
}
