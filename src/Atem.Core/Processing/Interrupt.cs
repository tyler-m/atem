using System.IO;
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

    public class Interrupt : IStateful
    {
        public byte IE { get; set; }
        public byte IF { get; set; }

        public void SetInterrupt(InterruptType interruptType)
        {
            IF = IF.SetBit((int)interruptType);
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
