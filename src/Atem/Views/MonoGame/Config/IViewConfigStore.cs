
namespace Atem.Views.MonoGame.Config
{
    public interface IViewConfigStore
    {
        public void Save(ViewConfig config);
        public ViewConfig Load();
    }
}
