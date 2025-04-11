
namespace Atem.Config
{
    public interface IConfigDefaultsProvider<T> where T : IConfig<T>
    {
        public T GetDefaults();
    }
}
