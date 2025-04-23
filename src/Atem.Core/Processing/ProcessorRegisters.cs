
namespace Atem.Core.Processing
{
    public class ProcessorFlags
    {
        public byte F { get; set; }
        public bool Z { get => F.GetBit(7); set => F = F.SetBit(7, value); }
        public bool N { get => F.GetBit(6); set => F = F.SetBit(6, value); }
        public bool H { get => F.GetBit(5); set => F = F.SetBit(5, value); }
        public bool C { get => F.GetBit(4); set => F = F.SetBit(4, value); }
    }

    public class ProcessorRegisters
    {
        public ushort SP, PC;
        public byte A, B, C, D, E, H, L;
        public ProcessorFlags Flags = new();

        public ushort AF
        {
            get => (ushort)(Flags.F | (A << 8));
            set
            {
                A = value.GetHighByte();
                Flags.F = (byte)(value.GetLowByte() & 0xF0);
            }
        }

        public ushort BC
        {
            get => (ushort)(C | (B << 8));
            set
            {
                B = value.GetHighByte();
                C = value.GetLowByte();
            }
        }

        public ushort DE
        {
            get => (ushort)(E | (D << 8));
            set
            {
                D = value.GetHighByte();
                E = value.GetLowByte();
            }
        }

        public ushort HL
        {
            get => (ushort)(L | (H << 8));
            set
            {
                H = value.GetHighByte();
                L = value.GetLowByte();
            }
        }
    }
}
