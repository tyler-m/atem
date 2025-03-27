using System;

namespace Atem.Core.Processing
{
    public struct CPUFlags
    {
        public byte F;

        public bool Z
        {
            get { return F.GetBit(7); }
            set { F = F.SetBit(7, value); }
        }

        public bool N
        {
            get { return F.GetBit(6); }
            set { F = F.SetBit(6, value); }
        }

        public bool H
        {
            get { return F.GetBit(5); }
            set { F = F.SetBit(5, value); }
        }

        public bool C
        {
            get { return F.GetBit(4); }
            set { F = F.SetBit(4, value); }
        }
    }

    public struct CPURegisters
    {
        public ushort SP, PC;
        public byte A, B, C, D, E, H, L;
        public byte W, Z;
        public CPUFlags Flags;

        public ushort AF
        {
            get
            {
                return (ushort)(Flags.F | (A << 8));
            }
            set
            {
                A = value.GetHighByte();
                Flags.F = (byte)(value.GetLowByte() & 0xF0);
            }
        }

        public ushort BC
        {
            get
            {
                return (ushort)(C | (B << 8));
            }
            set
            {
                B = value.GetHighByte();
                C = value.GetLowByte();
            }
        }

        public ushort DE
        {
            get
            {
                return (ushort)(E | (D << 8));
            }
            set
            {
                D = value.GetHighByte();
                E = value.GetLowByte();
            }
        }

        public ushort HL
        {
            get
            {
                return (ushort)(L | (H << 8));
            }
            set
            {
                H = value.GetHighByte();
                L = value.GetLowByte();
            }
        }

        public ushort this[string register]
        {
            get
            {
                ushort value;

                switch (register)
                {
                    case "A":
                        value = A; break;
                    case "B":
                        value = B; break;
                    case "C":
                        value = C; break;
                    case "D":
                        value = D; break;
                    case "E":
                        value = E; break;
                    case "F":
                        value = Flags.F; break;
                    case "H":
                        value = H; break;
                    case "L":
                        value = L; break;
                    case "W":
                        value = W; break;
                    case "Z":
                        value = Z; break;
                    case "AF":
                        value = AF; break;
                    case "BC":
                        value = BC; break;
                    case "DE":
                        value = DE; break;
                    case "HL":
                        value = HL; break;
                    case "PC":
                        value = PC; break;
                    case "SP":
                        value = SP; break;
                    default:
                        throw new Exception("Invalid register.");
                }

                return value;
            }
            set
            {
                switch (register)
                {
                    case "A":
                        A = (byte)value; break;
                    case "B":
                        B = (byte)value; break;
                    case "C":
                        C = (byte)value; break;
                    case "D":
                        D = (byte)value; break;
                    case "E":
                        E = (byte)value; break;
                    case "H":
                        H = (byte)value; break;
                    case "L":
                        L = (byte)value; break;
                    case "F":
                        Flags.F = (byte)value; break;
                    case "W":
                        W = (byte)value; break;
                    case "Z":
                        Z = (byte)value; break;
                    case "AF":
                        AF = value; break;
                    case "BC":
                        BC = value; break;
                    case "DE":
                        DE = value; break;
                    case "HL":
                        HL = value; break;
                    case "PC":
                        PC = value; break;
                    case "SP":
                        SP = value; break;
                    default:
                        throw new Exception("Invalid register.");
                }
            }
        }
    }
}
