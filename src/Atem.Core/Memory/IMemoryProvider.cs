
namespace Atem.Core.Memory
{
    public interface IMemoryProvider
    {
        public int MemorySize { get; }
        public byte Read(ushort address);
    }
}
