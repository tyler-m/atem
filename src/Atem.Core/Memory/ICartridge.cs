using Atem.Core.State;

namespace Atem.Core.Memory
{
    public interface ICartridge : IMemoryProvider, IStateful
    {
        public bool Loaded { get; }
        public void LoadBatterySave(byte[] data);
        public byte[] GetBatterySave();
    }
}
