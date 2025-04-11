using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Atem.Core.Processing.Instructions
{
    internal static class Control
    {
        public static void PopulateLookup(Dictionary<byte, Func<IProcessor, int>> lookup)
        {
            lookup.Add(0x00, NOP);
            lookup.Add(0x10, STOP);
            lookup.Add(0xCB, PREFIX);
            lookup.Add(0xF3, DI);
            lookup.Add(0xFB, EI);

            lookup.Add(0x20, JRNZ);
            lookup.Add(0x30, JRNC);
            lookup.Add(0x18, JR);
            lookup.Add(0x28, JRZ);
            lookup.Add(0x38, JRC);

            lookup.Add(0x76, HALT);

            lookup.Add(0xC0, RETNZ);
            lookup.Add(0xD0, RETNC);
            lookup.Add(0xC8, RETZ);
            lookup.Add(0xD8, RETC);
            lookup.Add(0xC9, RET);
            lookup.Add(0xD9, RETI);

            lookup.Add(0xC2, JPNZ);
            lookup.Add(0xD2, JPNC);
            lookup.Add(0xCA, JPZ);
            lookup.Add(0xDA, JPC);
            lookup.Add(0xC3, JP);
            lookup.Add(0xE9, JPHL);

            lookup.Add(0xC4, CALLNZ);
            lookup.Add(0xD4, CALLNC);
            lookup.Add(0xCC, CALLZ);
            lookup.Add(0xDC, CALLC);
            lookup.Add(0xCD, CALL);

            lookup.Add(0xC7, RST00);
            lookup.Add(0xD7, RST10);
            lookup.Add(0xE7, RST20);
            lookup.Add(0xF7, RST30);
            lookup.Add(0xCF, RST08);
            lookup.Add(0xDF, RST18);
            lookup.Add(0xEF, RST28);
            lookup.Add(0xFF, RST38);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RelativeJump(IProcessor cpu, bool condition)
        {
            sbyte offset = (sbyte)cpu.ReadByte();

            if (condition)
            {
                if (offset >= 0)
                {
                    cpu.Registers.PC = (ushort)(cpu.Registers.PC + offset);
                }
                else
                {
                    cpu.Registers.PC = (ushort)(cpu.Registers.PC - Math.Abs(offset));
                }

                return 3;
            }

            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Return(IProcessor cpu, bool condition, bool shortCircuit = false)
        {
            if (shortCircuit || condition)
            {
                cpu.Registers.PC = cpu.PopWord();
                return shortCircuit ? 4 : 5;
            }

            return 2;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Jump(IProcessor cpu, bool condition)
        {
            ushort address = cpu.ReadWord();
            if (condition)
            {
                cpu.Registers.PC = address;
                return 4;
            }

            return 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Call(IProcessor cpu, bool condition)
        {
            ushort address = cpu.ReadWord();
            if (condition)
            {
                cpu.PushWord(cpu.Registers.PC);
                cpu.Registers.PC = address;
                return 6;
            }

            return 3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RST(IProcessor cpu, ushort address)
        {
            cpu.PushWord(cpu.Registers.PC);
            cpu.Registers.PC = address;
            return 4;
        }

        public static int NOP(IProcessor cpu) // 0x00
        {
            return 1;
        }

        public static int STOP(IProcessor cpu) // 0x10
        {
            if (cpu.SpeedSwitchFlag)
            {
                cpu.SpeedSwitchFlag = false;
                cpu.DoubleSpeed = !cpu.DoubleSpeed;
            }

            return 1;
        }

        public static int HALT(IProcessor cpu) // 0x76
        {
            cpu.Halt();
            return 1;
        }

        public static int PREFIX(IProcessor cpu) // 0xCB
        {
            cpu.CB = true;
            return 1;
        }

        public static int DI(IProcessor cpu) // 0xF3
        {
            cpu.IME = false;
            return 1;
        }

        public static int EI(IProcessor cpu) // 0xFB
        {
            cpu.IME = true;
            return 1;
        }

        public static int JR(IProcessor cpu) // 0x18
        {
            return RelativeJump(cpu, true);
        }

        public static int JRNZ(IProcessor cpu) // 0x20
        {
            return RelativeJump(cpu, !cpu.Registers.Flags.Z);
        }

        public static int JRZ(IProcessor cpu) // 0x28
        {
            return RelativeJump(cpu, cpu.Registers.Flags.Z);
        }

        public static int JRNC(IProcessor cpu) // 0x30
        {
            return RelativeJump(cpu, !cpu.Registers.Flags.C);
        }

        public static int JRC(IProcessor cpu) // 0x38
        {
            return RelativeJump(cpu, cpu.Registers.Flags.C);
        }

        public static int RETNZ(IProcessor cpu) // 0xC0
        {
            return Return(cpu, !cpu.Registers.Flags.Z);
        }

        public static int RETZ(IProcessor cpu) // 0xC8
        {
            return Return(cpu, cpu.Registers.Flags.Z);
        }

        public static int RET(IProcessor cpu) // 0xC9
        {
            return Return(cpu, true, true);
        }

        public static int RETNC(IProcessor cpu) // 0xD0
        {
            return Return(cpu, !cpu.Registers.Flags.C);
        }

        public static int RETC(IProcessor cpu) // 0xD8
        {
            return Return(cpu, cpu.Registers.Flags.C);
        }

        public static int RETI(IProcessor cpu) // 0xD9
        {
            cpu.IME = true;
            return Return(cpu, true, true);
        }

        public static int JPNZ(IProcessor cpu) // 0xC2
        {
            return Jump(cpu, !cpu.Registers.Flags.Z);
        }

        public static int JP(IProcessor cpu) // 0xC3
        {
            return Jump(cpu, true);
        }

        public static int JPZ(IProcessor cpu) // 0xCA
        {
            return Jump(cpu, cpu.Registers.Flags.Z);
        }

        public static int JPNC(IProcessor cpu) // 0xD2
        {
            return Jump(cpu, !cpu.Registers.Flags.C);
        }

        public static int JPC(IProcessor cpu) // 0xDA
        {
            return Jump(cpu, cpu.Registers.Flags.C);
        }

        public static int JPHL(IProcessor cpu) // 0xE9
        {
            cpu.Registers.PC = cpu.Registers.HL;
            return 1;
        }

        public static int CALLNZ(IProcessor cpu) // 0xC4
        {
            return Call(cpu, !cpu.Registers.Flags.Z);
        }

        public static int CALLZ(IProcessor cpu) // 0xCC
        {
            return Call(cpu, cpu.Registers.Flags.Z);
        }

        public static int CALL(IProcessor cpu) // 0xCD
        {
            return Call(cpu, true);
        }

        public static int CALLNC(IProcessor cpu) // 0xD4
        {
            return Call(cpu, !cpu.Registers.Flags.C);
        }

        public static int CALLC(IProcessor cpu) // 0xDC
        {
            return Call(cpu, cpu.Registers.Flags.C);
        }

        public static int RST00(IProcessor cpu) // 0xC7
        {
            return RST(cpu, 0x00);
        }

        public static int RST08(IProcessor cpu) // 0xCF
        {
            return RST(cpu, 0x08);
        }

        public static int RST10(IProcessor cpu) // 0xD7
        {
            return RST(cpu, 0x10);
        }

        public static int RST18(IProcessor cpu) // 0xDF
        {
            return RST(cpu, 0x18);
        }

        public static int RST20(IProcessor cpu) // 0xE7
        {
            return RST(cpu, 0x20);
        }

        public static int RST28(IProcessor cpu) // 0xEF
        {
            return RST(cpu, 0x28);
        }

        public static int RST30(IProcessor cpu) // 0xF7
        {
            return RST(cpu, 0x30);
        }

        public static int RST38(IProcessor cpu) // 0xFF
        {
            return RST(cpu, 0x38);
        }
    }
}
