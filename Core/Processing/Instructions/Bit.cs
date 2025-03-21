using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Atem.Core.Processing.Instructions
{
    internal static class Bit
    {
        public static void PopulateLookup(Dictionary<byte, Func<Processor, int>> lookup)
        {
            lookup.Add(0x00, RLCB);
            lookup.Add(0x01, RLCC);
            lookup.Add(0x02, RLCD);
            lookup.Add(0x03, RLCE);
            lookup.Add(0x04, RLCH);
            lookup.Add(0x05, RLCL);
            lookup.Add(0x06, RLC_HL_);
            lookup.Add(0x07, RLCA);

            lookup.Add(0x08, RRCB);
            lookup.Add(0x09, RRCC);
            lookup.Add(0x0A, RRCD);
            lookup.Add(0x0B, RRCE);
            lookup.Add(0x0C, RRCH);
            lookup.Add(0x0D, RRCL);
            lookup.Add(0x0E, RRC_HL_);
            lookup.Add(0x0F, RRCA);

            lookup.Add(0x10, RLB);
            lookup.Add(0x11, RLC);
            lookup.Add(0x12, RLD);
            lookup.Add(0x13, RLE);
            lookup.Add(0x14, RLH);
            lookup.Add(0x15, RLL);
            lookup.Add(0x16, RL_HL_);
            lookup.Add(0x17, RLA);

            lookup.Add(0x18, RRB);
            lookup.Add(0x19, RRC);
            lookup.Add(0x1A, RRD);
            lookup.Add(0x1B, RRE);
            lookup.Add(0x1C, RRH);
            lookup.Add(0x1D, RRL);
            lookup.Add(0x1E, RR_HL_);
            lookup.Add(0x1F, RRA);

            lookup.Add(0x20, SLAB);
            lookup.Add(0x21, SLAC);
            lookup.Add(0x22, SLAD);
            lookup.Add(0x23, SLAE);
            lookup.Add(0x24, SLAH);
            lookup.Add(0x25, SLAL);
            lookup.Add(0x26, SLA_HL_);
            lookup.Add(0x27, SLAA);

            lookup.Add(0x28, SRAB);
            lookup.Add(0x29, SRAC);
            lookup.Add(0x2A, SRAD);
            lookup.Add(0x2B, SRAE);
            lookup.Add(0x2C, SRAH);
            lookup.Add(0x2D, SRAL);
            lookup.Add(0x2E, SRA_HL_);
            lookup.Add(0x2F, SRAA);

            lookup.Add(0x30, SWAPB);
            lookup.Add(0x31, SWAPC);
            lookup.Add(0x32, SWAPD);
            lookup.Add(0x33, SWAPE);
            lookup.Add(0x34, SWAPH);
            lookup.Add(0x35, SWAPL);
            lookup.Add(0x36, SWAP_HL_);
            lookup.Add(0x37, SWAPA);

            lookup.Add(0x38, SRLB);
            lookup.Add(0x39, SRLC);
            lookup.Add(0x3A, SRLD);
            lookup.Add(0x3B, SRLE);
            lookup.Add(0x3C, SRLH);
            lookup.Add(0x3D, SRLL);
            lookup.Add(0x3E, SRL_HL_);
            lookup.Add(0x3F, SRLA);

            lookup.Add(0x40, BIT0B);
            lookup.Add(0x41, BIT0C);
            lookup.Add(0x42, BIT0D);
            lookup.Add(0x43, BIT0E);
            lookup.Add(0x44, BIT0H);
            lookup.Add(0x45, BIT0L);
            lookup.Add(0x46, BIT0_HL_);
            lookup.Add(0x47, BIT0A);

            lookup.Add(0x48, BIT1B);
            lookup.Add(0x49, BIT1C);
            lookup.Add(0x4A, BIT1D);
            lookup.Add(0x4B, BIT1E);
            lookup.Add(0x4C, BIT1H);
            lookup.Add(0x4D, BIT1L);
            lookup.Add(0x4E, BIT1_HL_);
            lookup.Add(0x4F, BIT1A);

            lookup.Add(0x50, BIT2B);
            lookup.Add(0x51, BIT2C);
            lookup.Add(0x52, BIT2D);
            lookup.Add(0x53, BIT2E);
            lookup.Add(0x54, BIT2H);
            lookup.Add(0x55, BIT2L);
            lookup.Add(0x56, BIT2_HL_);
            lookup.Add(0x57, BIT2A);

            lookup.Add(0x58, BIT3B);
            lookup.Add(0x59, BIT3C);
            lookup.Add(0x5A, BIT3D);
            lookup.Add(0x5B, BIT3E);
            lookup.Add(0x5C, BIT3H);
            lookup.Add(0x5D, BIT3L);
            lookup.Add(0x5E, BIT3_HL_);
            lookup.Add(0x5F, BIT3A);

            lookup.Add(0x60, BIT4B);
            lookup.Add(0x61, BIT4C);
            lookup.Add(0x62, BIT4D);
            lookup.Add(0x63, BIT4E);
            lookup.Add(0x64, BIT4H);
            lookup.Add(0x65, BIT4L);
            lookup.Add(0x66, BIT4_HL_);
            lookup.Add(0x67, BIT4A);

            lookup.Add(0x68, BIT5B);
            lookup.Add(0x69, BIT5C);
            lookup.Add(0x6A, BIT5D);
            lookup.Add(0x6B, BIT5E);
            lookup.Add(0x6C, BIT5H);
            lookup.Add(0x6D, BIT5L);
            lookup.Add(0x6E, BIT5_HL_);
            lookup.Add(0x6F, BIT5A);

            lookup.Add(0x70, BIT6B);
            lookup.Add(0x71, BIT6C);
            lookup.Add(0x72, BIT6D);
            lookup.Add(0x73, BIT6E);
            lookup.Add(0x74, BIT6H);
            lookup.Add(0x75, BIT6L);
            lookup.Add(0x76, BIT6_HL_);
            lookup.Add(0x77, BIT6A);

            lookup.Add(0x78, BIT7B);
            lookup.Add(0x79, BIT7C);
            lookup.Add(0x7A, BIT7D);
            lookup.Add(0x7B, BIT7E);
            lookup.Add(0x7C, BIT7H);
            lookup.Add(0x7D, BIT7L);
            lookup.Add(0x7E, BIT7_HL_);
            lookup.Add(0x7F, BIT7A);

            lookup.Add(0x80, RES0B);
            lookup.Add(0x81, RES0C);
            lookup.Add(0x82, RES0D);
            lookup.Add(0x83, RES0E);
            lookup.Add(0x84, RES0H);
            lookup.Add(0x85, RES0L);
            lookup.Add(0x86, RES0_HL_);
            lookup.Add(0x87, RES0A);

            lookup.Add(0x88, RES1B);
            lookup.Add(0x89, RES1C);
            lookup.Add(0x8A, RES1D);
            lookup.Add(0x8B, RES1E);
            lookup.Add(0x8C, RES1H);
            lookup.Add(0x8D, RES1L);
            lookup.Add(0x8E, RES1_HL_);
            lookup.Add(0x8F, RES1A);

            lookup.Add(0x90, RES2B);
            lookup.Add(0x91, RES2C);
            lookup.Add(0x92, RES2D);
            lookup.Add(0x93, RES2E);
            lookup.Add(0x94, RES2H);
            lookup.Add(0x95, RES2L);
            lookup.Add(0x96, RES2_HL_);
            lookup.Add(0x97, RES2A);


            lookup.Add(0x98, RES3B);
            lookup.Add(0x99, RES3C);
            lookup.Add(0x9A, RES3D);
            lookup.Add(0x9B, RES3E);
            lookup.Add(0x9C, RES3H);
            lookup.Add(0x9D, RES3L);
            lookup.Add(0x9E, RES3_HL_);
            lookup.Add(0x9F, RES3A);

            lookup.Add(0xA0, RES4B);
            lookup.Add(0xA1, RES4C);
            lookup.Add(0xA2, RES4D);
            lookup.Add(0xA3, RES4E);
            lookup.Add(0xA4, RES4H);
            lookup.Add(0xA5, RES4L);
            lookup.Add(0xA6, RES4_HL_);
            lookup.Add(0xA7, RES4A);

            lookup.Add(0xA8, RES5B);
            lookup.Add(0xA9, RES5C);
            lookup.Add(0xAA, RES5D);
            lookup.Add(0xAB, RES5E);
            lookup.Add(0xAC, RES5H);
            lookup.Add(0xAD, RES5L);
            lookup.Add(0xAE, RES5_HL_);
            lookup.Add(0xAF, RES5A);

            lookup.Add(0xB0, RES6B);
            lookup.Add(0xB1, RES6C);
            lookup.Add(0xB2, RES6D);
            lookup.Add(0xB3, RES6E);
            lookup.Add(0xB4, RES6H);
            lookup.Add(0xB5, RES6L);
            lookup.Add(0xB6, RES6_HL_);
            lookup.Add(0xB7, RES6A);

            lookup.Add(0xB8, RES7B);
            lookup.Add(0xB9, RES7C);
            lookup.Add(0xBA, RES7D);
            lookup.Add(0xBB, RES7E);
            lookup.Add(0xBC, RES7H);
            lookup.Add(0xBD, RES7L);
            lookup.Add(0xBE, RES7_HL_);
            lookup.Add(0xBF, RES7A);

            lookup.Add(0xC0, SET0B);
            lookup.Add(0xC1, SET0C);
            lookup.Add(0xC2, SET0D);
            lookup.Add(0xC3, SET0E);
            lookup.Add(0xC4, SET0H);
            lookup.Add(0xC5, SET0L);
            lookup.Add(0xC6, SET0_HL_);
            lookup.Add(0xC7, SET0A);

            lookup.Add(0xC8, SET1B);
            lookup.Add(0xC9, SET1C);
            lookup.Add(0xCA, SET1D);
            lookup.Add(0xCB, SET1E);
            lookup.Add(0xCC, SET1H);
            lookup.Add(0xCD, SET1L);
            lookup.Add(0xCE, SET1_HL_);
            lookup.Add(0xCF, SET1A);

            lookup.Add(0xD0, SET2B);
            lookup.Add(0xD1, SET2C);
            lookup.Add(0xD2, SET2D);
            lookup.Add(0xD3, SET2E);
            lookup.Add(0xD4, SET2H);
            lookup.Add(0xD5, SET2L);
            lookup.Add(0xD6, SET2_HL_);
            lookup.Add(0xD7, SET2A);


            lookup.Add(0xD8, SET3B);
            lookup.Add(0xD9, SET3C);
            lookup.Add(0xDA, SET3D);
            lookup.Add(0xDB, SET3E);
            lookup.Add(0xDC, SET3H);
            lookup.Add(0xDD, SET3L);
            lookup.Add(0xDE, SET3_HL_);
            lookup.Add(0xDF, SET3A);

            lookup.Add(0xE0, SET4B);
            lookup.Add(0xE1, SET4C);
            lookup.Add(0xE2, SET4D);
            lookup.Add(0xE3, SET4E);
            lookup.Add(0xE4, SET4H);
            lookup.Add(0xE5, SET4L);
            lookup.Add(0xE6, SET4_HL_);
            lookup.Add(0xE7, SET4A);

            lookup.Add(0xE8, SET5B);
            lookup.Add(0xE9, SET5C);
            lookup.Add(0xEA, SET5D);
            lookup.Add(0xEB, SET5E);
            lookup.Add(0xEC, SET5H);
            lookup.Add(0xED, SET5L);
            lookup.Add(0xEE, SET5_HL_);
            lookup.Add(0xEF, SET5A);

            lookup.Add(0xF0, SET6B);
            lookup.Add(0xF1, SET6C);
            lookup.Add(0xF2, SET6D);
            lookup.Add(0xF3, SET6E);
            lookup.Add(0xF4, SET6H);
            lookup.Add(0xF5, SET6L);
            lookup.Add(0xF6, SET6_HL_);
            lookup.Add(0xF7, SET6A);

            lookup.Add(0xF8, SET7B);
            lookup.Add(0xF9, SET7C);
            lookup.Add(0xFA, SET7D);
            lookup.Add(0xFB, SET7E);
            lookup.Add(0xFC, SET7H);
            lookup.Add(0xFD, SET7L);
            lookup.Add(0xFE, SET7_HL_);
            lookup.Add(0xFF, SET7A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Rlc(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = value.GetBit(7);
            value = (byte)(value << 1);
            value = value.SetBit(0, cpu.Registers.Flags.C);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Rrc(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = value.GetBit(0);
            value = (byte)(value >> 1);
            value = value.SetBit(7, cpu.Registers.Flags.C);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Rl(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            bool oldCarry = cpu.Registers.Flags.C;
            cpu.Registers.Flags.C = value.GetBit(7);
            value = (byte)(value << 1);
            value = value.SetBit(0, oldCarry);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Rr(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            bool oldCarry = cpu.Registers.Flags.C;
            cpu.Registers.Flags.C = value.GetBit(0);
            value = (byte)(value >> 1);
            value = value.SetBit(7, oldCarry);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Sla(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = value.GetBit(7);
            value = (byte)(value << 1);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Sra(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = value.GetBit(0);
            bool sign = value.GetBit(7);
            value = (byte)(value >> 1);
            value = value.SetBit(7, sign);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Swap(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = false;
            value = value.SwapNibbles();
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Srl(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = value.GetBit(0);
            value = (byte)(value >> 1);
            cpu.Registers.Flags.Z = value == 0;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FlipBit(Processor cpu, int bit, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = true;
            cpu.Registers.Flags.Z = !value.GetBit(Convert.ToInt32(bit));
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Res(Processor cpu, int bit, ref byte value)
        {
            value = value.ClearBit(Convert.ToInt32(bit));
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Set(Processor cpu, int bit, ref byte value)
        {
            value = value.SetBit(Convert.ToInt32(bit));
            return 2;
        }

        public static int RLCB(Processor cpu) // 0x00
        {
            return Rlc(cpu, ref cpu.Registers.B);
        }

        public static int RLCC(Processor cpu) // 0x01
        {
            return Rlc(cpu, ref cpu.Registers.C);
        }

        public static int RLCD(Processor cpu) // 0x02
        {
            return Rlc(cpu, ref cpu.Registers.D);
        }

        public static int RLCE(Processor cpu) // 0x03
        {
            return Rlc(cpu, ref cpu.Registers.E);
        }

        public static int RLCH(Processor cpu) // 0x04
        {
            return Rlc(cpu, ref cpu.Registers.H);
        }

        public static int RLCL(Processor cpu) // 0x05
        {
            return Rlc(cpu, ref cpu.Registers.L);
        }

        public static int RLC_HL_(Processor cpu) // 0x06
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Rlc(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RLCA(Processor cpu) // 0x07
        {
            return Rlc(cpu, ref cpu.Registers.A);
        }

        public static int RRCB(Processor cpu) // 0x08
        {
            return Rrc(cpu, ref cpu.Registers.B);
        }

        public static int RRCC(Processor cpu) // 0x09
        {
            return Rrc(cpu, ref cpu.Registers.C);
        }

        public static int RRCD(Processor cpu) // 0x0A
        {
            return Rrc(cpu, ref cpu.Registers.D);
        }

        public static int RRCE(Processor cpu) // 0x0B
        {
            return Rrc(cpu, ref cpu.Registers.E);
        }

        public static int RRCH(Processor cpu) // 0x0C
        {
            return Rrc(cpu, ref cpu.Registers.H);
        }

        public static int RRCL(Processor cpu) // 0x0D
        {
            return Rrc(cpu, ref cpu.Registers.L);
        }

        public static int RRC_HL_(Processor cpu) // 0x0E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Rrc(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RRCA(Processor cpu) // 0x0F
        {
            return Rrc(cpu, ref cpu.Registers.A);
        }

        public static int RLB(Processor cpu) // 0x10
        {
            return Rl(cpu, ref cpu.Registers.B);
        }

        public static int RLC(Processor cpu) // 0x11
        {
            return Rl(cpu, ref cpu.Registers.C);
        }

        public static int RLD(Processor cpu) // 0x12
        {
            return Rl(cpu, ref cpu.Registers.D);
        }

        public static int RLE(Processor cpu) // 0x13
        {
            return Rl(cpu, ref cpu.Registers.E);
        }

        public static int RLH(Processor cpu) // 0x14
        {
            return Rl(cpu, ref cpu.Registers.H);
        }

        public static int RLL(Processor cpu) // 0x15
        {
            return Rl(cpu, ref cpu.Registers.L);
        }

        public static int RL_HL_(Processor cpu) // 0x16
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Rl(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RLA(Processor cpu) // 0x17
        {
            return Rl(cpu, ref cpu.Registers.A);
        }

        public static int RRB(Processor cpu) // 0x18
        {
            return Rr(cpu, ref cpu.Registers.B);
        }

        public static int RRC(Processor cpu) // 0x19
        {
            return Rr(cpu, ref cpu.Registers.C);
        }

        public static int RRD(Processor cpu) // 0x1A
        {
            return Rr(cpu, ref cpu.Registers.D);
        }

        public static int RRE(Processor cpu) // 0x1B
        {
            return Rr(cpu, ref cpu.Registers.E);
        }

        public static int RRH(Processor cpu) // 0x1C
        {
            return Rr(cpu, ref cpu.Registers.H);
        }

        public static int RRL(Processor cpu) // 0x1D
        {
            return Rr(cpu, ref cpu.Registers.L);
        }

        public static int RR_HL_(Processor cpu) // 0x1E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Rr(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RRA(Processor cpu) // 0x1F
        {
            return Rr(cpu, ref cpu.Registers.A);
        }

        public static int SLAB(Processor cpu) // 0x20
        {
            return Sla(cpu, ref cpu.Registers.B);
        }

        public static int SLAC(Processor cpu) // 0x21
        {
            return Sla(cpu, ref cpu.Registers.C);
        }

        public static int SLAD(Processor cpu) // 0x22
        {
            return Sla(cpu, ref cpu.Registers.D);
        }

        public static int SLAE(Processor cpu) // 0x23
        {
            return Sla(cpu, ref cpu.Registers.E);
        }

        public static int SLAH(Processor cpu) // 0x24
        {
            return Sla(cpu, ref cpu.Registers.H);
        }

        public static int SLAL(Processor cpu) // 0x25
        {
            return Sla(cpu, ref cpu.Registers.L);
        }

        public static int SLA_HL_(Processor cpu) // 0x26
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Sla(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SLAA(Processor cpu) // 0x27
        {
            return Sla(cpu, ref cpu.Registers.A);
        }

        public static int SRAB(Processor cpu) // 0x28
        {
            return Sra(cpu, ref cpu.Registers.B);
        }

        public static int SRAC(Processor cpu) // 0x29
        {
            return Sra(cpu, ref cpu.Registers.C);
        }

        public static int SRAD(Processor cpu) // 0x2A
        {
            return Sra(cpu, ref cpu.Registers.D);
        }

        public static int SRAE(Processor cpu) // 0x2B
        {
            return Sra(cpu, ref cpu.Registers.E);
        }

        public static int SRAH(Processor cpu) // 0x2C
        {
            return Sra(cpu, ref cpu.Registers.H);
        }

        public static int SRAL(Processor cpu) // 0x2D
        {
            return Sra(cpu, ref cpu.Registers.L);
        }

        public static int SRA_HL_(Processor cpu) // 0x2E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Sra(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SRAA(Processor cpu) // 0x2F
        {
            return Sra(cpu, ref cpu.Registers.A);
        }

        public static int SWAPB(Processor cpu) // 0x30
        {
            return Swap(cpu, ref cpu.Registers.B);
        }

        public static int SWAPC(Processor cpu) // 0x31
        {
            return Swap(cpu, ref cpu.Registers.C);
        }

        public static int SWAPD(Processor cpu) // 0x32
        {
            return Swap(cpu, ref cpu.Registers.D);
        }

        public static int SWAPE(Processor cpu) // 0x33
        {
            return Swap(cpu, ref cpu.Registers.E);
        }

        public static int SWAPH(Processor cpu) // 0x34
        {
            return Swap(cpu, ref cpu.Registers.H);
        }

        public static int SWAPL(Processor cpu) // 0x35
        {
            return Swap(cpu, ref cpu.Registers.L);
        }

        public static int SWAP_HL_(Processor cpu) // 0x36
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Swap(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SWAPA(Processor cpu) // 0x37
        {
            return Swap(cpu, ref cpu.Registers.A);
        }

        public static int SRLB(Processor cpu) // 0x38
        {
            return Srl(cpu, ref cpu.Registers.B);
        }

        public static int SRLC(Processor cpu) // 0x39
        {
            return Srl(cpu, ref cpu.Registers.C);
        }

        public static int SRLD(Processor cpu) // 0x3A
        {
            return Srl(cpu, ref cpu.Registers.D);
        }

        public static int SRLE(Processor cpu) // 0x3B
        {
            return Srl(cpu, ref cpu.Registers.E);
        }

        public static int SRLH(Processor cpu) // 0x3C
        {
            return Srl(cpu, ref cpu.Registers.H);
        }

        public static int SRLL(Processor cpu) // 0x3D
        {
            return Srl(cpu, ref cpu.Registers.L);
        }

        public static int SRL_HL_(Processor cpu) // 0x3E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Srl(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SRLA(Processor cpu) // 0x3F
        {
            return Srl(cpu, ref cpu.Registers.A);
        }

        public static int BIT0B(Processor cpu) // 0x40
        {
            return FlipBit(cpu, 0, ref cpu.Registers.B);
        }

        public static int BIT0C(Processor cpu) // 0x41
        {
            return FlipBit(cpu, 0, ref cpu.Registers.C);
        }

        public static int BIT0D(Processor cpu) // 0x42
        {
            return FlipBit(cpu, 0, ref cpu.Registers.D);
        }

        public static int BIT0E(Processor cpu) // 0x43
        {
            return FlipBit(cpu, 0, ref cpu.Registers.E);
        }

        public static int BIT0H(Processor cpu) // 0x44
        {
            return FlipBit(cpu, 0, ref cpu.Registers.H);
        }

        public static int BIT0L(Processor cpu) // 0x45
        {
            return FlipBit(cpu, 0, ref cpu.Registers.L);
        }

        public static int BIT0_HL_(Processor cpu) // 0x46
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 0, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT0A(Processor cpu) // 0x47
        {
            return FlipBit(cpu, 0, ref cpu.Registers.A);
        }

        public static int BIT1B(Processor cpu) // 0x48
        {
            return FlipBit(cpu, 1, ref cpu.Registers.B);
        }

        public static int BIT1C(Processor cpu) // 0x49
        {
            return FlipBit(cpu, 1, ref cpu.Registers.C);
        }

        public static int BIT1D(Processor cpu) // 0x4A
        {
            return FlipBit(cpu, 1, ref cpu.Registers.D);
        }

        public static int BIT1E(Processor cpu) // 0x4B
        {
            return FlipBit(cpu, 1, ref cpu.Registers.E);
        }

        public static int BIT1H(Processor cpu) // 0x4C
        {
            return FlipBit(cpu, 1, ref cpu.Registers.H);
        }

        public static int BIT1L(Processor cpu) // 0x4D
        {
            return FlipBit(cpu, 1, ref cpu.Registers.L);
        }

        public static int BIT1_HL_(Processor cpu) // 0x4E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 1, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT1A(Processor cpu) // 0x4F
        {
            return FlipBit(cpu, 1, ref cpu.Registers.A);
        }

        public static int BIT2B(Processor cpu) // 0x50
        {
            return FlipBit(cpu, 2, ref cpu.Registers.B);
        }

        public static int BIT2C(Processor cpu) // 0x51
        {
            return FlipBit(cpu, 2, ref cpu.Registers.C);
        }

        public static int BIT2D(Processor cpu) // 0x52
        {
            return FlipBit(cpu, 2, ref cpu.Registers.D);
        }

        public static int BIT2E(Processor cpu) // 0x53
        {
            return FlipBit(cpu, 2, ref cpu.Registers.E);
        }

        public static int BIT2H(Processor cpu) // 0x54
        {
            return FlipBit(cpu, 2, ref cpu.Registers.H);
        }

        public static int BIT2L(Processor cpu) // 0x55
        {
            return FlipBit(cpu, 2, ref cpu.Registers.L);
        }

        public static int BIT2_HL_(Processor cpu) // 0x56
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 2, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT2A(Processor cpu) // 0x57
        {
            return FlipBit(cpu, 2, ref cpu.Registers.A);
        }

        public static int BIT3B(Processor cpu) // 0x58
        {
            return FlipBit(cpu, 3, ref cpu.Registers.B);
        }

        public static int BIT3C(Processor cpu) // 0x59
        {
            return FlipBit(cpu, 3, ref cpu.Registers.C);
        }

        public static int BIT3D(Processor cpu) // 0x5A
        {
            return FlipBit(cpu, 3, ref cpu.Registers.D);
        }

        public static int BIT3E(Processor cpu) // 0x5B
        {
            return FlipBit(cpu, 3, ref cpu.Registers.E);
        }

        public static int BIT3H(Processor cpu) // 0x5C
        {
            return FlipBit(cpu, 3, ref cpu.Registers.H);
        }

        public static int BIT3L(Processor cpu) // 0x5D
        {
            return FlipBit(cpu, 3, ref cpu.Registers.L);
        }

        public static int BIT3_HL_(Processor cpu) // 0x5E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 3, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT3A(Processor cpu) // 0x5F
        {
            return FlipBit(cpu, 3, ref cpu.Registers.A);
        }

        public static int BIT4B(Processor cpu) // 0x60
        {
            return FlipBit(cpu, 4, ref cpu.Registers.B);
        }

        public static int BIT4C(Processor cpu) // 0x61
        {
            return FlipBit(cpu, 4, ref cpu.Registers.C);
        }

        public static int BIT4D(Processor cpu) // 0x62
        {
            return FlipBit(cpu, 4, ref cpu.Registers.D);
        }

        public static int BIT4E(Processor cpu) // 0x63
        {
            return FlipBit(cpu, 4, ref cpu.Registers.E);
        }

        public static int BIT4H(Processor cpu) // 0x64
        {
            return FlipBit(cpu, 4, ref cpu.Registers.H);
        }

        public static int BIT4L(Processor cpu) // 0x65
        {
            return FlipBit(cpu, 4, ref cpu.Registers.L);
        }

        public static int BIT4_HL_(Processor cpu) // 0x66
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 4, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT4A(Processor cpu) // 0x67
        {
            return FlipBit(cpu, 4, ref cpu.Registers.A);
        }

        public static int BIT5B(Processor cpu) // 0x68
        {
            return FlipBit(cpu, 5, ref cpu.Registers.B);
        }

        public static int BIT5C(Processor cpu) // 0x69
        {
            return FlipBit(cpu, 5, ref cpu.Registers.C);
        }

        public static int BIT5D(Processor cpu) // 0x6A
        {
            return FlipBit(cpu, 5, ref cpu.Registers.D);
        }

        public static int BIT5E(Processor cpu) // 0x6B
        {
            return FlipBit(cpu, 5, ref cpu.Registers.E);
        }

        public static int BIT5H(Processor cpu) // 0x6C
        {
            return FlipBit(cpu, 5, ref cpu.Registers.H);
        }

        public static int BIT5L(Processor cpu) // 0x6D
        {
            return FlipBit(cpu, 5, ref cpu.Registers.L);
        }

        public static int BIT5_HL_(Processor cpu) // 0x6E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 5, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT5A(Processor cpu) // 0x6F
        {
            return FlipBit(cpu, 5, ref cpu.Registers.A);
        }

        public static int BIT6B(Processor cpu) // 0x70
        {
            return FlipBit(cpu, 6, ref cpu.Registers.B);
        }

        public static int BIT6C(Processor cpu) // 0x71
        {
            return FlipBit(cpu, 6, ref cpu.Registers.C);
        }

        public static int BIT6D(Processor cpu) // 0x72
        {
            return FlipBit(cpu, 6, ref cpu.Registers.D);
        }

        public static int BIT6E(Processor cpu) // 0x73
        {
            return FlipBit(cpu, 6, ref cpu.Registers.E);
        }

        public static int BIT6H(Processor cpu) // 0x74
        {
            return FlipBit(cpu, 6, ref cpu.Registers.H);
        }

        public static int BIT6L(Processor cpu) // 0x75
        {
            return FlipBit(cpu, 6, ref cpu.Registers.L);
        }

        public static int BIT6_HL_(Processor cpu) // 0x76
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 6, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT6A(Processor cpu) // 0x77
        {
            return FlipBit(cpu, 6, ref cpu.Registers.A);
        }

        public static int BIT7B(Processor cpu) // 0x78
        {
            return FlipBit(cpu, 7, ref cpu.Registers.B);
        }

        public static int BIT7C(Processor cpu) // 0x79
        {
            return FlipBit(cpu, 7, ref cpu.Registers.C);
        }

        public static int BIT7D(Processor cpu) // 0x7A
        {
            return FlipBit(cpu, 7, ref cpu.Registers.D);
        }

        public static int BIT7E(Processor cpu) // 0x7B
        {
            return FlipBit(cpu, 7, ref cpu.Registers.E);
        }

        public static int BIT7H(Processor cpu) // 0x7C
        {
            return FlipBit(cpu, 7, ref cpu.Registers.H);
        }

        public static int BIT7L(Processor cpu) // 0x7D
        {
            return FlipBit(cpu, 7, ref cpu.Registers.L);
        }

        public static int BIT7_HL_(Processor cpu) // 0x7E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            FlipBit(cpu, 7, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int BIT7A(Processor cpu) // 0x7F
        {
            return FlipBit(cpu, 7, ref cpu.Registers.A);
        }

        public static int RES0B(Processor cpu) // 0x80
        {
            return Res(cpu, 0, ref cpu.Registers.B);
        }

        public static int RES0C(Processor cpu) // 0x81
        {
            return Res(cpu, 0, ref cpu.Registers.C);
        }

        public static int RES0D(Processor cpu) // 0x82
        {
            return Res(cpu, 0, ref cpu.Registers.D);
        }

        public static int RES0E(Processor cpu) // 0x83
        {
            return Res(cpu, 0, ref cpu.Registers.E);
        }

        public static int RES0H(Processor cpu) // 0x84
        {
            return Res(cpu, 0, ref cpu.Registers.H);
        }

        public static int RES0L(Processor cpu) // 0x85
        {
            return Res(cpu, 0, ref cpu.Registers.L);
        }

        public static int RES0_HL_(Processor cpu) // 0x86
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 0, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES0A(Processor cpu) // 0x87
        {
            return Res(cpu, 0, ref cpu.Registers.A);
        }

        public static int RES1B(Processor cpu) // 0x88
        {
            return Res(cpu, 1, ref cpu.Registers.B);
        }

        public static int RES1C(Processor cpu) // 0x89
        {
            return Res(cpu, 1, ref cpu.Registers.C);
        }

        public static int RES1D(Processor cpu) // 0x8A
        {
            return Res(cpu, 1, ref cpu.Registers.D);
        }

        public static int RES1E(Processor cpu) // 0x8B
        {
            return Res(cpu, 1, ref cpu.Registers.E);
        }

        public static int RES1H(Processor cpu) // 0x8C
        {
            return Res(cpu, 1, ref cpu.Registers.H);
        }

        public static int RES1L(Processor cpu) // 0x8D
        {
            return Res(cpu, 1, ref cpu.Registers.L);
        }

        public static int RES1_HL_(Processor cpu) // 0x8E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 1, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES1A(Processor cpu) // 0x8F
        {
            return Res(cpu, 1, ref cpu.Registers.A);
        }

        public static int RES2B(Processor cpu) // 0x90
        {
            return Res(cpu, 2, ref cpu.Registers.B);
        }

        public static int RES2C(Processor cpu) // 0x91
        {
            return Res(cpu, 2, ref cpu.Registers.C);
        }

        public static int RES2D(Processor cpu) // 0x92
        {
            return Res(cpu, 2, ref cpu.Registers.D);
        }

        public static int RES2E(Processor cpu) // 0x93
        {
            return Res(cpu, 2, ref cpu.Registers.E);
        }

        public static int RES2H(Processor cpu) // 0x94
        {
            return Res(cpu, 2, ref cpu.Registers.H);
        }

        public static int RES2L(Processor cpu) // 0x95
        {
            return Res(cpu, 2, ref cpu.Registers.L);
        }

        public static int RES2_HL_(Processor cpu) // 0x96
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 2, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES2A(Processor cpu) // 0x97
        {
            return Res(cpu, 2, ref cpu.Registers.A);
        }

        public static int RES3B(Processor cpu) // 0x98
        {
            return Res(cpu, 3, ref cpu.Registers.B);
        }

        public static int RES3C(Processor cpu) // 0x99
        {
            return Res(cpu, 3, ref cpu.Registers.C);
        }

        public static int RES3D(Processor cpu) // 0x9A
        {
            return Res(cpu, 3, ref cpu.Registers.D);
        }

        public static int RES3E(Processor cpu) // 0x9B
        {
            return Res(cpu, 3, ref cpu.Registers.E);
        }

        public static int RES3H(Processor cpu) // 0x9C
        {
            return Res(cpu, 3, ref cpu.Registers.H);
        }

        public static int RES3L(Processor cpu) // 0x9D
        {
            return Res(cpu, 3, ref cpu.Registers.L);
        }

        public static int RES3_HL_(Processor cpu) // 0x9E
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 3, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES3A(Processor cpu) // 0x9F
        {
            return Res(cpu, 3, ref cpu.Registers.A);
        }

        public static int RES4B(Processor cpu) // 0xA0
        {
            return Res(cpu, 4, ref cpu.Registers.B);
        }

        public static int RES4C(Processor cpu) // 0xA1
        {
            return Res(cpu, 4, ref cpu.Registers.C);
        }

        public static int RES4D(Processor cpu) // 0xA2
        {
            return Res(cpu, 4, ref cpu.Registers.D);
        }

        public static int RES4E(Processor cpu) // 0xA3
        {
            return Res(cpu, 4, ref cpu.Registers.E);
        }

        public static int RES4H(Processor cpu) // 0xA4
        {
            return Res(cpu, 4, ref cpu.Registers.H);
        }

        public static int RES4L(Processor cpu) // 0xA5
        {
            return Res(cpu, 4, ref cpu.Registers.L);
        }

        public static int RES4_HL_(Processor cpu) // 0xA6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 4, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES4A(Processor cpu) // 0xA7
        {
            return Res(cpu, 4, ref cpu.Registers.A);
        }

        public static int RES5B(Processor cpu) // 0xA8
        {
            return Res(cpu, 5, ref cpu.Registers.B);
        }

        public static int RES5C(Processor cpu) // 0xA9
        {
            return Res(cpu, 5, ref cpu.Registers.C);
        }

        public static int RES5D(Processor cpu) // 0xAA
        {
            return Res(cpu, 5, ref cpu.Registers.D);
        }

        public static int RES5E(Processor cpu) // 0xAB
        {
            return Res(cpu, 5, ref cpu.Registers.E);
        }

        public static int RES5H(Processor cpu) // 0xAC
        {
            return Res(cpu, 5, ref cpu.Registers.H);
        }

        public static int RES5L(Processor cpu) // 0xAD
        {
            return Res(cpu, 5, ref cpu.Registers.L);
        }

        public static int RES5_HL_(Processor cpu) // 0xAE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 5, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES5A(Processor cpu) // 0xAF
        {
            return Res(cpu, 5, ref cpu.Registers.A);
        }

        public static int RES6B(Processor cpu) // 0xB0
        {
            return Res(cpu, 6, ref cpu.Registers.B);
        }

        public static int RES6C(Processor cpu) // 0xB1
        {
            return Res(cpu, 6, ref cpu.Registers.C);
        }

        public static int RES6D(Processor cpu) // 0xB2
        {
            return Res(cpu, 6, ref cpu.Registers.D);
        }

        public static int RES6E(Processor cpu) // 0xB3
        {
            return Res(cpu, 6, ref cpu.Registers.E);
        }

        public static int RES6H(Processor cpu) // 0xB4
        {
            return Res(cpu, 6, ref cpu.Registers.H);
        }

        public static int RES6L(Processor cpu) // 0xB5
        {
            return Res(cpu, 6, ref cpu.Registers.L);
        }

        public static int RES6_HL_(Processor cpu) // 0xB6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 6, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES6A(Processor cpu) // 0xB7
        {
            return Res(cpu, 6, ref cpu.Registers.A);
        }

        public static int RES7B(Processor cpu) // 0xB8
        {
            return Res(cpu, 7, ref cpu.Registers.B);
        }

        public static int RES7C(Processor cpu) // 0xB9
        {
            return Res(cpu, 7, ref cpu.Registers.C);
        }

        public static int RES7D(Processor cpu) // 0xBA
        {
            return Res(cpu, 7, ref cpu.Registers.D);
        }

        public static int RES7E(Processor cpu) // 0xBB
        {
            return Res(cpu, 7, ref cpu.Registers.E);
        }

        public static int RES7H(Processor cpu) // 0xBC
        {
            return Res(cpu, 7, ref cpu.Registers.H);
        }

        public static int RES7L(Processor cpu) // 0xBD
        {
            return Res(cpu, 7, ref cpu.Registers.L);
        }

        public static int RES7_HL_(Processor cpu) // 0xBE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Res(cpu, 7, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int RES7A(Processor cpu) // 0xBF
        {
            return Res(cpu, 7, ref cpu.Registers.A);
        }

        public static int SET0B(Processor cpu) // 0xC0
        {
            return Set(cpu, 0, ref cpu.Registers.B);
        }

        public static int SET0C(Processor cpu) // 0xC1
        {
            return Set(cpu, 0, ref cpu.Registers.C);
        }

        public static int SET0D(Processor cpu) // 0xC2
        {
            return Set(cpu, 0, ref cpu.Registers.D);
        }

        public static int SET0E(Processor cpu) // 0xC3
        {
            return Set(cpu, 0, ref cpu.Registers.E);
        }

        public static int SET0H(Processor cpu) // 0xC4
        {
            return Set(cpu, 0, ref cpu.Registers.H);
        }

        public static int SET0L(Processor cpu) // 0xC5
        {
            return Set(cpu, 0, ref cpu.Registers.L);
        }

        public static int SET0_HL_(Processor cpu) // 0xC6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 0, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET0A(Processor cpu) // 0xC7
        {
            return Set(cpu, 0, ref cpu.Registers.A);
        }

        public static int SET1B(Processor cpu) // 0xC8
        {
            return Set(cpu, 1, ref cpu.Registers.B);
        }

        public static int SET1C(Processor cpu) // 0xC9
        {
            return Set(cpu, 1, ref cpu.Registers.C);
        }

        public static int SET1D(Processor cpu) // 0xCA
        {
            return Set(cpu, 1, ref cpu.Registers.D);
        }

        public static int SET1E(Processor cpu) // 0xCB
        {
            return Set(cpu, 1, ref cpu.Registers.E);
        }

        public static int SET1H(Processor cpu) // 0xCC
        {
            return Set(cpu, 1, ref cpu.Registers.H);
        }

        public static int SET1L(Processor cpu) // 0xCD
        {
            return Set(cpu, 1, ref cpu.Registers.L);
        }

        public static int SET1_HL_(Processor cpu) // 0xCE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 1, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET1A(Processor cpu) // 0xCF
        {
            return Set(cpu, 1, ref cpu.Registers.A);
        }

        public static int SET2B(Processor cpu) // 0xD0
        {
            return Set(cpu, 2, ref cpu.Registers.B);
        }

        public static int SET2C(Processor cpu) // 0xD1
        {
            return Set(cpu, 2, ref cpu.Registers.C);
        }

        public static int SET2D(Processor cpu) // 0xD2
        {
            return Set(cpu, 2, ref cpu.Registers.D);
        }

        public static int SET2E(Processor cpu) // 0xD3
        {
            return Set(cpu, 2, ref cpu.Registers.E);
        }

        public static int SET2H(Processor cpu) // 0xD4
        {
            return Set(cpu, 2, ref cpu.Registers.H);
        }

        public static int SET2L(Processor cpu) // 0xD5
        {
            return Set(cpu, 2, ref cpu.Registers.L);
        }

        public static int SET2_HL_(Processor cpu) // 0xD6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 2, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET2A(Processor cpu) // 0xD7
        {
            return Set(cpu, 2, ref cpu.Registers.A);
        }

        public static int SET3B(Processor cpu) // 0xD8
        {
            return Set(cpu, 3, ref cpu.Registers.B);
        }

        public static int SET3C(Processor cpu) // 0xD9
        {
            return Set(cpu, 3, ref cpu.Registers.C);
        }

        public static int SET3D(Processor cpu) // 0xDA
        {
            return Set(cpu, 3, ref cpu.Registers.D);
        }

        public static int SET3E(Processor cpu) // 0xDB
        {
            return Set(cpu, 3, ref cpu.Registers.E);
        }

        public static int SET3H(Processor cpu) // 0xDC
        {
            return Set(cpu, 3, ref cpu.Registers.H);
        }

        public static int SET3L(Processor cpu) // 0xDD
        {
            return Set(cpu, 3, ref cpu.Registers.L);
        }

        public static int SET3_HL_(Processor cpu) // 0xDE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 3, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET3A(Processor cpu) // 0xDF
        {
            return Set(cpu, 3, ref cpu.Registers.A);
        }

        public static int SET4B(Processor cpu) // 0xE0
        {
            return Set(cpu, 4, ref cpu.Registers.B);
        }

        public static int SET4C(Processor cpu) // 0xE1
        {
            return Set(cpu, 4, ref cpu.Registers.C);
        }

        public static int SET4D(Processor cpu) // 0xE2
        {
            return Set(cpu, 4, ref cpu.Registers.D);
        }

        public static int SET4E(Processor cpu) // 0xE3
        {
            return Set(cpu, 4, ref cpu.Registers.E);
        }

        public static int SET4H(Processor cpu) // 0xE4
        {
            return Set(cpu, 4, ref cpu.Registers.H);
        }

        public static int SET4L(Processor cpu) // 0xE5
        {
            return Set(cpu, 4, ref cpu.Registers.L);
        }

        public static int SET4_HL_(Processor cpu) // 0xE6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 4, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET4A(Processor cpu) // 0xE7
        {
            return Set(cpu, 4, ref cpu.Registers.A);
        }

        public static int SET5B(Processor cpu) // 0xE8
        {
            return Set(cpu, 5, ref cpu.Registers.B);
        }

        public static int SET5C(Processor cpu) // 0xE9
        {
            return Set(cpu, 5, ref cpu.Registers.C);
        }

        public static int SET5D(Processor cpu) // 0xEA
        {
            return Set(cpu, 5, ref cpu.Registers.D);
        }

        public static int SET5E(Processor cpu) // 0xEB
        {
            return Set(cpu, 5, ref cpu.Registers.E);
        }

        public static int SET5H(Processor cpu) // 0xEC
        {
            return Set(cpu, 5, ref cpu.Registers.H);
        }

        public static int SET5L(Processor cpu) // 0xED
        {
            return Set(cpu, 5, ref cpu.Registers.L);
        }

        public static int SET5_HL_(Processor cpu) // 0xEE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 5, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET5A(Processor cpu) // 0xEF
        {
            return Set(cpu, 5, ref cpu.Registers.A);
        }

        public static int SET6B(Processor cpu) // 0xF0
        {
            return Set(cpu, 6, ref cpu.Registers.B);
        }

        public static int SET6C(Processor cpu) // 0xF1
        {
            return Set(cpu, 6, ref cpu.Registers.C);
        }

        public static int SET6D(Processor cpu) // 0xF2
        {
            return Set(cpu, 6, ref cpu.Registers.D);
        }

        public static int SET6E(Processor cpu) // 0xF3
        {
            return Set(cpu, 6, ref cpu.Registers.E);
        }

        public static int SET6H(Processor cpu) // 0xF4
        {
            return Set(cpu, 6, ref cpu.Registers.H);
        }

        public static int SET6L(Processor cpu) // 0xF5
        {
            return Set(cpu, 6, ref cpu.Registers.L);
        }

        public static int SET6_HL_(Processor cpu) // 0xF6
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 6, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET6A(Processor cpu) // 0xF7
        {
            return Set(cpu, 6, ref cpu.Registers.A);
        }

        public static int SET7B(Processor cpu) // 0xF8
        {
            return Set(cpu, 7, ref cpu.Registers.B);
        }

        public static int SET7C(Processor cpu) // 0xF9
        {
            return Set(cpu, 7, ref cpu.Registers.C);
        }

        public static int SET7D(Processor cpu) // 0xFA
        {
            return Set(cpu, 7, ref cpu.Registers.D);
        }

        public static int SET7E(Processor cpu) // 0xFB
        {
            return Set(cpu, 7, ref cpu.Registers.E);
        }

        public static int SET7H(Processor cpu) // 0xFC
        {
            return Set(cpu, 7, ref cpu.Registers.H);
        }

        public static int SET7L(Processor cpu) // 0xFD
        {
            return Set(cpu, 7, ref cpu.Registers.L);
        }

        public static int SET7_HL_(Processor cpu) // 0xFE
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Set(cpu, 7, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 4;
        }

        public static int SET7A(Processor cpu) // 0xFF
        {
            return Set(cpu, 7, ref cpu.Registers.A);
        }
    }
}
