namespace Atem.App.Config
{
    public interface IConfig<T> : IEquatable<T> where T : IConfig<T> { }
}
