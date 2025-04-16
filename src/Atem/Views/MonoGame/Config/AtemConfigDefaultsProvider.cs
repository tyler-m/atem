using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Atem.Config;
using Atem.Input;
using Atem.Input.Command;

namespace Atem.Views.MonoGame.Config
{
    public class AtemConfigDefaultsProvider : IConfigDefaultsProvider<AtemConfig>
    {
        public AtemConfig GetDefaults()
        {
            return new AtemConfig()
            {
                WindowWidth = 640,
                WindowHeight = 480,
                ScreenSizeFactor = 2,
                ScreenSizeLocked = true,
                Keybinds = GetKeybinds(),
                UserVolumeFactor = 1.0f,
                RecentFiles = [],
            };
        }

        private Dictionary<CommandType, List<Keybind>> GetKeybinds()
        {
            Dictionary<CommandType, List<Keybind>> keybinds = [];
            AddGameboyCommands(keybinds);
            AddEmulatorControlCommands(keybinds);
            AddSaveStateCommands(keybinds);
            AddViewCommands(keybinds);
            return keybinds;
        }

        private static void AddSaveStateCommands(Dictionary<CommandType, List<Keybind>> keybinds)
        {
            keybinds.Add(CommandType.SaveState0, [new Keybind() { CommandType = CommandType.SaveState0, Alt = true, Key = (int)Keys.D1 }]);
            keybinds.Add(CommandType.SaveState1, [new Keybind() { CommandType = CommandType.SaveState1, Alt = true, Key = (int)Keys.D2 }]);
            keybinds.Add(CommandType.SaveState2, [new Keybind() { CommandType = CommandType.SaveState2, Alt = true, Key = (int)Keys.D3 }]);
            keybinds.Add(CommandType.SaveState3, [new Keybind() { CommandType = CommandType.SaveState3, Alt = true, Key = (int)Keys.D4 }]);
            keybinds.Add(CommandType.SaveState4, [new Keybind() { CommandType = CommandType.SaveState4, Alt = true, Key = (int)Keys.D5 }]);
            keybinds.Add(CommandType.SaveState5, [new Keybind() { CommandType = CommandType.SaveState5, Alt = true, Key = (int)Keys.D6 }]);
            keybinds.Add(CommandType.SaveState6, [new Keybind() { CommandType = CommandType.SaveState6, Alt = true, Key = (int)Keys.D7 }]);
            keybinds.Add(CommandType.SaveState7, [new Keybind() { CommandType = CommandType.SaveState7, Alt = true, Key = (int)Keys.D8 }]);
            keybinds.Add(CommandType.SaveState8, [new Keybind() { CommandType = CommandType.SaveState8, Alt = true, Key = (int)Keys.D9 }]);
            keybinds.Add(CommandType.SaveState9, [new Keybind() { CommandType = CommandType.SaveState9, Alt = true, Key = (int)Keys.D0 }]);

            keybinds.Add(CommandType.LoadState0, [new Keybind() { CommandType = CommandType.LoadState0, Control = true, Key = (int)Keys.D1 }]);
            keybinds.Add(CommandType.LoadState1, [new Keybind() { CommandType = CommandType.LoadState1, Control = true, Key = (int)Keys.D2 }]);
            keybinds.Add(CommandType.LoadState2, [new Keybind() { CommandType = CommandType.LoadState2, Control = true, Key = (int)Keys.D3 }]);
            keybinds.Add(CommandType.LoadState3, [new Keybind() { CommandType = CommandType.LoadState3, Control = true, Key = (int)Keys.D4 }]);
            keybinds.Add(CommandType.LoadState4, [new Keybind() { CommandType = CommandType.LoadState4, Control = true, Key = (int)Keys.D5 }]);
            keybinds.Add(CommandType.LoadState5, [new Keybind() { CommandType = CommandType.LoadState5, Control = true, Key = (int)Keys.D6 }]);
            keybinds.Add(CommandType.LoadState6, [new Keybind() { CommandType = CommandType.LoadState6, Control = true, Key = (int)Keys.D7 }]);
            keybinds.Add(CommandType.LoadState7, [new Keybind() { CommandType = CommandType.LoadState7, Control = true, Key = (int)Keys.D8 }]);
            keybinds.Add(CommandType.LoadState8, [new Keybind() { CommandType = CommandType.LoadState8, Control = true, Key = (int)Keys.D9 }]);
            keybinds.Add(CommandType.LoadState9, [new Keybind() { CommandType = CommandType.LoadState9, Control = true, Key = (int)Keys.D0 }]);
        }

        private static void AddViewCommands(Dictionary<CommandType, List<Keybind>> keybinds)
        {
            keybinds.Add(CommandType.Exit, [new Keybind() { CommandType = CommandType.Exit, Key = (int)Keys.Escape }]);
        }

        private static void AddEmulatorControlCommands(Dictionary<CommandType, List<Keybind>> keybinds)
        {
            keybinds.Add(CommandType.Continue, [new Keybind() { CommandType = CommandType.Continue, Key = (int)Keys.F5 }]);
            keybinds.Add(CommandType.Pause, [new Keybind() { CommandType = CommandType.Pause, Key = (int)Keys.Space }]);
        }

        private static void AddGameboyCommands(Dictionary<CommandType, List<Keybind>> keybinds)
        {
            keybinds.Add(CommandType.Up, [new Keybind() { CommandType = CommandType.Up, Key = (int)Keys.Up }]);
            keybinds.Add(CommandType.Down, [new Keybind() { CommandType = CommandType.Down, Key = (int)Keys.Down }]);
            keybinds.Add(CommandType.Left, [new Keybind() { CommandType = CommandType.Left, Key = (int)Keys.Left }]);
            keybinds.Add(CommandType.Right, [new Keybind() { CommandType = CommandType.Right, Key = (int)Keys.Right }]);
            keybinds.Add(CommandType.B, [new Keybind() { CommandType = CommandType.B, Key = (int)Keys.Z }]);
            keybinds.Add(CommandType.A, [new Keybind() { CommandType = CommandType.A, Key = (int)Keys.X }]);
            keybinds.Add(CommandType.Start, [new Keybind() { CommandType = CommandType.Start, Key = (int)Keys.Enter }]);
            keybinds.Add(CommandType.Select, [new Keybind() { CommandType = CommandType.Select, Key = (int)Keys.Back }]);
        }
    }
}
