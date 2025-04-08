using Atem.Views.MonoGame.Input.Command;
using Microsoft.Xna.Framework.Input;

namespace Atem.Views.MonoGame.Input
{
    public class Keybind
    {
        public CommandType CommandType { get; set; }

        public bool Shift { get; set; }

        public Keys Key { get; set; }
    }
}
