
namespace Atem.Views.MonoGame.Saving
{
    public interface ICartridgeLoader
    {
        public ICartridgeContext Context { get; }
        public bool Load();
    }
}
