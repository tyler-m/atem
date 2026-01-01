namespace Atem.App.Saving
{
    public interface ICartridgeLoader
    {
        public ICartridgeContext Context { get; }
        public bool Load();
    }
}
