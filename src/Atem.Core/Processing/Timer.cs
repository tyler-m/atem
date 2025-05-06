using System.Collections.Generic;
using System.IO;
using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core.Processing
{
    public class Timer : IAddressable, IBootable, IStateful
    {
        private readonly Interrupt _interrupt;

        // should probably be incrementing _div every clock
        // and bit shifting it on read instead of using the
        // additional _divTick with conditionals
        private byte _div;

        private int _divTick;
        private int _timaTick;

        public Timer(Interrupt interrupt)
        {
            _interrupt = interrupt;
        }

        public byte DIV { get => _div; set => _div = 0; }
        public byte TIMA { get; set; }
        public byte TMA { get; set; }
        public byte TAC { get; set; }
        private bool TACEnabled => TAC.GetBit(2);

        private int TIMATickPeriod
        {
            get
            {
                int clockMode = TAC & 0b11;

                if (clockMode == 0)
                {
                    return 256; // 4194304Hz / 4096Hz / 4
                }
                else if (clockMode == 1)
                {
                    return 4; // 4194304Hz / 262144Hz / 4
                }
                else if (clockMode == 2)
                {
                    return 16; // 4194304Hz / 65536Hz / 4
                }
                else
                {
                    return 64; // 4194304Hz / 16384Hz / 4
                }
            }
        }

        private void IncrementTIMA()
        {
            if (TIMA.WillCarry(1))
            {
                TIMA = TMA;
                _interrupt.SetInterrupt(InterruptType.Timer);
            }
            else
            {
                TIMA++;
            }
        }

        public void Clock()
        {
            _divTick++;

            if (_divTick >= 64) // 4194304Hz / 16384Hz / 4
            {
                _div++;
                _divTick = 0;
            }

            if (TACEnabled)
            {
                _timaTick++;

                if (_timaTick >= TIMATickPeriod)
                {
                    IncrementTIMA();
                    _timaTick = 0;
                }
            }
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return address switch
            {
                0xFF04 => DIV,
                0xFF05 => TIMA,
                0xFF06 => TMA,
                0xFF07 => TAC,
                _ => 0xFF
            };
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            switch (address)
            {
                case 0xFF04:
                    DIV = value;
                    break;
                case 0xFF05:
                    TIMA = value;
                    break;
                case 0xFF06:
                    TMA = value;
                    break;
                case 0xFF07:
                    TAC = value;
                    break;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (0xFF04, 0xFF07); // registers
        }

        public void Boot(BootMode mode)
        {
            switch (mode)
            {
                case BootMode.CGB:
                    TIMA = 0x00;
                    TMA = 0x00;
                    TAC = 0xF8;
                    break;
                case BootMode.DMG:
                    TIMA = 0x00;
                    TMA = 0x00;
                    TAC = 0xF8;
                    break;
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(_div);
            writer.Write(TIMA);
            writer.Write(TMA);
            writer.Write(TAC);
            writer.Write(_divTick);
            writer.Write(_timaTick);
        }

        public void SetState(BinaryReader reader)
        {
            _div = reader.ReadByte();
            TIMA = reader.ReadByte();
            TMA = reader.ReadByte();
            TAC = reader.ReadByte();
            _divTick = reader.ReadInt32();
            _timaTick = reader.ReadInt32();
        }
    }
}
