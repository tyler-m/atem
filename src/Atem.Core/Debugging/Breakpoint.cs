using System;

namespace Atem.Core.Debugging
{
    public class Breakpoint : IComparable<Breakpoint>
    {
        private bool _enabled;

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public ref bool EnabledRef { get => ref _enabled; }
        public ushort Address { get; set; }
        public int HitCount { get; set; }

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
