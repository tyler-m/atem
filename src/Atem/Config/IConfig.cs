using System;

namespace Atem.Config
{
    public interface IConfig<T> : IEquatable<T> where T : IConfig<T> { }
}
