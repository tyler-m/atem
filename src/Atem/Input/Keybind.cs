using Atem.Input.Command;

namespace Atem.Input
{
    public class Keybind
    {
        public CommandType CommandType { get; set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public int Key { get; set; }
    }
}
