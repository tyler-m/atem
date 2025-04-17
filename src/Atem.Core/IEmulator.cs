using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core
{
    public interface IEmulator : IStateful
    {
        public ICartridge Cartridge { get; }
    }
}
