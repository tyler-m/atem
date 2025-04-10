using System;
using Atem.Input.Command;

namespace Atem.Input
{
    public class Keybind : IEquatable<Keybind>
    {
        public CommandType CommandType { get; set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public int Key { get; set; }

        public bool Equals(Keybind other)
        {
            if (other == null)
            {
                return false;
            }

            return CommandType == other.CommandType
                && Shift == other.Shift
                && Control == other.Control
                && Alt == other.Alt
                && Key == other.Key;
        }

        public override bool Equals(object otherObject) => Equals(otherObject as Keybind);

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(CommandType);
            hash.Add(Shift);
            hash.Add(Control);
            hash.Add(Alt);
            hash.Add(Key);

            return hash.ToHashCode();
        }
    }
}
