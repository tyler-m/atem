﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Atem.Core.Processing.Instructions
{
    internal static class Arithmetic
    {
        public static void PopulateLookup(Dictionary<byte, Func<Processor, int>> lookup)
        {
            lookup.Add(0x07, RLCA);
            lookup.Add(0x0F, RRCA);
            lookup.Add(0x17, RLA);
            lookup.Add(0x1F, RRA);
            lookup.Add(0x27, DAA);
            lookup.Add(0x2F, CPL);
            lookup.Add(0x37, SCF);
            lookup.Add(0x3F, CCF);

            lookup.Add(0x03, INCBC);
            lookup.Add(0x13, INCDE);
            lookup.Add(0x23, INCHL);
            lookup.Add(0x33, INCSP);
            lookup.Add(0x3C, INCA);
            lookup.Add(0x04, INCB);
            lookup.Add(0x0C, INCC);
            lookup.Add(0x14, INCD);
            lookup.Add(0x1C, INCE);
            lookup.Add(0x24, INCH);
            lookup.Add(0x2C, INCL);
            lookup.Add(0x34, INC_HL_);

            lookup.Add(0x0B, DECBC);
            lookup.Add(0x1B, DECDE);
            lookup.Add(0x2B, DECHL);
            lookup.Add(0x3B, DECSP);
            lookup.Add(0x3D, DECA);
            lookup.Add(0x05, DECB);
            lookup.Add(0x0D, DECC);
            lookup.Add(0x15, DECD);
            lookup.Add(0x1D, DECE);
            lookup.Add(0x25, DECH);
            lookup.Add(0x2D, DECL);
            lookup.Add(0x35, DEC_HL_);

            lookup.Add(0x87, ADDA);
            lookup.Add(0x80, ADDB);
            lookup.Add(0x81, ADDC);
            lookup.Add(0x82, ADDD);
            lookup.Add(0x83, ADDE);
            lookup.Add(0x84, ADDH);
            lookup.Add(0x85, ADDL);
            lookup.Add(0x86, ADD_HL_);
            lookup.Add(0xC6, ADDAU8);
            lookup.Add(0x09, ADDHLBC);
            lookup.Add(0x19, ADDHLDE);
            lookup.Add(0x29, ADDHLHL);
            lookup.Add(0x39, ADDHLSP);
            lookup.Add(0xE8, ADDSPS8);

            lookup.Add(0x88, ADCAB);
            lookup.Add(0x89, ADCAC);
            lookup.Add(0x8A, ADCAD);
            lookup.Add(0x8B, ADCAE);
            lookup.Add(0x8C, ADCAH);
            lookup.Add(0x8D, ADCAL);
            lookup.Add(0x8E, ADCA_HL_);
            lookup.Add(0x8F, ADCAA);
            lookup.Add(0xCE, ADCAU8);

            lookup.Add(0x90, SUBB);
            lookup.Add(0x91, SUBC);
            lookup.Add(0x92, SUBD);
            lookup.Add(0x93, SUBE);
            lookup.Add(0x94, SUBH);
            lookup.Add(0x95, SUBL);
            lookup.Add(0x96, SUB_HL_);
            lookup.Add(0x97, SUBA);
            lookup.Add(0xD6, SUBAU8);

            lookup.Add(0x98, SBCB);
            lookup.Add(0x99, SBCC);
            lookup.Add(0x9A, SBCD);
            lookup.Add(0x9B, SBCE);
            lookup.Add(0x9C, SBCH);
            lookup.Add(0x9D, SBCL);
            lookup.Add(0x9E, SBC_HL_);
            lookup.Add(0x9F, SBCA);
            lookup.Add(0xDE, SBCAU8);

            lookup.Add(0xA0, ANDB);
            lookup.Add(0xA1, ANDC);
            lookup.Add(0xA2, ANDD);
            lookup.Add(0xA3, ANDE);
            lookup.Add(0xA4, ANDH);
            lookup.Add(0xA5, ANDL);
            lookup.Add(0xA6, AND_HL_);
            lookup.Add(0xA7, ANDA);
            lookup.Add(0xE6, ANDAU8);

            lookup.Add(0xA8, XORB);
            lookup.Add(0xA9, XORC);
            lookup.Add(0xAA, XORD);
            lookup.Add(0xAB, XORE);
            lookup.Add(0xAC, XORH);
            lookup.Add(0xAD, XORL);
            lookup.Add(0xAE, XOR_HL_);
            lookup.Add(0xAF, XORA);
            lookup.Add(0xEE, XORAU8);

            lookup.Add(0xB0, ORB);
            lookup.Add(0xB1, ORC);
            lookup.Add(0xB2, ORD);
            lookup.Add(0xB3, ORE);
            lookup.Add(0xB4, ORH);
            lookup.Add(0xB5, ORL);
            lookup.Add(0xB6, OR_HL_);
            lookup.Add(0xB7, ORA);
            lookup.Add(0xF6, ORAU8);

            lookup.Add(0xB8, CPAB);
            lookup.Add(0xB9, CPAC);
            lookup.Add(0xBA, CPAD);
            lookup.Add(0xBB, CPAE);
            lookup.Add(0xBC, CPAH);
            lookup.Add(0xBD, CPAL);
            lookup.Add(0xBE, CPA_HL_);
            lookup.Add(0xBF, CPAA);
            lookup.Add(0xFE, CPAU8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Inc(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = value.WillHalfCarry(1);
            value++;
            cpu.Registers.Flags.Z = value == 0;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Dec(Processor cpu, ref byte value)
        {
            cpu.Registers.Flags.N = true;
            cpu.Registers.Flags.H = value.WillHalfBorrow(1);
            value--;
            cpu.Registers.Flags.Z = value == 0;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Add(Processor cpu, byte value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = cpu.Registers.A.WillHalfCarry(value);
            cpu.Registers.Flags.C = cpu.Registers.A.WillCarry(value);
            cpu.Registers.A = (byte)(cpu.Registers.A + value);
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ADDHL(Processor cpu, ushort value)
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = cpu.Registers.HL.WillHalfCarry(value);
            cpu.Registers.Flags.C = cpu.Registers.HL.WillCarry(value);
            cpu.Registers.HL += value;
            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ADC(Processor cpu, byte value)
        {
            byte newValue = (byte)(cpu.Registers.A + (cpu.Registers.Flags.C ? 1 : 0));
            cpu.Registers.Flags.H = cpu.Registers.A.WillHalfCarry(cpu.Registers.Flags.C ? 1 : 0);
            cpu.Registers.Flags.C = cpu.Registers.A.WillCarry(cpu.Registers.Flags.C ? 1 : 0);
            cpu.Registers.Flags.H |= newValue.WillHalfCarry(value);
            cpu.Registers.Flags.C |= newValue.WillCarry(value);
            cpu.Registers.Flags.N = false;
            newValue += value;
            cpu.Registers.Flags.Z = newValue == 0;
            cpu.Registers.A = newValue;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Sub(Processor cpu, byte value)
        {
            cpu.Registers.Flags.N = true;
            cpu.Registers.Flags.H = cpu.Registers.A.WillHalfBorrow(value);
            cpu.Registers.Flags.C = cpu.Registers.A.WillBorrow(value);
            cpu.Registers.A = (byte)(cpu.Registers.A - value);
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SBC(Processor cpu, byte value)
        {
            int carry = cpu.Registers.Flags.C ? 1 : 0;
            cpu.Registers.Flags.H = cpu.Registers.A.WillHalfBorrow(carry);
            cpu.Registers.Flags.C = cpu.Registers.A.WillBorrow(carry);
            cpu.Registers.A -= (byte)(carry);
            cpu.Registers.Flags.H |= cpu.Registers.A.WillHalfBorrow(value);
            cpu.Registers.Flags.C |= cpu.Registers.A.WillBorrow(value);
            cpu.Registers.Flags.N = true;
            cpu.Registers.A -= value;
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AND(Processor cpu, byte value)
        {
            cpu.Registers.A &= value;
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = true;
            cpu.Registers.Flags.C = false;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int XOR(Processor cpu, byte value)
        {
            cpu.Registers.A ^= value;
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = false;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int OR(Processor cpu, byte value)
        {
            cpu.Registers.A |= value;
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = false;
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CP(Processor cpu, byte value)
        {
            ushort value2 = value;
            byte newValue = (byte)(cpu.Registers.A - value2);
            cpu.Registers.Flags.Z = newValue == 0;
            cpu.Registers.Flags.N = true;
            cpu.Registers.Flags.H = cpu.Registers.A.WillHalfBorrow(value2);
            cpu.Registers.Flags.C = cpu.Registers.A.WillBorrow(value2);
            return 1;
        }

        public static int RLCA(Processor cpu) // 0x07
        {
            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = cpu.Registers.A.GetBit(7);
            cpu.Registers.A = (byte)(cpu.Registers.A << 1);
            cpu.Registers.A = cpu.Registers.A.SetBit(0, cpu.Registers.Flags.C);
            return 1;
        }

        public static int RRCA(Processor cpu) // 0x0F
        {
            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = cpu.Registers.A.GetBit(0);
            cpu.Registers.A = (byte)(cpu.Registers.A >> 1);
            cpu.Registers.A = cpu.Registers.A.SetBit(7, cpu.Registers.Flags.C);
            return 1;
        }

        public static int RLA(Processor cpu) // 0x17
        {
            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            bool oldCarry = cpu.Registers.Flags.C;
            cpu.Registers.Flags.C = cpu.Registers.A.GetBit(7);
            cpu.Registers.A = (byte)(cpu.Registers.A << 1);
            cpu.Registers.A = cpu.Registers.A.SetBit(0, oldCarry);
            return 1;
        }

        public static int RRA(Processor cpu) // 0x1F
        {
            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            bool oldCarry = cpu.Registers.Flags.C;
            cpu.Registers.Flags.C = cpu.Registers.A.GetBit(0);
            cpu.Registers.A = (byte)(cpu.Registers.A >> 1);
            cpu.Registers.A = cpu.Registers.A.SetBit(7, oldCarry);
            return 1;
        }

        public static int DAA(Processor cpu) // 0x27
        {
            int adj = 0;
            if (cpu.Registers.Flags.C || cpu.Registers.A > 0x99 && !cpu.Registers.Flags.N)
            {
                cpu.Registers.Flags.C = !cpu.Registers.Flags.N || cpu.Registers.Flags.C;
                adj += 0x60;
            }
            if (cpu.Registers.Flags.H || (cpu.Registers.A & 0x0F) > 0x09 && !cpu.Registers.Flags.N)
            {
                adj += 0x06;
            }
            cpu.Registers.A += (byte)(cpu.Registers.Flags.N ? -adj : adj);
            cpu.Registers.Flags.Z = cpu.Registers.A == 0;
            cpu.Registers.Flags.H = false;
            return 1;
        }

        public static int CPL(Processor cpu) // 0x2F
        {
            cpu.Registers.A = (byte)~cpu.Registers.A;
            cpu.Registers.Flags.N = true;
            cpu.Registers.Flags.H = true;
            return 1;
        }

        public static int SCF(Processor cpu) // 0x37
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = true;
            return 1;
        }

        public static int CCF(Processor cpu) // 3F
        {
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = false;
            cpu.Registers.Flags.C = !cpu.Registers.Flags.C;
            return 1;
        }

        public static int INCBC(Processor cpu) // 0x03
        {
            cpu.Registers.BC++;
            return 2;
        }

        public static int INCDE(Processor cpu) // 0x13
        {
            cpu.Registers.DE++;
            return 2;
        }

        public static int INCHL(Processor cpu) // 0x23
        {
            cpu.Registers.HL++;
            return 2;
        }

        public static int INCSP(Processor cpu) // 0x33
        {
            cpu.Registers.SP++;
            return 2;
        }

        public static int INCB(Processor cpu) // 0x04
        {
            return Inc(cpu, ref cpu.Registers.B);
        }

        public static int INCD(Processor cpu) // 0x14
        {
            return Inc(cpu, ref cpu.Registers.D);
        }

        public static int INCH(Processor cpu) // 0x24
        {
            return Inc(cpu, ref cpu.Registers.H);
        }

        public static int INC_HL_(Processor cpu) // 0x34
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Inc(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int INCC(Processor cpu) // 0x0C
        {
            return Inc(cpu, ref cpu.Registers.C);
        }

        public static int INCE(Processor cpu) // 0x1C
        {
            return Inc(cpu, ref cpu.Registers.E);
        }

        public static int INCL(Processor cpu) // 0x2C
        {
            return Inc(cpu, ref cpu.Registers.L);
        }

        public static int INCA(Processor cpu) // 0x3C
        {
            return Inc(cpu, ref cpu.Registers.A);
        }

        public static int DECBC(Processor cpu) // 0x0B
        {
            cpu.Registers.BC--;
            return 2;
        }

        public static int DECDE(Processor cpu) // 0x1B
        {
            cpu.Registers.DE--;
            return 2;
        }

        public static int DECHL(Processor cpu) // 0x2B
        {
            cpu.Registers.HL--;
            return 2;
        }

        public static int DECSP(Processor cpu) // 0x3B
        {
            cpu.Registers.SP--;
            return 2;
        }

        public static int DECB(Processor cpu) // 0x05
        {
            return Dec(cpu, ref cpu.Registers.B);
        }

        public static int DECC(Processor cpu) // 0x0D
        {
            return Dec(cpu, ref cpu.Registers.C);
        }

        public static int DECD(Processor cpu) // 0x15
        {
            return Dec(cpu, ref cpu.Registers.D);
        }

        public static int DECE(Processor cpu) // 0x1D
        {
            return Dec(cpu, ref cpu.Registers.E);
        }

        public static int DECH(Processor cpu) // 0x25
        {
            return Dec(cpu, ref cpu.Registers.H);
        }

        public static int DECL(Processor cpu) // 0x2D
        {
            return Dec(cpu, ref cpu.Registers.L);
        }

        public static int DEC_HL_(Processor cpu) // 0x35
        {
            byte value = cpu.ReadBus(cpu.Registers.HL);
            Dec(cpu, ref value);
            cpu.WriteBus(cpu.Registers.HL, value);
            return 3;
        }

        public static int DECA(Processor cpu) // 0x3D
        {
            return Dec(cpu, ref cpu.Registers.A);
        }

        public static int ADDB(Processor cpu) // 0x80
        {
            return Add(cpu, cpu.Registers.B);
        }

        public static int ADDC(Processor cpu) // 0x81
        {
            return Add(cpu, cpu.Registers.C);
        }

        public static int ADDD(Processor cpu) // 0x82
        {
            return Add(cpu, cpu.Registers.D);
        }

        public static int ADDE(Processor cpu) // 0x83
        {
            return Add(cpu, cpu.Registers.E);
        }

        public static int ADDH(Processor cpu) // 0x84
        {
            return Add(cpu, cpu.Registers.H);
        }

        public static int ADDL(Processor cpu) // 0x85
        {
            return Add(cpu, cpu.Registers.L);
        }

        public static int ADD_HL_(Processor cpu) // 0x86
        {
            Add(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int ADDA(Processor cpu) // 0x87
        {
            return Add(cpu, cpu.Registers.A);
        }

        public static int ADDAU8(Processor cpu) // 0xC6
        {
            Add(cpu, cpu.ReadByte());
            return 2;
        }

        public static int ADDHLBC(Processor cpu) // 0x09
        {
            return ADDHL(cpu, cpu.Registers.BC);
        }

        public static int ADDHLDE(Processor cpu) // 0x19
        {
            return ADDHL(cpu, cpu.Registers.DE);
        }

        public static int ADDHLHL(Processor cpu) // 0x29
        {
            return ADDHL(cpu, cpu.Registers.HL);
        }

        public static int ADDHLSP(Processor cpu) // 0x39
        {
            return ADDHL(cpu, cpu.Registers.SP);
        }

        public static int ADDSPS8(Processor cpu) // 0xE8
        {
            sbyte offset = (sbyte)cpu.ReadByte();

            cpu.Registers.Flags.Z = false;
            cpu.Registers.Flags.N = false;
            cpu.Registers.Flags.H = cpu.Registers.SP.GetLowByte().WillHalfCarry(offset);
            cpu.Registers.Flags.C = cpu.Registers.SP.GetLowByte().WillCarry(offset);

            if (offset >= 0)
            {
                cpu.Registers.SP += (byte)offset;
            }
            else
            {
                cpu.Registers.SP -= (byte)Math.Abs(offset);
            }

            return 4;
        }

        public static int ADCAB(Processor cpu) // 0x88
        {
            return ADC(cpu, cpu.Registers.B);
        }

        public static int ADCAC(Processor cpu) // 0x89
        {
            return ADC(cpu, cpu.Registers.C);
        }

        public static int ADCAD(Processor cpu) // 0x8A
        {
            return ADC(cpu, cpu.Registers.D);
        }

        public static int ADCAE(Processor cpu) // 0x8B
        {
            return ADC(cpu, cpu.Registers.E);
        }

        public static int ADCAH(Processor cpu) // 0x8C
        {
            return ADC(cpu, cpu.Registers.H);
        }

        public static int ADCAL(Processor cpu) // 0x8D
        {
            return ADC(cpu, cpu.Registers.L);
        }

        public static int ADCA_HL_(Processor cpu) // 0x8E
        {
            ADC(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int ADCAA(Processor cpu) // 0x8F
        {
            return ADC(cpu, cpu.Registers.A);
        }

        private static int ADCAU8(Processor cpu) // 0xCE
        {
            ADC(cpu, cpu.ReadByte());
            return 2;
        }

        public static int SUBB(Processor cpu) // 0x90
        {
            return Sub(cpu, cpu.Registers.B);
        }

        public static int SUBC(Processor cpu) // 0x91
        {
            return Sub(cpu, cpu.Registers.C);
        }

        public static int SUBD(Processor cpu) // 0x92
        {
            return Sub(cpu, cpu.Registers.D);
        }

        public static int SUBE(Processor cpu) // 0x93
        {
            return Sub(cpu, cpu.Registers.E);
        }

        public static int SUBH(Processor cpu) // 0x94
        {
            return Sub(cpu, cpu.Registers.H);
        }

        public static int SUBL(Processor cpu) // 0x95
        {
            return Sub(cpu, cpu.Registers.L);
        }

        public static int SUB_HL_(Processor cpu) // 0x96
        {
            Sub(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int SUBA(Processor cpu) // 0x97
        {
            return Sub(cpu, cpu.Registers.A);
        }

        private static int SUBAU8(Processor cpu) // 0xD6
        {
            Sub(cpu, cpu.ReadByte());
            return 2;
        }

        public static int SBCB(Processor cpu) // 0x98
        {
            return SBC(cpu, cpu.Registers.B);
        }

        public static int SBCC(Processor cpu) // 0x99
        {
            return SBC(cpu, cpu.Registers.C);
        }

        public static int SBCD(Processor cpu) // 0x9A
        {
            return SBC(cpu, cpu.Registers.D);
        }

        public static int SBCE(Processor cpu) // 0x9B
        {
            return SBC(cpu, cpu.Registers.E);
        }

        public static int SBCH(Processor cpu) // 0x9C
        {
            return SBC(cpu, cpu.Registers.H);
        }

        public static int SBCL(Processor cpu) // 0x9D
        {
            return SBC(cpu, cpu.Registers.L);
        }

        public static int SBC_HL_(Processor cpu) // 0x9E
        {
            SBC(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int SBCA(Processor cpu) // 0x9F
        {
            return SBC(cpu, cpu.Registers.A);
        }

        private static int SBCAU8(Processor cpu) // 0xDE
        {
            SBC(cpu, cpu.ReadByte());
            return 2;
        }

        public static int ANDB(Processor cpu) // 0xA0;
        {
            return AND(cpu, cpu.Registers.B);
        }

        public static int ANDC(Processor cpu) // 0xA1;
        {
            return AND(cpu, cpu.Registers.C);
        }

        public static int ANDD(Processor cpu) // 0xA2;
        {
            return AND(cpu, cpu.Registers.D);
        }

        public static int ANDE(Processor cpu) // 0xA3;
        {
            return AND(cpu, cpu.Registers.E);
        }

        public static int ANDH(Processor cpu) // 0xA4;
        {
            return AND(cpu, cpu.Registers.H);
        }

        public static int ANDL(Processor cpu) // 0xA5;
        {
            return AND(cpu, cpu.Registers.L);
        }

        public static int AND_HL_(Processor cpu) // 0xA6;
        {
            AND(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int ANDA(Processor cpu) // 0xA7;
        {
            return AND(cpu, cpu.Registers.A);
        }

        private static int ANDAU8(Processor cpu) // 0xE6
        {
            AND(cpu, cpu.ReadByte());
            return 2;
        }

        public static int XORB(Processor cpu) // 0xA8
        {
            return XOR(cpu, cpu.Registers.B);
        }

        public static int XORC(Processor cpu) // 0xA9
        {
            return XOR(cpu, cpu.Registers.C);
        }

        public static int XORD(Processor cpu) // 0xAA
        {
            return XOR(cpu, cpu.Registers.D);
        }

        public static int XORE(Processor cpu) // 0xAB
        {
            return XOR(cpu, cpu.Registers.E);
        }

        public static int XORH(Processor cpu) // 0xAC
        {
            return XOR(cpu, cpu.Registers.H);
        }

        public static int XORL(Processor cpu) // 0xAD
        {
            return XOR(cpu, cpu.Registers.L);
        }

        public static int XOR_HL_(Processor cpu) // 0xAE
        {
            XOR(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int XORA(Processor cpu) // 0xAF
        {
            return XOR(cpu, cpu.Registers.A);
        }

        private static int XORAU8(Processor cpu) // 0xEE
        {
            XOR(cpu, cpu.ReadByte());
            return 2;
        }

        public static int ORB(Processor cpu) // 0xB0
        {
            return OR(cpu, cpu.Registers.B);
        }

        public static int ORC(Processor cpu) // 0xB1
        {
            return OR(cpu, cpu.Registers.C);
        }

        public static int ORD(Processor cpu) // 0xB2
        {
            return OR(cpu, cpu.Registers.D);
        }

        public static int ORE(Processor cpu) // 0xB3
        {
            return OR(cpu, cpu.Registers.E);
        }

        public static int ORH(Processor cpu) // 0xB4
        {
            return OR(cpu, cpu.Registers.H);
        }

        public static int ORL(Processor cpu) // 0xB5
        {
            return OR(cpu, cpu.Registers.L);
        }

        public static int OR_HL_(Processor cpu) // 0xB6
        {
            OR(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int ORA(Processor cpu) // 0xB7
        {
            return OR(cpu, cpu.Registers.A);
        }

        private static int ORAU8(Processor cpu) // 0xF6
        {
            OR(cpu, cpu.ReadByte());
            return 2;
        }

        public static int CPAB(Processor cpu) // 0xB8
        {
            return CP(cpu, cpu.Registers.B);
        }

        public static int CPAC(Processor cpu) // 0xB9
        {
            return CP(cpu, cpu.Registers.C);
        }

        public static int CPAD(Processor cpu) // 0xBA
        {
            return CP(cpu, cpu.Registers.D);
        }

        public static int CPAE(Processor cpu) // 0xBB
        {
            return CP(cpu, cpu.Registers.E);
        }

        public static int CPAH(Processor cpu) // 0xBC
        {
            return CP(cpu, cpu.Registers.H);
        }

        public static int CPAL(Processor cpu) // 0xBD
        {
            return CP(cpu, cpu.Registers.L);
        }

        public static int CPA_HL_(Processor cpu) // 0xBE
        {
            CP(cpu, cpu.ReadBus(cpu.Registers.HL));
            return 2;
        }

        public static int CPAA(Processor cpu) // 0xBF
        {
            return CP(cpu, cpu.Registers.A);
        }

        private static int CPAU8(Processor cpu) // 0xFE
        {
            CP(cpu, cpu.ReadByte());
            return 2;
        }
    }
}
