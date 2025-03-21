using System;
using System.Collections.Generic;

namespace Atem.Core.Processing.Instructions
{
    internal static class Load
    {
        public static void PopulateLookup(Dictionary<byte, Func<Processor, int>> lookup)
        {
            lookup.Add(0x01, LDBCU16);
            lookup.Add(0x11, LDDEU16);
            lookup.Add(0x21, LDHLU16);
            lookup.Add(0x31, LDSPU16);

            lookup.Add(0x02, LD_BC_A);
            lookup.Add(0x12, LD_DE_A);
            lookup.Add(0x22, LD_HLI_A);
            lookup.Add(0x32, LD_HLD_A);

            lookup.Add(0x06, LDBU8);
            lookup.Add(0x16, LDDU8);
            lookup.Add(0x26, LDHU8);
            lookup.Add(0x36, LD_HL_U8);

            lookup.Add(0x08, LD_U16_SP);

            lookup.Add(0x0A, LDA_BC_);
            lookup.Add(0x1A, LDA_DE_);
            lookup.Add(0x2A, LDA_HLI_);
            lookup.Add(0x3A, LDA_HLD_);

            lookup.Add(0x0E, LDCU8);
            lookup.Add(0x1E, LDEU8);
            lookup.Add(0x2E, LDLU8);
            lookup.Add(0x3E, LDAU8);

            lookup.Add(0x40, LDBB);
            lookup.Add(0x41, LDBC);
            lookup.Add(0x42, LDBD);
            lookup.Add(0x43, LDBE);
            lookup.Add(0x44, LDBH);
            lookup.Add(0x45, LDBL);
            lookup.Add(0x46, LDB_HL_);
            lookup.Add(0x47, LDBA);

            lookup.Add(0x48, LDCB);
            lookup.Add(0x49, LDCC);
            lookup.Add(0x4A, LDCD);
            lookup.Add(0x4B, LDCE);
            lookup.Add(0x4C, LDCH);
            lookup.Add(0x4D, LDCL);
            lookup.Add(0x4E, LDC_HL_);
            lookup.Add(0x4F, LDCA);

            lookup.Add(0x50, LDDB);
            lookup.Add(0x51, LDDC);
            lookup.Add(0x52, LDDD);
            lookup.Add(0x53, LDDE);
            lookup.Add(0x54, LDDH);
            lookup.Add(0x55, LDDL);
            lookup.Add(0x56, LDD_HL_);
            lookup.Add(0x57, LDDA);

            lookup.Add(0x58, LDEB);
            lookup.Add(0x59, LDEC);
            lookup.Add(0x5A, LDED);
            lookup.Add(0x5B, LDEE);
            lookup.Add(0x5C, LDEH);
            lookup.Add(0x5D, LDEL);
            lookup.Add(0x5E, LDE_HL_);
            lookup.Add(0x5F, LDEA);

            lookup.Add(0x60, LDHB);
            lookup.Add(0x61, LDHC);
            lookup.Add(0x62, LDHD);
            lookup.Add(0x63, LDHE);
            lookup.Add(0x64, LDHH);
            lookup.Add(0x65, LDHL);
            lookup.Add(0x66, LDH_HL_);
            lookup.Add(0x67, LDHA);

            lookup.Add(0x68, LDLB);
            lookup.Add(0x69, LDLC);
            lookup.Add(0x6A, LDLD);
            lookup.Add(0x6B, LDLE);
            lookup.Add(0x6C, LDLH);
            lookup.Add(0x6D, LDLL);
            lookup.Add(0x6E, LDL_HL_);
            lookup.Add(0x6F, LDLA);

            lookup.Add(0x70, LD_HL_B);
            lookup.Add(0x71, LD_HL_C);
            lookup.Add(0x72, LD_HL_D);
            lookup.Add(0x73, LD_HL_E);
            lookup.Add(0x74, LD_HL_H);
            lookup.Add(0x75, LD_HL_L);
            lookup.Add(0x77, LD_HL_A);

            lookup.Add(0x78, LDAB);
            lookup.Add(0x79, LDAC);
            lookup.Add(0x7A, LDAD);
            lookup.Add(0x7B, LDAE);
            lookup.Add(0x7C, LDAH);
            lookup.Add(0x7D, LDAL);
            lookup.Add(0x7E, LDA_HL_);
            lookup.Add(0x7F, LDAA);

            lookup.Add(0xE0, LD_U8_A);
            lookup.Add(0xF0, LDA_U8_);

            lookup.Add(0xE2, LD_C_A);
            lookup.Add(0xF2, LDA_C_);

            lookup.Add(0xF8, LDHLS8);

            lookup.Add(0xF9, LDSPHL);

            lookup.Add(0xEA, LD_U16_A);
            lookup.Add(0xFA, LDA_U16_);

            lookup.Add(0xC1, POPBC);
            lookup.Add(0xD1, POPDE);
            lookup.Add(0xE1, POPHL);
            lookup.Add(0xF1, POPAF);

            lookup.Add(0xC5, PUSHBC);
            lookup.Add(0xD5, PUSHDE);
            lookup.Add(0xE5, PUSHHL);
            lookup.Add(0xF5, PUSHAF);
        }

        public static int LDBCU16(Processor cpu) // 0x01
        {
            cpu.Registers.BC = cpu.ReadWord();
            return 3;
        }

        public static int LDDEU16(Processor cpu) // 0x11
        {
            cpu.Registers.DE = cpu.ReadWord();
            return 3;
        }

        public static int LDHLU16(Processor cpu) // 0x21
        {
            cpu.Registers.HL = cpu.ReadWord();
            return 3;
        }

        public static int LDSPU16(Processor cpu) // 0x31
        {
            cpu.Registers.SP = cpu.ReadWord();
            return 3;
        }

        public static int LD_BC_A(Processor cpu) // 0x02
        {
            cpu.WriteBus(cpu.Registers.BC, cpu.Registers.A);
            return 2;
        }

        public static int LD_DE_A(Processor cpu) // 0x12
        {
            cpu.WriteBus(cpu.Registers.DE, cpu.Registers.A);
            return 2;
        }

        public static int LD_HLI_A(Processor cpu) // 0x22
        {
            cpu.WriteBus(cpu.Registers.HL++, cpu.Registers.A);
            return 2;
        }

        public static int LD_HLD_A(Processor cpu) // 0x32
        {
            cpu.WriteBus(cpu.Registers.HL--, cpu.Registers.A);
            return 2;
        }

        public static int LDBU8(Processor cpu) // 0x06
        {
            cpu.Registers.B = cpu.ReadByte();
            return 2;
        }

        public static int LDDU8(Processor cpu) // 0x16
        {
            cpu.Registers.D = cpu.ReadByte();
            return 2;
        }

        public static int LDHU8(Processor cpu) // 0x26
        {
            cpu.Registers.H = cpu.ReadByte();
            return 2;
        }

        public static int LD_HL_U8(Processor cpu) // 0x36
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.ReadByte());
            return 3;
        }

        public static int LD_U16_SP(Processor cpu) // 0x08
        {
            ushort address = cpu.ReadWord();
            cpu.WriteBus(address++, cpu.Registers.SP.GetLowByte());
            cpu.WriteBus(address, cpu.Registers.SP.GetHighByte());
            return 5;
        }

        public static int LDA_BC_(Processor cpu) // 0x0A
        {
            cpu.Registers.A = cpu.ReadBus(cpu.Registers.BC);
            return 2;
        }

        public static int LDA_DE_(Processor cpu) // 0x1A
        {
            cpu.Registers.A = cpu.ReadBus(cpu.Registers.DE);
            return 2;
        }

        public static int LDA_HLI_(Processor cpu) // 0x2A
        {
            cpu.Registers.A = cpu.ReadBus(cpu.Registers.HL++);
            return 2;
        }

        public static int LDA_HLD_(Processor cpu) // 0x3A
        {
            cpu.Registers.A = cpu.ReadBus(cpu.Registers.HL--);
            return 2;
        }

        public static int LDCU8(Processor cpu) // 0x0E
        {
            cpu.Registers.C = cpu.ReadByte();
            return 2;
        }

        public static int LDEU8(Processor cpu) // 0x1E
        {
            cpu.Registers.E = cpu.ReadByte();
            return 2;
        }

        public static int LDLU8(Processor cpu) // 0x2E
        {
            cpu.Registers.L = cpu.ReadByte();
            return 2;
        }

        public static int LDAU8(Processor cpu) // 0x3E
        {
            cpu.Registers.A = cpu.ReadByte();
            return 2;
        }

        public static int LDBB(Processor cpu) // 0x40
        {
            return 1;
        }

        public static int LDBC(Processor cpu) // 0x41
        {
            cpu.Registers.B = cpu.Registers.C;
            return 1;
        }

        public static int LDBD(Processor cpu) // 0x42
        {
            cpu.Registers.B = cpu.Registers.D;
            return 1;
        }

        public static int LDBE(Processor cpu) // 0x43
        {
            cpu.Registers.B = cpu.Registers.E;
            return 1;
        }

        public static int LDBH(Processor cpu) // 0x44
        {
            cpu.Registers.B = cpu.Registers.H;
            return 1;
        }

        public static int LDBL(Processor cpu) // 0x45
        {
            cpu.Registers.B = cpu.Registers.L;
            return 1;
        }

        public static int LDB_HL_(Processor cpu) // 0x46
        {
            cpu.Registers.B = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDBA(Processor cpu) // 0x47
        {
            cpu.Registers.B = cpu.Registers.A;
            return 1;
        }

        public static int LDCB(Processor cpu) // 0x48
        {
            cpu.Registers.C = cpu.Registers.B;
            return 1;
        }

        public static int LDCC(Processor cpu) // 0x49
        {
            return 1;
        }

        public static int LDCD(Processor cpu) // 0x4A
        {
            cpu.Registers.C = cpu.Registers.D;
            return 1;
        }

        public static int LDCE(Processor cpu) // 0x4B
        {
            cpu.Registers.C = cpu.Registers.E;
            return 1;
        }

        public static int LDCH(Processor cpu) // 0x4C
        {
            cpu.Registers.C = cpu.Registers.H;
            return 1;
        }

        public static int LDCL(Processor cpu) // 0x4D
        {
            cpu.Registers.C = cpu.Registers.L;
            return 1;
        }

        public static int LDC_HL_(Processor cpu) // 0x4E
        {
            cpu.Registers.C = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDCA(Processor cpu) // 0x4F
        {
            cpu.Registers.C = cpu.Registers.A;
            return 1;
        }

        public static int LDDB(Processor cpu) // 0x50
        {
            cpu.Registers.D = cpu.Registers.B;
            return 1;
        }

        public static int LDDC(Processor cpu) // 0x51
        {
            cpu.Registers.D = cpu.Registers.C;
            return 1;
        }

        public static int LDDD(Processor cpu) // 0x52
        {
            return 1;
        }

        public static int LDDE(Processor cpu) // 0x53
        {
            cpu.Registers.D = cpu.Registers.E;
            return 1;
        }

        public static int LDDH(Processor cpu) // 0x54
        {
            cpu.Registers.D = cpu.Registers.H;
            return 1;
        }

        public static int LDDL(Processor cpu) // 0x55
        {
            cpu.Registers.D = cpu.Registers.L;
            return 1;
        }

        public static int LDD_HL_(Processor cpu) // 0x56
        {
            cpu.Registers.D = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDDA(Processor cpu) // 0x57
        {
            cpu.Registers.D = cpu.Registers.A;
            return 1;
        }

        public static int LDEB(Processor cpu) // 0x58
        {
            cpu.Registers.E = cpu.Registers.B;
            return 1;
        }

        public static int LDEC(Processor cpu) // 0x59
        {
            cpu.Registers.E = cpu.Registers.C;
            return 1;
        }

        public static int LDED(Processor cpu) // 0x5A
        {
            cpu.Registers.E = cpu.Registers.D;
            return 1;
        }

        public static int LDEE(Processor cpu) // 0x5B
        {
            return 1;
        }

        public static int LDEH(Processor cpu) // 0x5C
        {
            cpu.Registers.E = cpu.Registers.H;
            return 1;
        }

        public static int LDEL(Processor cpu) // 0x5D
        {
            cpu.Registers.E = cpu.Registers.L;
            return 1;
        }

        public static int LDE_HL_(Processor cpu) // 0x5E
        {
            cpu.Registers.E = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDEA(Processor cpu) // 0x5F
        {
            cpu.Registers.E = cpu.Registers.A;
            return 1;
        }

        public static int LDHB(Processor cpu) // 0x60
        {
            cpu.Registers.H = cpu.Registers.B;
            return 1;
        }

        public static int LDHC(Processor cpu) // 0x61
        {
            cpu.Registers.H = cpu.Registers.C;
            return 1;
        }

        public static int LDHD(Processor cpu) // 0x62
        {
            cpu.Registers.H = cpu.Registers.D;
            return 1;
        }

        public static int LDHE(Processor cpu) // 0x63
        {
            cpu.Registers.H = cpu.Registers.E;
            return 1;
        }

        public static int LDHH(Processor cpu) // 0x64
        {
            return 1;
        }

        public static int LDHL(Processor cpu) // 0x65
        {
            cpu.Registers.H = cpu.Registers.L;
            return 1;
        }

        public static int LDH_HL_(Processor cpu) // 0x66
        {
            cpu.Registers.H = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDHA(Processor cpu) // 0x67
        {
            cpu.Registers.H = cpu.Registers.A;
            return 1;
        }

        public static int LDLB(Processor cpu) // 0x68
        {
            cpu.Registers.L = cpu.Registers.B;
            return 1;
        }

        public static int LDLC(Processor cpu) // 0x69
        {
            cpu.Registers.L = cpu.Registers.C;
            return 1;
        }

        public static int LDLD(Processor cpu) // 0x6A
        {
            cpu.Registers.L = cpu.Registers.D;
            return 1;
        }

        public static int LDLE(Processor cpu) // 0x6B
        {
            cpu.Registers.L = cpu.Registers.E;
            return 1;
        }

        public static int LDLH(Processor cpu) // 0x6C
        {
            cpu.Registers.L = cpu.Registers.H;
            return 1;
        }

        public static int LDLL(Processor cpu) // 0x6D
        {
            return 1;
        }

        public static int LDL_HL_(Processor cpu) // 0x6E
        {
            cpu.Registers.L = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDLA(Processor cpu) // 0x6F
        {
            cpu.Registers.L = cpu.Registers.A;
            return 1;
        }

        public static int LD_HL_B(Processor cpu) // 0x70
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.B);
            return 2;
        }

        public static int LD_HL_C(Processor cpu) // 0x71
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.C);
            return 2;
        }

        public static int LD_HL_D(Processor cpu) // 0x72
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.D);
            return 2;
        }

        public static int LD_HL_E(Processor cpu) // 0x73
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.E);
            return 2;
        }

        public static int LD_HL_H(Processor cpu) // 0x74
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.H);
            return 2;
        }

        public static int LD_HL_L(Processor cpu) // 0x75
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.L);
            return 2;
        }

        public static int LD_HL_A(Processor cpu) // 0x77
        {
            cpu.WriteBus(cpu.Registers.HL, cpu.Registers.A);
            return 2;
        }

        public static int LDAB(Processor cpu) // 0x78
        {
            cpu.Registers.A = cpu.Registers.B;
            return 1;
        }

        public static int LDAC(Processor cpu) // 0x79
        {
            cpu.Registers.A = cpu.Registers.C;
            return 1;
        }

        public static int LDAD(Processor cpu) // 0x7A
        {
            cpu.Registers.A = cpu.Registers.D;
            return 1;
        }

        public static int LDAE(Processor cpu) // 0x7B
        {
            cpu.Registers.A = cpu.Registers.E;
            return 1;
        }

        public static int LDAH(Processor cpu) // 0x7C
        {
            cpu.Registers.A = cpu.Registers.H;
            return 1;
        }

        public static int LDAL(Processor cpu) // 0x7D
        {
            cpu.Registers.A = cpu.Registers.L;
            return 1;
        }

        public static int LDA_HL_(Processor cpu) // 0x7E
        {
            cpu.Registers.A = cpu.ReadBus(cpu.Registers.HL);
            return 2;
        }

        public static int LDAA(Processor cpu) // 0x7F
        {
            return 1;
        }

        public static int LD_U8_A(Processor cpu) // 0xE0
        {
            cpu.WriteBus((ushort)(0xFF00 + cpu.ReadByte()), cpu.Registers.A);
            return 3;
        }

        public static int LDA_U8_(Processor cpu) // 0xF0
        {
            cpu.Registers.A = cpu.ReadBus((ushort)(0xFF00 + cpu.ReadByte()));
            return 3;
        }

        public static int LD_C_A(Processor cpu) // 0xE2
        {
            cpu.WriteBus((ushort)(0xFF00 + cpu.Registers.C), cpu.Registers.A);
            return 2;
        }

        public static int LDA_C_(Processor cpu) // 0xF2
        {
            cpu.Registers.A = cpu.ReadBus((ushort)(0xFF00 + cpu.Registers.C));
            return 2;
        }

        public static int LDHLS8(Processor cpu) // 0xF8
        {
            ushort value = cpu.Registers.SP;
            byte low = value.GetLowByte();
            sbyte offset = (sbyte)cpu.ReadByte();

            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = low.WillHalfCarry(offset);
            cpu.Registers.Flags.C = low.WillCarry(offset);

            if (offset >= 0)
            {
                value += (byte)offset;
            }
            else
            {
                value -= (byte)Math.Abs(offset);
            }

            cpu.Registers.HL = value;
            return 3;
        }

        public static int LDSPHL(Processor cpu) // 0xF9
        {
            cpu.Registers.SP = cpu.Registers.HL;
            return 2;
        }

        public static int LD_U16_A(Processor cpu) // 0xEA
        {
            cpu.WriteBus(cpu.ReadWord(), cpu.Registers.A);
            return 4;
        }

        public static int LDA_U16_(Processor cpu) // 0xFA
        {
            cpu.Registers.A = cpu.ReadBus(cpu.ReadWord());
            return 4;
        }

        public static int POPBC(Processor cpu) // 0xC1
        {
            cpu.Registers.BC = cpu.PopWord();
            return 3;
        }

        public static int POPDE(Processor cpu) // 0xD1
        {
            cpu.Registers.DE = cpu.PopWord();
            return 3;
        }

        public static int POPHL(Processor cpu) // 0xE1
        {
            cpu.Registers.HL = cpu.PopWord();
            return 3;
        }

        public static int POPAF(Processor cpu) // 0xF1
        {
            cpu.Registers.AF = cpu.PopWord();
            return 3;
        }

        public static int PUSHBC(Processor cpu) // 0xC5
        {
            cpu.PushWord(cpu.Registers.BC);
            return 4;
        }

        public static int PUSHDE(Processor cpu) // 0xD5
        {
            cpu.PushWord(cpu.Registers.DE);
            return 4;
        }

        public static int PUSHHL(Processor cpu) // 0xE5
        {
            cpu.PushWord(cpu.Registers.HL);
            return 4;
        }

        public static int PUSHAF(Processor cpu) // 0xF5
        {
            cpu.PushWord(cpu.Registers.AF);
            return 4;
        }
    }
}
