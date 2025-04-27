using System.IO;
using Atem.Core.State;

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

    public class ProcessorRegisters : IStateful
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

        public void GetState(BinaryWriter writer)
        {
            writer.Write(A);
            writer.Write(B);
            writer.Write(C);
            writer.Write(D);
            writer.Write(E);
            writer.Write(H);
            writer.Write(L);
            writer.Write(SP);
            writer.Write(PC);
            writer.Write(Flags.F);
        }

        public void SetState(BinaryReader reader)
        {
            A = reader.ReadByte();
            B = reader.ReadByte();
            C = reader.ReadByte();
            D = reader.ReadByte();
            E = reader.ReadByte();
            H = reader.ReadByte();
            L = reader.ReadByte();
            SP = reader.ReadUInt16();
            PC = reader.ReadUInt16();
            Flags.F = reader.ReadByte();
        }
    }
}
