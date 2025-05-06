using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core.Processing
{
    public interface IProcessor : IAddressable, IStateful
    {
        bool CB { get; set; }
        bool DoubleSpeed { get; set; }
        bool IME { get; set; }
        byte KEY1 { get; set; }
        bool SpeedSwitchFlag { get; set; }
        void Halt();
        ProcessorRegisters Registers { get; }
        byte ReadBus(ushort address);
        void WriteBus(ushort address, byte value);
        byte ReadByte();
        ushort PopWord();
        void PushWord(ushort value);
        ushort ReadWord();
        bool Clock();
    }
}
