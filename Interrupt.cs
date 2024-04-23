﻿
namespace Atem
{
    public enum InterruptType
    {
        VerticalBlank
    }

    internal class Interrupt
    {
        public byte IE { get; set; }
        public byte IF { get; set; }

        public void SetInterrupt(InterruptType interruptType)
        {
            IF = IF.SetBit((int)interruptType);
        }
    }
}