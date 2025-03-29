using System;

namespace Atem.Core.Debugging
{
    public class Breakpoint : IComparable<Breakpoint>
    {
        private ushort _address;
        private bool _enabled;
        private int _hitCount;

        public ushort Address { get => _address; set => _address = value; }
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public ref bool EnabledRef { get => ref _enabled; }
        public int HitCount { get => _hitCount; set => _hitCount = value; }

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
