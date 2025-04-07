using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Atem.Views.MonoGame.Config
{
    public class ViewConfig
    {
        public Dictionary<string, HashSet<Keys>> Commands { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public string RomsDirectory { get; set; }
        public float UserVolumeFactor { get; set; }

        public ViewConfig()
        {
            ScreenWidth = 160;
            ScreenHeight = 144;
            ScreenSizeFactor = 2.0f;
            RomsDirectory = "roms/";
            Commands = [];
            UserVolumeFactor = 1.0f;
        }
    }
}
