
namespace Atem.Core.Processing
{
    public interface IProcessor
    {
        bool CB { get; set; }
        bool DoubleSpeed { get; set; }
        bool IME { get; set; }
        bool SpeedSwitchFlag { get; set; }
        void Halt();
        CPURegisters Registers { get; set; }
        byte ReadBus(ushort address);
        void WriteBus(ushort address, byte value);
        byte ReadByte();
        ushort PopWord();
        void PushWord(ushort value);
        ushort ReadWord();
    }
}
