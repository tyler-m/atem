using System.Collections.Generic;
using System.IO;
using Atem.Core.Memory;
using Atem.Core.State;

namespace Atem.Core.Processing
{
    public enum InterruptType
    {
        VerticalBlank,
        STAT,
        Timer,
        Serial,
        Joypad
    }

    public class Interrupt : IAddressable, IBootable, IStateful
    {
        public byte IE { get; set; }
        public byte IF { get; set; }

        public void SetInterrupt(InterruptType interruptType)
        {
            IF = IF.SetBit((int)interruptType);
        }

        public byte Read(ushort address, bool ignoreAccessRestrictions = false)
        {
            return address switch
            {
                0xFF0F => IF,
                0xFFFF => IE,
                _ => 0xFF,
            };
        }

        public void Write(ushort address, byte value, bool ignoreAccessRestrictions = false)
        {
            switch (address)
            {
                case 0xFF0F:
                    IF = value;
                    break;
                case 0xFFFF:
                    IE = value;
                    break;
            }
        }

        public IEnumerable<(ushort Start, ushort End)> GetAddressRanges()
        {
            yield return (0xFF0F, 0xFF0F); // IF register
            yield return (0xFFFF, 0xFFFF); // IE register
        }

        public void Boot(BootMode mode)
        {
            switch (mode)
            {
                case BootMode.CGB:
                    IF = 0xE1;
                    IE = 0x00;
                    break;
                case BootMode.DMG:
                    IF = 0xE1;
                    IE = 0x00;
                    break;
            }
        }

        public void GetState(BinaryWriter writer)
        {
            writer.Write(IE);
            writer.Write(IF);
        }

        public void SetState(BinaryReader reader)
        {
            IE = reader.ReadByte();
            IF = reader.ReadByte();
        }
    }
}
