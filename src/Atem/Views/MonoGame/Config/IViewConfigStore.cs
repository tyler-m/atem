
namespace Atem.Views.MonoGame.Config
{
    public interface IViewConfigStore
    {
        public void Save(Config config);
        public Config Load();
    }
}
