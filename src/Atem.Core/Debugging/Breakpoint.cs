using System;

namespace Atem.Core.Debugging
{
    public class Breakpoint : IComparable<Breakpoint>
    {
        public ushort Address;
        public bool Enabled;
        public int HitCount;

        public Breakpoint(ushort address, bool enabled = true)
        {
            Address = address;
            Enabled = enabled;
        }

        public int CompareTo(Breakpoint other)
        {
            return Address - other.Address;
        }
    }
}
