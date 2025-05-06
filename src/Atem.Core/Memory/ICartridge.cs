using Atem.Core.State;

namespace Atem.Core.Memory
{
    public interface ICartridge : IAddressable, IStateful
    {
        public bool Loaded { get; }
        public void LoadBatterySave(byte[] data);
        public byte[] GetBatterySave();
    }
}
