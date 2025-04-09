using Microsoft.Xna.Framework.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.Input
{
    public class Keybind
    {
        public CommandType CommandType { get; set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public Keys Key { get; set; }
    }
}
