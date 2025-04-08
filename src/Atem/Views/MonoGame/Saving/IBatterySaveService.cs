
namespace Atem.Views.MonoGame.Saving
{
    public interface IBatterySaveService
    {
        public void Load(ICartridgeContext context);
        public void Save(ICartridgeContext context);
    }
}
