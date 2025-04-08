
namespace Atem.Views.MonoGame.Saving
{
    public interface ISaveStateService
    {
        void Save(int slot, ICartridgeContext context);
        void Load(int slot, ICartridgeContext context);
    }
}
