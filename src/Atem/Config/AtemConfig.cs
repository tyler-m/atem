using System.Collections.Generic;
using Atem.Input;
using Atem.Input.Command;

namespace Atem.Config
{
    public class AtemConfig
    {
        public Dictionary<CommandType, List<Keybind>> Keybinds { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public float UserVolumeFactor { get; set; }
    }
}
