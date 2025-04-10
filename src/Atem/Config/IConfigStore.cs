
namespace Atem.Config
{
    public interface IConfigStore<T> where T : IConfig<T>
    {
        public void Save(T config);
        public T Load();
    }
}
