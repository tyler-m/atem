using System;
using System.Collections.Generic;
using Atem.Input;
using Atem.Input.Command;

namespace Atem.Config
{
    public class AtemConfig : IConfig<AtemConfig>
    {
        public Dictionary<CommandType, List<Keybind>> Keybinds { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public float ScreenSizeFactor { get; set; }
        public float UserVolumeFactor { get; set; }

        public bool Equals(AtemConfig otherConfig)
        {
            if (otherConfig == null)
            {
                return false;
            }

            if (Keybinds == null || otherConfig.Keybinds == null)
            {
                if (Keybinds != null || otherConfig.Keybinds != null)
                {
                    return false;
                }
            }
            else
            {
                if (Keybinds.Count != otherConfig.Keybinds.Count)
                {
                    return false;
                }

                foreach ((CommandType type, List<Keybind> keybinds) in Keybinds)
                {
                    if (!otherConfig.Keybinds.TryGetValue(type, out List<Keybind> otherKeybinds))
                    {
                        return false;
                    }

                    if (keybinds.Count != otherKeybinds.Count)
                    {
                        return false;
                    }

                    // the order of keybinds matters
                    for (int i = 0; i < keybinds.Count; i++)
                    {
                        if (!keybinds[i].Equals(otherKeybinds[i]))
                        {
                            return false;
                        }
                    }
                }
            }

            // TODO: consider floating point equality issues due to precision
            return ScreenWidth == otherConfig.ScreenWidth
                && ScreenHeight == otherConfig.ScreenHeight
                && ScreenSizeFactor == otherConfig.ScreenSizeFactor
                && UserVolumeFactor == otherConfig.UserVolumeFactor;
        }

        public override bool Equals(object otherObject) => Equals(otherObject as AtemConfig);

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(ScreenWidth);
            hash.Add(ScreenHeight);
            hash.Add(ScreenSizeFactor);
            hash.Add(UserVolumeFactor);

            if (Keybinds != null)
            {
                foreach ((CommandType type, List<Keybind> keybinds) in Keybinds)
                {
                    hash.Add(type);
                    
                    foreach (Keybind keybind in keybinds)
                    {
                        hash.Add(keybind);
                    }
                }
            }

            return hash.ToHashCode();
        }
    }
}
