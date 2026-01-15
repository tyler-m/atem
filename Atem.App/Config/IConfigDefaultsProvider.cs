
namespace Atem.App.Config
{
    public interface IConfigDefaultsProvider<T> where T : IConfig<T>
    {
        public T GetDefaults();
    }
}
