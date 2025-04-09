
namespace Atem.Views.MonoGame.Config
{
    public interface IConfigStore
    {
        public void Save(Config config);
        public Config Load();
    }
}
