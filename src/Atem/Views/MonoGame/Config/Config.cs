using System.Collections.Generic;
using Atem.Views.MonoGame.Input;
using Atem.Views.MonoGame.Input.Command;

namespace Atem.Views.MonoGame.Config
{
    public class Config
    {
        public Dictionary<CommandType, List<Keybind>> Keybinds { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public float UserVolumeFactor { get; set; }
    }
}
